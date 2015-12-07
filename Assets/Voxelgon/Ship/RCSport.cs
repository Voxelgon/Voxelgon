using UnityEngine;

namespace Voxelgon.Ship
{
    public class RCSport : MonoBehaviour {

        public float Thrust;
        public float MaxThrust = 2;

        private int _linMultiplyer = 0; //1 through -1 (for inverting axes)
        private int _latMultiplyer = 0;
        private int _yawMultiplyer = 0;

        private GameObject _ship;
        private ShipManager _shipManager;
        private Rigidbody _shipRigidbody;

        private Vector3 _centerOfMass;
        private Vector3 _offsetVector;
        private Vector3 _forceVector;

        private Vector3 _torqueVector;

        public RCSport()
        {
            _yawMultiplyer = 0;
            _latMultiplyer = 0;
            _linMultiplyer = 0;
        }

        public void Start() {
            _ship = transform.parent.gameObject;
            _shipManager = _ship.GetComponent<ShipManager>();
            _shipRigidbody = _ship.GetComponent<Rigidbody>();

            _torqueVector = Vector3.Cross(
                transform.position - _shipRigidbody.worldCenterOfMass, 
                transform.forward);

        }
    }
}
