using UnityEngine;
using System.Text;

namespace Voxelgon.Asset {
    public class ModelComponent : PartComponent {

        // CONSTRUCTOR

        public ModelComponent() {
            Shader = "standard";
        }


        // PROPERTIES

        public string ModelPath {get; set;}
        public string Shader {get; set;}


        // METHODS

        public GameObject Create(GameObject parent) {
            var gameObject = base.Create(parent);

            var filter = gameObject.AddComponent<MeshFilter>();
            var renderer = gameObject.AddComponent<MeshRenderer>();

            return gameObject;
        }

        public override string ToString() {
            var builder = new StringBuilder();

            builder.AppendLine("# Model #");
            builder.Append(base.ToString());

            builder.AppendLine("Model Path: " + ModelPath);
            builder.AppendLine("Shader: " + Shader);

            return builder.ToString();
        }
    }
}