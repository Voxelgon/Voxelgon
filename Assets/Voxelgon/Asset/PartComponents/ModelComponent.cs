using UnityEngine;
using System.Text;
using System.IO;

namespace Voxelgon.Asset {
    public class ModelComponent : PartComponent {

        // CONSTRUCTOR

        public ModelComponent() {
            Material = "standard";
        }


        // PROPERTIES

        public string ModelPath {get; set;}
        public string Material {get; set;}

        public string ModelPathFull {
            get {
                return Path.Combine(Path.GetDirectoryName(YamlPath), ModelPath);
            }
        }


        // METHODS

        public override GameObject Instantiate(GameObject parent) {
            var gameObject = base.Instantiate(parent);
            var mesh = AssetDatabase.GetMesh(ModelPathFull);
            var material = AssetDatabase.GetMaterial(Material);

            var filter = gameObject.AddComponent<MeshFilter>();
            var renderer = gameObject.AddComponent<MeshRenderer>();

            filter.mesh = mesh;
            renderer.material = material;

            return gameObject;
        }

        public override string ToString() {
            var builder = new StringBuilder();

            builder.AppendLine("# Model #");
            builder.Append(base.ToString());

            builder.AppendLine("Model Path: " + ModelPathFull);
            builder.AppendLine("Material: " + Material);

            return builder.ToString();
        }
    }
}