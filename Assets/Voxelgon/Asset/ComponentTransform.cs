using UnityEngine;
using System.Text;

namespace Voxelgon.Asset {
    public class ComponentTransform {

        //CONSTRUCTORS

        public ComponentTransform() {
            Position = Vector3.zero;
            Rotation = Vector3.zero;
            Scale = Vector3.one;
        }


        //PROPERTIES

        public ComponentVector Position {get; set;}
        public ComponentVector Rotation {get; set;}
        public ComponentVector Scale {get; set;}


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