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
        [SerializeField]
        private GameObject _gridObject;
        [SerializeField]
        private GameObject _lightObject;

        private Material _gridMaterial;

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
            _gridMaterial = _gridObject.GetComponent<MeshRenderer>().material;
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


        public static Vector3 CalcCursorPosition() {
            var y = 0;
            var cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            var xySlope = cursorRay.direction.y / cursorRay.direction.x;
            var zySlope = cursorRay.direction.y / cursorRay.direction.z;

            var deltaY = cursorRay.origin.y - y;

            var xIntercept = cursorRay.origin.x + deltaY / -xySlope;
            var zIntercept = cursorRay.origin.z + deltaY / -zySlope;

            var interceptPoint = new Vector3(xIntercept, y, zIntercept);

            return interceptPoint;
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