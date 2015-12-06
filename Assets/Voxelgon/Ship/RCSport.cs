using UnityEngine;

namespace Assets.Voxelgon.Ship
{
    public class RcSport : MonoBehaviour {

        public float Thrust;
        public float MaxThrust = 2;
        
        //ToDo: Create a custom type that wont allow inproper values for these
        private int _linMultiplyer; //1 through -1 (for inverting axes)
        private int _latMultiplyer;
        private int _yawMultiplyer;

        private GameObject _ship;
        private ShipManager _shipManager;
        private Rigidbody _shipRigidbody;

        private Vector3 _centerOfMass;
        private Vector3 _offsetVector;
        private Vector3 _forceVector;

        private Vector3 _torqueVector;

        public RcSport()
        {
            _linMultiplyer = 0;
            _latMultiplyer = 0;
            _yawMultiplyer = 0;
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
