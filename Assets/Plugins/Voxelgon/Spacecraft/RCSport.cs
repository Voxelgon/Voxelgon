using UnityEngine;
using System.Collections;
using Voxelgon;

public class RCSport : MonoBehaviour {

    public float thrust;
    public float maxThrust = 2;

    private int linMultiplyer = 0; //1 through -1 (for inverting axes)
    private int latMultiplyer = 0;
    private int yawMultiplyer = 0;

    private GameObject ship;
    private ShipManager shipManager;
    private Rigidbody shipRigidbody;

    private Vector3 centerOfMass;
    private Vector3 relativePos;

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
        ship = transform.parent.gameObject;
        shipManager = ship.GetComponent<ShipManager>();
        shipRigidbody = ship.GetComponent<Rigidbody>();

        centerOfMass = shipRigidbody.centerOfMass;
        relativePos = transform.localPosition - centerOfMass;

    }
}
