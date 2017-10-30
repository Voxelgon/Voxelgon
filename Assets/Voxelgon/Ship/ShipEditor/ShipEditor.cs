using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Voxelgon.Geometry;
using Voxelgon.Geometry2D;
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
        [SerializeField] private int _gridRange;

        [SerializeField] [RangeAttribute(0, 1)] private float _gridSize;
        [SerializeField] private Mesh _gridMesh;
        [SerializeField] private Material _gridMaterial;

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

        public void Update() {

            DrawGrid();

            if (Input.GetButtonDown("ChangeFloor")) {
                transform.Translate(Vector3.up * 2 * (int) Input.GetAxis("ChangeFloor"));
            }

            if (Input.GetButtonDown("Mouse0")) {
            }

            if (Input.GetButtonDown("Mouse1")) {
            }
        }


        public static Vector3 CalcCursorPosition(float y) {
            var cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            var xySlope = cursorRay.direction.y / cursorRay.direction.x;
            var zySlope = cursorRay.direction.y / cursorRay.direction.z;

            var deltaY = cursorRay.origin.y - y;

            var xIntercept = cursorRay.origin.x + deltaY / -xySlope;
            var zIntercept = cursorRay.origin.z + deltaY / -zySlope;

            var interceptPoint = new Vector3(xIntercept, y, zIntercept);

            return interceptPoint;
        }

        public static Vector3 CalcCursorPosition(Vector2 screenPoint, float y) {
            var cursorRay = Camera.main.ScreenPointToRay(screenPoint.xy());

            var xySlope = cursorRay.direction.y / cursorRay.direction.x;
            var zySlope = cursorRay.direction.y / cursorRay.direction.z;

            var deltaY = cursorRay.origin.y - y;

            var xIntercept = cursorRay.origin.x + deltaY / -xySlope;
            var zIntercept = cursorRay.origin.z + deltaY / -zySlope;

            var interceptPoint = new Vector3(xIntercept, y, zIntercept);

            return interceptPoint;
        }

        private void DrawGrid() {
            _cursorPosition = CalcCursorPosition(transform.localPosition.y);
            var gridPos = _cursorPosition.Round();

            var matrices = new Matrix4x4[(2 * _gridRange + 1) * (2 * _gridRange + 1)];
            var index = 0;

            for (int x = -_gridRange; x <= _gridRange; x++) {
                for (int y = -_gridRange; y <= _gridRange; y++) {
                    var nodePos = gridPos + new Vector3(x, 0, y);
                    var nodeScale =
                        _gridSize * Mathf.Clamp01(1 - Vector3.SqrMagnitude(nodePos - _cursorPosition) /
                                                  (_gridRange * _gridRange));
                    matrices[index] = transform.localToWorldMatrix *
                                      Matrix4x4.TRS(nodePos, Quaternion.identity, Vector3.one * nodeScale);
                    index++;
                }
            }

            UnityEngine.Graphics.DrawMeshInstanced(_gridMesh, 0, _gridMaterial, matrices, matrices.Length,
                new MaterialPropertyBlock(), UnityEngine.Rendering.ShadowCastingMode.Off, false, gameObject.layer,
                Camera.main);
        }
    }
}