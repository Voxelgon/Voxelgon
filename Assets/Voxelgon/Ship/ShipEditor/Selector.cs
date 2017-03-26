using UnityEngine;

namespace Voxelgon.Ship.Editor {

    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Renderer))]
    public class Selector : MonoBehaviour {

        // FIELDS

        private Renderer _renderer;

        [SerializeField]
        private ISelectable _selectable;

        [SerializeField]
        private ShipEditor _editor;

        [SerializeField]
        private bool cursor = false;

        // METHODS

        public void Start() {
            _renderer = GetComponent<Renderer>();
            _renderer.enabled = false;
        }

        public void OnMouseEnter() {
            _renderer.enabled = true;
        }

        public void OnMouseExit() {
            _renderer.enabled = false;
        }
    }
}
