using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Voxelgon.Graphics;
using Voxelgon.Util;
using Voxelgon.Util.Geometry;
using Voxelgon.EventSystems;

namespace Voxelgon.Ship.Editor {

    public class ShipEditor : MonoBehaviour, IModeChangeHandler {
        //Fields

        public Object gridPrefab;
        public Object cursorPrefab;
        public Object lightPrefab;
        public Material gridMaterial;

        public float cursorHitbox = 0.1f;

        private Dictionary<GridSegment, List<Wall>> _edgeDictionary;
        private BVH<GridSegment> _edgeBVH;

        private Dictionary<GridVector, GameObject> _selectedNodes;

        private MeshBuilder _hullMeshBuilder = new MeshBuilder();

        private BuildMode _mode;
        private BuildState _state;

        private bool _onNode;
        private GridVector _cursorNode;
        private Vector3 _cursorOffset;
        private Vector3 _cursorPosition;

        private GameObject _gridObject;
        private GameObject _cursorObject;
        private GameObject _lightObject;

        //Properties

        public Wall TempWall { get; private set; }

        public List<Wall> Walls { get; private set; }

        public bool NodesChanged { get; private set; }

        public bool WallsChanged { get; private set; }

        public Mesh SimpleHullMesh {
            get {
                if (WallsChanged && Walls.Count > 0) {
                    var wallMeshes = Walls.Select(w => w.Mesh).ToList();
                    _hullMeshBuilder.Clear();
                    foreach (Wall w in Walls) {
                        _hullMeshBuilder.AddFragment(w.Mesh);
                    }
                }

                WallsChanged = false;
                return _hullMeshBuilder.FirstMesh;
            }
        }

        //Enums

        public enum BuildMode {
            Polygon,
            Wall
        }

        public enum BuildState {
            Add,
            Edit
        }

        //Methods

        public void OnModeChange(ModeChangeEventData eventData) {
        }

        public void Start() {
            _mode = BuildMode.Polygon;
            TempWall = new Wall(this);
            Walls = new List<Wall>();
            _edgeDictionary = new Dictionary<GridSegment, List<Wall>>();
            _edgeBVH = new GridBVH<GridSegment>();

            _selectedNodes = new Dictionary<GridVector, GameObject>();

            _gridObject = (GameObject)Instantiate(gridPrefab, transform.position, transform.rotation);
            _gridObject.transform.parent = transform;
            _cursorObject = (GameObject)Instantiate(cursorPrefab, transform.position, transform.rotation);
            _cursorObject.transform.parent = _gridObject.transform;
            _lightObject = (GameObject)Instantiate(lightPrefab, transform.position, transform.rotation);
            _lightObject.transform.parent = transform;
        }

        public void Update() {
            _cursorPosition = CalcCursorPosition();
            var cursorNodePosition = _cursorPosition.Round();
            _cursorOffset = _cursorPosition - cursorNodePosition;
            _cursorNode = (GridVector)cursorNodePosition;

            _gridObject.transform.localPosition = cursorNodePosition;
            gridMaterial.mainTextureOffset = new Vector2(_cursorOffset.x / 10, _cursorOffset.z / 10);

            _lightObject.transform.localPosition = _cursorPosition;

            _onNode = Mathf.Abs(_cursorOffset.x) < cursorHitbox && Mathf.Abs(_cursorOffset.z) < cursorHitbox;
            _cursorObject.SetActive(_onNode);

            if (Input.GetButtonDown("ChangeFloor")) {
                transform.Translate(Vector3.up * 2 * (int)Input.GetAxis("ChangeFloor"));
            }

            if (Input.GetButtonDown("Mouse0")) {
                if (_onNode) {
                    AddNode(_cursorNode);
                }
            }

            if (Input.GetButtonDown("Mouse1")) {
                if (_onNode) {
                    RemoveNode(_cursorNode);
                }
            }

        }

        public bool AddNode(GridVector node) {
            if (ValidNode(node)) {
                var selectedNode = GameObject.CreatePrimitive(PrimitiveType.Cube);
                selectedNode.name = "selectedNode";

                var nodeRenderer = selectedNode.GetComponent<MeshRenderer>();
                nodeRenderer.material.shader = Shader.Find("Unlit/Color");
                nodeRenderer.material.color = ColorPallette.gridSelected;

                selectedNode.transform.parent = transform.parent;
                selectedNode.transform.localPosition = (Vector3)node;
                selectedNode.transform.localScale = Vector3.one * 0.25f;

                selectedNode.GetComponent<BoxCollider>().size = Vector3.one * 1.5f;
                selectedNode.AddComponent<ShipEditorGridSelected>();

                _selectedNodes.Add(node, selectedNode);
                NodesChanged = true;
                return true;
            }
            return false;
        }

        public bool RemoveNode(GridVector node) {
            if (!_selectedNodes.ContainsKey(node))
                return false;

            Destroy(_selectedNodes[node]);
            _selectedNodes.Remove(node);
            NodesChanged = true;
            return true;
        }

        public bool ValidNode(GridVector node) {
            return !ContainsNode(node) && TempWall.ValidNode(node);
        }

        public bool ContainsNode(GridVector node) {
            return _selectedNodes.ContainsKey(node);
        }

        public void AddWall(Wall wall) {
            Walls.Add(TempWall);
            TempWall = new Wall();
            AddWallEdges(wall);
            _edgeBVH.DrawDebug(1000.0f);
        }

        public void RemoveWall(Wall wall) {
            RemoveWallEdges(wall);
            Walls.Remove(wall);
            WallsChanged = true;
        }

        public void FinalizeTempWall() {
            AddWall(TempWall);
            TempWall = new Wall(this);

            foreach (var g in _selectedNodes) {
                Destroy(g.Value);
            }
            _selectedNodes.Clear();

            NodesChanged = true;
        }

        public bool UpdateTempWall() {
            if (!NodesChanged || !TempWall.SetNodes(_selectedNodes.Keys.ToList(), _mode)) return false;

            NodesChanged = false;
            return true;
        }


        public List<Wall> GetWallNeighbors(Wall wall) {
            var neighbors = new List<Wall>();
            // TODO

            return neighbors;
        }

        public List<Wall> GetWallNeighbors(Wall wall, int edge) {
            var neighbors = new List<Wall>();
            // TODO

            return neighbors;
        }

        private Vector3 CalcCursorPosition() {
            var y = transform.position.y;
            var cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            var xySlope = cursorRay.direction.y / cursorRay.direction.x;
            var zySlope = cursorRay.direction.y / cursorRay.direction.z;

            var deltaY = cursorRay.origin.y - y;

            var xIntercept = cursorRay.origin.x + deltaY / -xySlope;
            var zIntercept = cursorRay.origin.z + deltaY / -zySlope;

            var interceptPoint = new Vector3(xIntercept, y, zIntercept);

            return interceptPoint;
        }

        public void AddWallEdges(Wall wall) {
            wall.Edges.ForEach(e => {
                if (!_edgeDictionary.ContainsKey(e)) {
                    _edgeDictionary.Add(e, new List<Wall>());
                    _edgeBVH.Add(e);
                }
                _edgeDictionary[e].Add(wall);
            });
        }

        private void RemoveWallEdges(Wall wall) {
            wall.Edges.ForEach(e => {
                if (_edgeDictionary.ContainsKey(e)) {
                    _edgeDictionary[e].Remove(wall);

                    if (_edgeDictionary[e].Count == 0) {
                        _edgeDictionary.Remove(e);
                        _edgeBVH.Remove(e);
                    }
                }
            });
        }

    }
}