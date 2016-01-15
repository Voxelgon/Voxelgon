using UnityEngine;
using System.Text;

namespace Voxelgon.Asset {
    public class SphereColliderComponent : PartComponent {

        // CONSTRUCTOR

        public SphereColliderComponent() {
            Radius = 0.5f;
        }


        // PROPERTIES

        public float Radius {get; set;}

        // METHODS

        public override GameObject Instantiate(GameObject parent) {
            var gameObject = base.Instantiate(parent);

            var collider = gameObject.AddComponent<SphereCollider>();
            collider.radius = Radius;

            return gameObject;
        }

        public override string ToString() {
            var builder = new StringBuilder();

            builder.AppendLine("# Sphere Collider #");
            builder.Append(base.ToString());

            builder.Append("Radius: " + Radius);

            return builder.ToString();
        }
    }
}