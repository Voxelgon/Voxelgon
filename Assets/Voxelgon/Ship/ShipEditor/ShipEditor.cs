using UnityEngine;
using System.Collections.Generic;
using System;
using Voxelgon.Util.Geometry;
using Voxelgon.EventSystems;

namespace Voxelgon.Ship.Editor {

    [Serializable]
    public class ShipEditor : MonoBehaviour, IModeChangeHandler {
        //Fields

        public const float NODE_RADIUS = 0.3f;
        public const float EDGE_RADIUS = 0.2f;

        private BuildMode _mode;

        private Hull _hull;

        // selection stuff
        private ISelectable _hoverObject;
        private List<ISelectable> _selectedObjects;

        private GridVector _cursorNode;
        private Vector3 _cursorOffset;
        private Vector3 _cursorPosition;

        // grid stuff
        private GameObject _gridObject;
        private GameObject _cursorObject;
        private GameObject _lightObject;
        public Material _gridMaterial;

        //Properties

        public Hull Hull {
            get { return _hull; }
        }


        //Enums

        public enum BuildMode {
            Polygon,
            Wall
        }

        //Methods

        public void OnModeChange(ModeChangeEventData eventData) {
        }

        public void Start() {
            BuildGrid();
        }

        public void Update() {
            UpdateGrid();

            //_onNode = Mathf.Abs(_cursorOffset.x) < cursorHitbox && Mathf.Abs(_cursorOffset.z) < cursorHitbox;
            //_cursorObject.SetActive(_onNode);

            if (Input.GetButtonDown("ChangeFloor")) {
                transform.Translate(Vector3.up * 2 * (int)Input.GetAxis("ChangeFloor"));
            }

            if (Input.GetButtonDown("Mouse0")) {
            }

            if (Input.GetButtonDown("Mouse1")) {
            }

        }

        /*
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
                }*/

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

        private void BuildGrid() {
            // load prefabs into memory
            var gridPrefab = Resources.Load("ShipEditor/Prefabs/EditorGrid", typeof(GameObject));
            var cursorPrefab = Resources.Load("ShipEditor/Prefabs/EditorCursor", typeof(GameObject));
            var lightPrefab = Resources.Load("ShipEditor/Prefabs/EditorLight", typeof(GameObject));

            // instantiate prefabs into the world and set them to children of the editor
            _gridObject = Instantiate(gridPrefab, transform.position, transform.rotation) as GameObject;
            _gridObject.transform.parent = transform;
            _cursorObject = Instantiate(cursorPrefab, transform.position, transform.rotation) as GameObject;
            _cursorObject.transform.parent = _gridObject.transform;
            _lightObject = Instantiate(lightPrefab, transform.position, transform.rotation) as GameObject;
            _lightObject.transform.parent = transform;

            // save the material used in the grid for future usage
            _gridMaterial = _gridObject.GetComponent<MeshRenderer>().material;
        }

        private void UpdateGrid() {
            // calculate values
            _cursorPosition = CalcCursorPosition();
            var cursorNodePosition = _cursorPosition.Round();
            _cursorOffset = _cursorPosition - cursorNodePosition; // delta from cursor and nearest node
            _cursorNode = (GridVector)cursorNodePosition; // node currently nearest to

            //move the grid and its texture
            _gridObject.transform.localPosition = cursorNodePosition;
            _gridMaterial.mainTextureOffset = new Vector2(_cursorOffset.x / 10, _cursorOffset.z / 10);

            // move the light
            _lightObject.transform.localPosition = _cursorPosition;
        }
    }
}