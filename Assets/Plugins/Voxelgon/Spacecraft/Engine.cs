using UnityEngine;
using System.Collections;
using Voxelgon;

public class Engine : MonoBehaviour {

    public float thrust;
    public float maxThrust = 2;

    private int linMultiplyer = 0; 
    private int latMultiplyer = 0;
    private int yawMultiplyer = 0;

    private GameObject ship;
    private ShipManager shipManager;
    private Rigidbody shipRigidbody;

    public Vector3 torqueVector;
    public float torqueScalar;

    public void CalculateTorque() {
        ship = transform.parent.gameObject;
        shipManager = ship.GetComponent<ShipManager>();
        shipRigidbody = ship.GetComponent<Rigidbody>();

        torqueVector = Vector3.Cross((transform.position - shipRigidbody.worldCenterOfMass), transform.forward * -1);
        torqueScalar = torqueVector.magnitude;

    }

    public void UpdateEngine() {

    }
}
