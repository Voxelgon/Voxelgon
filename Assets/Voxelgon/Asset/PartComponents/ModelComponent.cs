using UnityEngine;
using System.Text;

namespace Voxelgon.Asset {
    public class ModelComponent : PartComponent {

        // CONSTRUCTOR

        public ModelComponent() {
            Shader = "standard";
        }


        // PROPERTIES

        public string Path {get; set;}
        public string Shader {get; set;}


        // METHODS

        //IPartComponent
        public GameObject Create() {
            var gameObject = new GameObject();

            gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshRenderer>();

            return gameObject;
        }

        //IPartComponent
        public override string ToString() {
            var builder = new StringBuilder();

            builder.AppendLine("# Model #");
            builder.Append(base.ToString());

            builder.AppendLine("Path: " + Path);
            builder.AppendLine("Shader: " + Shader);

            return builder.ToString();
        }
    }
}