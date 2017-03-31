using UnityEngine;
using Voxelgon.Util;
using Voxelgon.Util.Geometry;
using Voxelgon.Ship.Editor;

namespace Voxelgon {

    public class CameraController : MonoBehaviour {

        [HeaderAttribute("Axis Names")]

        public string panKey = "Pan";
        public string orbitKey = "Orbit";
        public string orbitXAxis = "Horizontal";
        public string orbitYAxis = "Vertical";
        public string mouseXAxis = "Mouse X";
        public string mouseYAxis = "Mouse Y";
        public string zoomAxis = "zoom";

        [SpaceAttribute]
        [HeaderAttribute("Settings")]

        [RangeAttribute(10, 50)]
        public float panSensitivity = 20f;
        [RangeAttribute(1, 10)]
        public float panZoomFalloff = 4.0f;
        [RangeAttribute(0.1f, 1.0f)]
        public float orbitSensitivity = 0.8f;
        [RangeAttribute(0, 20)]
        public float orbitZoomFalloff = 10;
        [RangeAttribute(0.5f, 2.0f)]
        public float zoomSensativity = 1f;
        [RangeAttribute(1.0f, 10.0f)]
        public float zoomFalloff = 3;
        [RangeAttribute(0.0f, 1.0f)]
        public float keyboardFalloff = 0.05f;

        [RangeAttribute(0, 1)]
        public float decay = 0.9f;

        public bool inertia;

        public PanMode panMode = PanMode.DragWorld;

        [SpaceAttribute]
        [HeaderAttribute("Limits")]

        public float minZoom = 1;
        public float maxZoom = 100;

        [RangeAttribute(0, 90)]
        public float minElev = 5;
        [RangeAttribute(0, 90)]
        public float maxElev = 85;

        [SpaceAttribute]
        [HeaderAttribute("Current Coordinates")]

        [SerializeField]
        [RangeAttribute(0, 1)]
        private float _zoom = 0.3f;

        [SerializeField]
        [RangeAttribute(0, 1)]
        private float _elev = 0.5f;

        [SerializeField]
        [RangeAttribute(0, 1)]
        private float _azim = 0;

        [SerializeField]
        private Vector3 _gimbal;

        private float _elevKBVel;
        private float _azimKBVel;
        private float _elevKB;
        private float _azimKB;
        private Vector2 _orbVelKB;
        private Vector2 _orbVelKB_2;
        private Vector2 _orbVel;
        private Vector2 _panVel;
        private float _zoomVel;
        private Vector3 _lastCursorPos; //only used in "drag world" mode

        public enum PanMode {
            DragSelf,
            DragWorld
        }

        void Start() {
            _gimbal = Vector3.zero;
        }

        // Update is called once per frame
        void Update() {

            // zoom
            if (inertia) {
                _zoomVel = MathVG.MaxAbs(_zoomVel, Input.GetAxis(zoomAxis));

                if (Mathf.Abs(_zoomVel) > 0.05f) {
                    _zoomVel *= decay;
                } else {
                    _zoomVel = 0;
                }
            } else {
                _zoomVel = Input.GetAxis(zoomAxis);
            }

            var dZoom = zoomSensativity * Time.deltaTime * _zoomVel;
            _zoom = Mathf.Clamp01(_zoom + dZoom);
            var adjustedZoom = MathVG.Smexperp(_zoom, zoomFalloff);
            var zoomValue = Mathf.Lerp(minElev, maxZoom, adjustedZoom);



            // orbit

            if (inertia) {
                _orbVelKB.x = Mathf.SmoothDamp(_orbVelKB.x, Input.GetAxis(orbitXAxis), ref _orbVelKB_2.x, keyboardFalloff);
                _orbVelKB.y = Mathf.SmoothDamp(_orbVelKB.y, Input.GetAxis(orbitYAxis), ref _orbVelKB_2.y, keyboardFalloff);
            } else {
                _orbVelKB.x = Input.GetAxis(orbitXAxis);
                _orbVelKB.y = Input.GetAxis(orbitYAxis);
            }

            if (Input.GetButton(orbitKey)) {
                _orbVel.y = -Input.GetAxis(mouseYAxis);
                _orbVel.x = Input.GetAxis(mouseXAxis);
            } else if (inertia) {
                if (Mathf.Abs(_orbVel.x) > 0.05f
                 || Mathf.Abs(_orbVel.y) > 0.05f) {
                    _orbVel *= decay;
                } else {
                    _orbVel = Vector2.zero;
                }

            } else {
                _orbVel = Vector2.zero;
            }

            var dElev = (orbitSensitivity * Time.deltaTime * MathVG.MaxAbs(_orbVel.y, _orbVelKB.y) * 4);
            var dAzim = (orbitSensitivity * Time.deltaTime * MathVG.MaxAbs(_orbVel.x, _orbVelKB.x));

            dElev /= 1.0f + (adjustedZoom * zoomFalloff);
            dAzim /= 1.0f + (adjustedZoom * zoomFalloff);

            _elev = Mathf.Clamp01(_elev + dElev);
            _azim = Mathf.Repeat(_azim + dAzim, 1);

            // calc actual values
            var elevAngle = Mathf.SmoothStep(minElev, maxElev, _elev);
            var azimAngle = Mathf.Lerp(0, 360, _azim);

            // calc trig values
            var sinElev = Mathf.Sin(elevAngle * Mathf.Deg2Rad);
            var cosElev = Mathf.Cos(elevAngle * Mathf.Deg2Rad);

            var azimMatrix = Matrix2x2.Rotation(azimAngle * Mathf.Deg2Rad);

            // pan
            var cursorPos = Input.mousePosition.xy();
            if (Input.GetButton(panKey)) {
                var worldPos1 = ShipEditor.CalcCursorPosition(cursorPos, _gimbal.y);
                var worldPos2 = ShipEditor.CalcCursorPosition(_lastCursorPos, _gimbal.y);
                if (panMode == PanMode.DragSelf) {
                    _panVel = (worldPos1 - worldPos2).xz();
                } else {
                    _panVel = (worldPos2 - worldPos1).xz();
                }
            } else {
                if (inertia) {
                    if (_panVel.sqrMagnitude > 0.05f) {
                        _panVel *= decay;
                    } else {
                        _panVel = Vector2.zero;
                    }
                } else {
                    _panVel = Vector2.zero;
                }
            }

            _lastCursorPos = cursorPos;
            _gimbal += _panVel.xz();


            // calc polar positions

            transform.localPosition = _gimbal + zoomValue * (Vector3.up * sinElev + (-cosElev * (azimMatrix * Vector2.up)).xz());
            transform.localEulerAngles = new Vector3(elevAngle, azimAngle, 0);
        }
    }
}