using System.Text;

namespace Voxelgon.Asset {
    public class Asset {

        //CONSTRUCTOR

        public Asset() {
            Namespace = "no_Namespace";
            ID = "no_ID";
        }


        //PROPERTIES

        public string Path {get; set;}
        public string Namespace {get; set;}
        public string ID {get; set;}

        public string GlobalID {
            get {
                return Namespace + "_" + ID;
            }
        }


        //METHODS

        public virtual string ToString() {
            var builder = new StringBuilder();

            builder.AppendLine("Path: " + Path);
            builder.AppendLine("Namespace: " + Namespace);
            builder.AppendLine("Global ID: " + GlobalID);

            return builder.ToString();
        }
    }
}