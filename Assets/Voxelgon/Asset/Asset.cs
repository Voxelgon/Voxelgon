using System.Text;

namespace Voxelgon.Asset {
    public class Asset {

        //CONSTRUCTOR

        public Asset() {
            Namespace = "no_Namespace";
            ID = "no_ID";
        }


        //PROPERTIES

        public string YamlPath {get; private set;}
        public string Namespace {get; set;}
        public string ID {get; set;}

        public string GlobalID {
            get {
                return Namespace + ":" + ID;
            }
        }


        //METHODS

        public virtual void SetYamlPath(string path) {
            YamlPath = path;
        }

        public override string ToString() {
            var builder = new StringBuilder();

            builder.AppendLine("Path: " + YamlPath);
            builder.AppendLine("Namespace: " + Namespace);
            builder.AppendLine("Global ID: " + GlobalID);

            return builder.ToString();
        }
    }
}