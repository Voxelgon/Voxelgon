using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

using LitJson;

namespace Voxelgon{

    public class Asset{
        public string name;

        //STATIC VARIABLES//

        static string resourcePath;

        static List<Element> elements;
        static List<Material> materials;
        static List<Item> items;

       //STATIC FUNCTIONS//

        private static string[] ignoredFiles= new string[] {
            ".[Dd][Ss]_[Ss]tore$",
            ".[Mm]eta$",
            ".[Ss]wp$"
        };

        //returns TRUE if the given path is ignored
        static public bool Ignored(string path){
            foreach (string r in ignoredFiles) {
                if (Regex.Match(path, r).Success) {
                    return true;
                }
            }

            return false;
        }

        //returns the parent (../) directory of the given path
        static public string Parent(string path) {
            string parent = Directory.GetParent(path).FullName;
            return parent;
        }

        //returns a list of files under the given path directory
        static public List<string> GetFiles(string path) {
            List<string> filesRaw = new List<string>(Directory.GetFiles(path));
            List<string> files = new List<string>();

            foreach(string file in filesRaw) {
                if(!Ignored(file)){
                    files.Add(file);
                }
            }

            return files;
        }

        //returns a list of directories under the given directory
        static public List<string> GetDirectories(string path) {
            List<string> dirs = new List<string>(Directory.GetDirectories(path));

            return dirs;
        }

        //returns a list of all files under the given directory in the file tree
        static public List<string> FilesUnderDirectory(string path) {
            List<string> directories = GetDirectories(path);

            List<string> files = GetFiles(path);

            foreach (string dir in directories){
                files.AddRange(FilesUnderDirectory(dir));
            }

            return files;
        }

        //imports assets (all testing code right now)
        static public void Import() {
            resourcePath = Parent(Application.dataPath) + "/Resources";
            Debug.Log(resourcePath);

            elements = new List<Element> ();
            materials = new List<Material> ();
            items = new List<Item> ();

            foreach (string path in FilesUnderDirectory(resourcePath)) {
                ImportAsset(path);
            }
        }

        //inports an asset
        static public void ImportAsset(string path) {

            StreamReader reader = new StreamReader (path);
            string text = reader.ReadToEnd();

            string extension = Path.GetExtension(path);

            if (extension == ".element") {
                Element element = JsonMapper.ToObject<Element>(text);
                Debug.Log(element.symbol);
                elements.Add(element);
            }
        }
    }

    public class Material : Asset {

        public float density = 7.5f;

        public bool ingot = true;

        public float strength = 2.0f;

        public float sheilding = 1.0f;
        public float radiation = 0.0f;

        public bool isMagnetic = false;
        public bool isOrganic = false;
        public bool isConductive = false;

        public Dictionary<Element, float> makeup;
    }

    public class BuildMaterial : Material {

    }

    public class Item : Asset {
        //item in inventory or elsewhere
        public float density;
    }

    public class Ingot : Item {
        //special item that represents a material
        public Material material;
    }

    public class Element : Asset {

        public int number;
        public float mass;
        public string symbol;

        public types type;
        public states state;

        public enum types {
            metal,
            nonmetal,
            semimetal
        }

        public enum states {
            solid,
            liquid,
            gas
        }
    }
}
