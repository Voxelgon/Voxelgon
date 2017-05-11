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

        private List<Vector2> _testVertices =  new List<Vector2> {
            new Vector2(-1.0f, -5.1f),
            new Vector2(-0.4f, -5.8f),
            new Vector2(0.8f, -6.6f),
            new Vector2(2.3f, -6.7f),
            new Vector2(4.4f, -6.6f),
            new Vector2(5.7f, -5.8f),
            new Vector2(6.7f, -4.9f),
            new Vector2(7.7f, -3.7f),
            new Vector2(8.8f, -2.5f),
            new Vector2(9.7f, -1.9f),
            new Vector2(10.9f, -2.3f),
            new Vector2(11.0f, -4.3f),
            new Vector2(9.7f, -5.9f),
            new Vector2(8.9f, -6.9f),
            new Vector2(6.7f, -6.9f),
            new Vector2(5.4f, -7.6f),
            new Vector2(4.1f, -7.8f),
            new Vector2(2.9f, -8.9f),
            new Vector2(1.9f, -9.7f),
            new Vector2(0.6f, -10.6f),
            new Vector2(-1.0f, -11.5f),
            new Vector2(-3.1f, -11.5f),
            new Vector2(-5.4f, -11.4f),
            new Vector2(-6.2f, -10.3f),
            new Vector2(-5.3f, -9.5f),
            new Vector2(-3.9f, -9.3f),
            new Vector2(-2.1f, -8.6f),
            new Vector2(-1.4f, -7.7f),
            new Vector2(-1.1f, -6.9f),
            new Vector2(-1.5f, -5.9f),
            new Vector2(-3.1f, -6.1f),
            new Vector2(-4.6f, -6.3f),
            new Vector2(-6.2f, -6.4f),
            new Vector2(-7.3f, -6.5f),
            new Vector2(-8.0f, -6.1f),
            new Vector2(-8.3f, -5.6f),
            new Vector2(-8.3f, -5.0f),
            new Vector2(-7.7f, -4.6f),
            new Vector2(-6.4f, -4.5f),
            new Vector2(-5.0f, -4.2f),
            new Vector2(-3.5f, -3.4f),
            new Vector2(-2.9f, -2.7f),
            new Vector2(-3.0f, -1.8f),
            new Vector2(-3.5f, -1.3f),
            new Vector2(-4.9f, -1.0f),
            new Vector2(-6.2f, -1.0f),
            new Vector2(-7.3f, -1.1f),
            new Vector2(-8.2f, -1.2f),
            new Vector2(-8.9f, -1.4f),
            new Vector2(-9.8f, -0.9f),
            new Vector2(-9.7f, 0.0f),
            new Vector2(-9.5f, 1.1f),
            new Vector2(-8.2f, 0.2f),
            new Vector2(-7.5f, 1.4f),
            new Vector2(-5.7f, 0.3f),
            new Vector2(-6.1f, 1.2f),
            new Vector2(-3.2f, 1.8f),
            new Vector2(-3.3f, 2.7f),
            new Vector2(-2.4f, 2.9f),
            new Vector2(-1.9f, 4.0f),
            new Vector2(-0.4f, 2.5f),
            new Vector2(0.5f, 2.9f),
            new Vector2(1.1f, 1.7f),
            new Vector2(1.9f, 1.9f),
            new Vector2(1.8f, 0.2f),
            new Vector2(3.0f, 0.1f),
            new Vector2(2.1f, -1.2f),
            new Vector2(3.4f, -1.6f),
            new Vector2(1.9f, -2.3f),
            new Vector2(3.1f, -3.3f),
            new Vector2(1.7f, -3.5f),
            new Vector2(1.7f, -4.6f),
            new Vector2(0.7f, -4.0f),
            new Vector2(-0.3f, -4.0f)
        };

        // grid stuff
        [SerializeField]
        private int _gridRange;
        [SerializeField]
        [RangeAttribute(0, 1)]
        private float _gridSize;
        [SerializeField]
        private Mesh _gridMesh;
        [SerializeField]
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

        public void Update() {
            //p1.Draw();
            for (var i = 0; i < _testVertices.Count; i++) {
                Debug.DrawLine(_testVertices[i].xz(), _testVertices[(i + 1) % _testVertices.Count].xz(), Color.blue, 0, true);
            }
            var tris = Triangulation.Triangulate(_testVertices).ToList();
            for (var i = 0; i < tris.Count; i += 3) {
                Debug.DrawLine(_testVertices[tris[i]].xz(), _testVertices[tris[i + 1]].xz(), Color.red, 0, true);
                Debug.DrawLine(_testVertices[tris[i + 1]].xz(), _testVertices[tris[i + 2]].xz(), Color.red, 0, true);
                Debug.DrawLine(_testVertices[tris[i + 2]].xz(), _testVertices[tris[i]].xz(), Color.red, 0, true);
            }
            DrawGrid();

            if (Input.GetButtonDown("ChangeFloor")) {
                transform.Translate(Vector3.up * 2 * (int)Input.GetAxis("ChangeFloor"));
            }

            if (Input.GetButtonDown("Mouse0")) {
                _testVertices.Add(CalcCursorPosition(transform.position.y).xz());
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
                    var nodeScale = _gridSize * Mathf.Clamp01(1 - Vector3.SqrMagnitude(nodePos - _cursorPosition) / (_gridRange * _gridRange));
                    matrices[index] = transform.localToWorldMatrix * Matrix4x4.TRS(nodePos, Quaternion.identity, Vector3.one * nodeScale);
                    index++;
                }
            }

            UnityEngine.Graphics.DrawMeshInstanced(_gridMesh, 0, _gridMaterial, matrices, matrices.Length, new MaterialPropertyBlock(), UnityEngine.Rendering.ShadowCastingMode.Off, false, gameObject.layer, Camera.main);
        }
    }
}