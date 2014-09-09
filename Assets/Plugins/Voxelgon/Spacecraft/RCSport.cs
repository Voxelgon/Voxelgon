using UnityEngine;
using System.Collections;
using Voxelgon;

public class RCSport : MonoBehaviour {

    public float thrust;
    public float maxThrust = 2;

    private int _linMultiplyer = 0; //1 through -1 (for inverting axes)
    private int _latMultiplyer = 0;
    private int _yawMultiplyer = 0;

    private GameObject _ship;
    private ShipManager _shipManager;
    private Rigidbody _shipRigidbody;

    private Vector3 _centerOfMass;
    private Vector3 _relativePos;

    /*
     *            ^ forward Vector
     *            |
     *            |       \
     *            |    \   \ -Engine
     *            |     \   \
     *            |     /\ \
     *            |    /    \
     *            |a1 /  a2  \ -engine thrust vector
     *            |  /
     *           -|-/
     *          (##) - Ship Center Of Mass
     *           --
     *    a1 = relativeAngle
     *    a2 = thustAngle
     */

    public void Start() {
        _ship = transform.parent.gameObject;
        _shipManager = _ship.GetComponent<ShipManager>();
        _shipRigidbody = _ship.GetComponent<Rigidbody>();

        _centerOfMass = _shipRigidbody.centerOfMass;
        _relativePos = transform.localPosition - _centerOfMass;

    }
}
