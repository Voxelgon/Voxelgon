using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Voxelgon.Math;
using Voxelgon.Graphics;
using Voxelgon.EventSystems;

namespace Voxelgon.ShipEditor {
    public class ShipEditor : MonoBehaviour, IModeChangeHandler {
        //Fields

        private Dictionary<Position, List<Wall>> _wallVertices;

        private readonly List<Vector3> _nodes = new List<Vector3>();
        private readonly List<GameObject> _nodeObjects = new List<GameObject>();

        private Mesh _simpleHullMesh;

        //Properties

        public Wall TempWall { get; private set; }

        public List<Wall> Walls { get; private set; }

        public BuildMode Mode { get; set; }

        public bool NodesChanged { get; private set; }

        public bool WallsChanged { get; private set; }

        public Mesh SimpleHullMesh {
            get {
                if (WallsChanged && Walls.Count > 0) {
                    var wallMeshes = Walls.Select(w => w.ComplexMesh).ToList();
                    _simpleHullMesh.Clear();
                    _simpleHullMesh = Geometry.MergeMeshes(wallMeshes);
                }

                WallsChanged = false;
                return _simpleHullMesh;
            }
        }

        //Enums

        public enum BuildMode {
            Polygon,
            Rectangle
        }

        //Methods

        public void OnModeChange(ModeChangeEventData eventData) {
        }

        public void Start() {
            Mode = BuildMode.Polygon;
            TempWall = new Wall();
            _simpleHullMesh = new Mesh();
            Walls = new List<Wall>();
            _wallVertices = new Dictionary<Position, List<Wall>>();
        }

        public void Update() {
            if (Input.GetButtonDown("ChangeFloor")) {
                transform.Translate(Vector3.up*2*(int) Input.GetAxis("ChangeFloor"));
            }
        }

        public bool AddNode(Vector3 node) {
            if (ValidNode(node)) {
                var selectedNode = GameObject.CreatePrimitive(PrimitiveType.Cube);
                selectedNode.name = "selectedNode";

                var nodeRenderer = selectedNode.GetComponent<MeshRenderer>();
                nodeRenderer.material.shader = Shader.Find("Unlit/Color");
                nodeRenderer.material.color = ColorPallette.gridSelected;

                selectedNode.transform.parent = transform.parent;
                selectedNode.transform.localPosition = node;
                selectedNode.transform.localScale = Vector3.one*0.25f;

                selectedNode.GetComponent<BoxCollider>().size = Vector3.one*1.5f;
                selectedNode.AddComponent<ShipEditorGridSelected>();

                _nodes.Add(node);
                _nodeObjects.Add(selectedNode);
                NodesChanged = true;
                return true;
            }
            return false;
        }

        public bool RemoveNode(Vector3 node, GameObject obj) {
            if (!_nodes.Contains(node)) return false;

            _nodes.Remove(node);
            _nodeObjects.Remove(obj);
            NodesChanged = true;
            return true;
        }

        public bool ValidNode(Vector3 node) {
            return TempWall.ValidVertex(node) && !ContainsNode(node);
        }

        public bool ContainsNode(Vector3 node) {
            return _nodes.Contains(node);
        }

        public void AddWall(Wall wall) {
            foreach (var p in wall.Vertices.Select(v => (Position) v)) {
                if (!_wallVertices.ContainsKey(p)) {
                    _wallVertices.Add(p, new List<Wall>());
                }
                _wallVertices[p].Add(wall);
            }
            Walls.Add(wall);
            WallsChanged = true;
        }

        public void RemoveWall(Wall wall) {
            foreach (var p in wall
                .Vertices
                .Select(v => (Position) v)
                .Where(p => _wallVertices.ContainsKey(p))) {
                _wallVertices[p].Remove(wall);

                if (_wallVertices[p].Count == 0) {
                    _wallVertices.Remove(p);
                }
            }
            Walls.Remove(wall);
            WallsChanged = true;
        }

        public void FinalizeTempWall() {
            AddWall(TempWall);
            TempWall = new Wall(this);

            foreach (var g in _nodeObjects) {
                Destroy(g);
            }
            _nodeObjects.Clear();
            _nodes.Clear();

            NodesChanged = true;
        }

        public bool UpdateTempWall() {
            if (!NodesChanged || !TempWall.UpdateVertices(_nodes, Mode)) return false;

            NodesChanged = false;
            return true;
        }


        public List<Wall> GetWallNeighbors(Wall wall) {
            var lastList = _wallVertices[(Position) wall.Vertices[wall.VertexCount - 1]];
            var neighbors = new List<Wall>();

            foreach (var v in wall.Vertices) {
                var p = (Position) v;

                if (_wallVertices.ContainsKey(p)) {
                    foreach (var w in _wallVertices[p]) {
                        if (w != wall && lastList.Contains(w)) {
                            neighbors.Add(w);
                        }
                        lastList = _wallVertices[p];
                    }
                }
            }
            return neighbors;
        }

        public List<Wall> GetWallNeighbors(Wall wall, int edge) {
            var neighbors = new List<Wall>();

            var p1 = (Position) wall.Vertices[edge];
            var p2 = (Position) wall.Vertices[(edge + 1)%wall.VertexCount];

            if (!_wallVertices.ContainsKey(p1) || !_wallVertices.ContainsKey(p2)) return neighbors;


            var l1 = _wallVertices[p1];
            var l2 = _wallVertices[p2];

            neighbors.AddRange(l1.Where(w => w != wall && l2.Contains(w)));
            return neighbors;
        }

        public static Vector3 GetEditCursorPos(float y) {
            var cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            var xySlope = cursorRay.direction.y/cursorRay.direction.x;
            var zySlope = cursorRay.direction.y/cursorRay.direction.z;

            var deltaY = cursorRay.origin.y - y;

            var xIntercept = cursorRay.origin.x + deltaY/-xySlope;
            var zIntercept = cursorRay.origin.z + deltaY/-zySlope;

            var interceptPoint = new Vector3(xIntercept, y, zIntercept);

            return interceptPoint;
        }

        public static Vector3 GetEditCursorPos() {
            return GetEditCursorPos(0);
        }
    }
}