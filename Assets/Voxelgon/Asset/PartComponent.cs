using UnityEngine;
using System.Text;

namespace Voxelgon.Asset {
    public class PartComponent {

        //CONSTRUCTOR

        public PartComponent() {
            ID = "no_ID";
            Transform = new YamlTransform();
        }


        //PROPERTIES

        public string YamlPath {get; set;}
        public string ID {get; set;}
        public YamlTransform Transform {get; set;}


        //METHODS

        public virtual GameObject Instantiate(GameObject parent) {
            var gameObject = new GameObject(ID);

            gameObject.transform.parent           = parent.transform; 
            gameObject.transform.localPosition    = Transform.Position;
            gameObject.transform.localEulerAngles = Transform.Rotation;
            gameObject.transform.localScale       = Transform.Scale;

            return gameObject;
        }

        public virtual string ToString() {
            var builder = new StringBuilder();

            builder.AppendLine("ID: " + ID);
            builder.AppendLine("Transform: ");
            builder.AppendLine("  " + Transform.ToString().Replace("\n", "\n  "));

            return builder.ToString();
        }
    }
}