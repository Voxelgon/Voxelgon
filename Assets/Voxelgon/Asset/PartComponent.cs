using System.Text;

namespace Voxelgon.Asset {
    public class PartComponent {

        //CONSTRUCTOR

        public PartComponent() {
            ID = "no_ID";
            Transform = new YamlTransform();
        }


        //PROPERTIES

        public string ID {get; set;}
        public YamlTransform Transform {get; set;}


        //METHODS

        public virtual string ToString() {
            var builder = new StringBuilder();

            builder.AppendLine("ID: " + ID);
            builder.AppendLine("Transform: ");
            builder.AppendLine("  " + Transform.ToString().Replace("\n", "\n  "));

            return builder.ToString();
        }
    }
}