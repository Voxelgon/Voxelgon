using System.Collections.Generic;
using System.Text;

namespace Voxelgon.Asset {
    public class Part  : Asset{

        // CONSTRUCTOR

        public Part() {
            Name = "Unnamed Part";
            Description = "No Description";
            Components = new List<PartComponent>();
        }


        // PROPERTIES

        public string Name {get; set;}
        public string Description {get; set;}

        public List<PartComponent> Components {get; set;}

        //METHODS

        public override string ToString() {
            var builder = new StringBuilder();

            builder.AppendLine("# Part #");
            builder.Append(base.ToString());

            builder.AppendLine("Name: " + Name);
            builder.AppendLine("Components:");
            foreach (PartComponent c in Components) {
                builder.AppendLine("  " + c.ToString().Replace("\n", "\n  "));
                builder.AppendLine();
            }

            return builder.ToString();
        }
    }
}