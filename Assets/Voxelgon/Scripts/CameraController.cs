using UnityEngine;
using Voxelgon.Ship.Editor;

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
    [HeaderAttribute("Sensitivity")]

    [RangeAttribute(10, 50)]
    public float panSensitivity = 20f;
    [RangeAttribute(1, 10)]
    public float panZoomFalloff = 4.0f;
    public float orbitSensitivity = 0.4f;
    public float zoomSensativity = 1f;
    public AnimationCurve zoomCurve;
    public AnimationCurve elevCurve;

    [SpaceAttribute]
    [HeaderAttribute("Limits")]


    public float minZoom = 1;
    public float maxZoom = 1000;

    [RangeAttribute(0, 90)]
    public float minElev = 5;

    [RangeAttribute(0, 90)]
    public float maxElev = 85;

    [SpaceAttribute]
    [HeaderAttribute("Current Coordinates")]

    [SerializeField]
    [RangeAttribute(0, 1)]
    private float _zoom = 1;

    [SerializeField]
    [RangeAttribute(0, 1)]
    private float _elev = 0.5f;

    [SerializeField]
    [RangeAttribute(0, 1)]
    private float _azim = 0;

    [SerializeField]
    private Vector3 _gimbal;

    private Matrix4x4 orthoSpace = new Matrix4x4();

    void Start() {
        _gimbal = Vector3.zero;
    }

    // Update is called once per frame
    void Update() {

        // get orbit values
        if (Input.GetButton(orbitKey)) {
            var dElev = 4 * orbitSensitivity * Time.deltaTime * Input.GetAxis(mouseYAxis);
            var dAzim = orbitSensitivity * Time.deltaTime * Input.GetAxis(mouseXAxis);
            _elev = Mathf.Clamp01(_elev + dElev);
            _azim = Mathf.Clamp01(_azim + dAzim);
        }

        var dElevKey = 4 * orbitSensitivity * Time.deltaTime * Input.GetAxis(orbitYAxis);
        var dAzimKey = orbitSensitivity * Time.deltaTime * Input.GetAxis(orbitXAxis);
        _elev = Mathf.Clamp01(_elev + dElevKey);
        _azim = Mathf.Repeat(_azim + dAzimKey, 1);

        var dZoom = zoomSensativity * Time.deltaTime * Input.GetAxis(zoomAxis);
        _zoom = Mathf.Clamp01(_zoom + dZoom);


        // calc actual values
        var zoomValue = Mathf.Lerp(minZoom, maxZoom, zoomCurve.Evaluate(_zoom));
        var elevAngle = Mathf.Lerp(minElev, maxElev, elevCurve.Evaluate(_elev));
        var azimAngle = Mathf.Lerp(0, 360, _azim);

        // calc trig values
        var sinAzim = Mathf.Sin(azimAngle * Mathf.Deg2Rad);
        var cosAzim = Mathf.Cos(azimAngle * Mathf.Deg2Rad);
        var sinElev = Mathf.Sin(elevAngle * Mathf.Deg2Rad);
        var cosElev = Mathf.Cos(elevAngle * Mathf.Deg2Rad);

        // move gimbal

        /*
        // zoom toward cursor
        // doesnt really work?
        if (_zoom > 0.0001) {
            var target = ShipEditor.CalcCursorPosition();
            _gimbal = Vector3.Lerp(_gimbal, target, 1000 * zoomCurve.Evaluate(-dZoom));
        }*/

        if (Input.GetButton(panKey)) {
            var dx = (1 + (_zoom * panZoomFalloff)) * panSensitivity * Time.deltaTime * Input.GetAxis(mouseXAxis);
            var dy = (1 + (_zoom * panZoomFalloff)) * panSensitivity * Time.deltaTime * Input.GetAxis(mouseYAxis);
            _gimbal += new Vector3(dy * sinAzim + dx * cosAzim, 0, -dx * sinAzim + dy * cosAzim);
        }

        // calc polar positions

        transform.localPosition = _gimbal + new Vector3(-cosElev * sinAzim, sinElev, -cosElev * cosAzim) * zoomValue;
        transform.localEulerAngles = new Vector3(elevAngle, azimAngle, 0);

        /*
                //set values of temporary variables
                elevation = transform.parent.eulerAngles.x;
                zoom = -transform.localPosition.z;
                orthoSpace = Matrix4x4.TRS(Vector3.one, Quaternion.Euler(-elevation, 0, 0), Vector3.one);



                //orbit camera (default to MMB)
                if (Input.GetButton("Orbit")) {
                    transform.parent.Rotate(Time.deltaTime * Vector3.up * orbitSensitivity * Input.GetAxis("Mouse X"), Space.World);
                    transform.parent.Rotate(Time.deltaTime * Vector3.right * (Mathf.Clamp(orbitSensitivity * Input.GetAxis("Mouse Y"), minAltitude - elevation, maxAltitude - elevation)));
                }

                transform.parent.Rotate(Time.deltaTime * Vector3.up * orbitSensitivity * Input.GetAxis("Horizontal"), Space.World);
                transform.parent.Rotate(Time.deltaTime * Vector3.right * (Mathf.Clamp(orbitSensitivity * Input.GetAxis("Vertical"), minAltitude - elevation, maxAltitude - elevation)));

                //zoom camera (default to Scroll Wheel)
                transform.Translate(Time.deltaTime * Vector3.back * Mathf.Clamp(
                                        Input.GetAxis("zoom") * zoomSensativity * Mathf.Pow(zoom / maxZoom, zoomExpo),
                                    minZoom - zoom,
                                    maxZoom - zoom));
        */
    }
}
