using UnityEngine;
using System.Text;

namespace Voxelgon.Asset {
    public class YamlTransform {

        //CONSTRUCTORS

        public YamlTransform() {
            Position = Vector3.zero;
            Rotation = Vector3.zero;
            Scale = Vector3.one;
        }


        //PROPERTIES

        public YamlVector Position {get; set;}
        public YamlVector Rotation {get; set;}
        public YamlVector Scale {get; set;}


        //METHODS

        public string ToString() {
            var builder = new StringBuilder();

            builder.AppendLine("Position: " + Position);
            builder.AppendLine("Rotation: " + Rotation);
            builder.Append("Scale: " + Scale);

            return builder.ToString();
        }
    }
}