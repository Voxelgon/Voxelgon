﻿using UnityEngine;
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
    private Vector3 offsetVector;
    private Vector3 forceVector;

    private Vector3 torqueVector;

    public void Start() {
        ship = transform.parent.gameObject;
        shipManager = ship.GetComponent<ShipManager>();
        shipRigidbody = ship.GetComponent<Rigidbody>();

        torqueVector = Vector3.Cross((transform.position - shipRigidbody.worldCenterOfMass), transform.forward);

    }
}
