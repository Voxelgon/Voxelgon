using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

using SimpleJSON;

namespace Voxelgon{

    public class Asset{


        //STATIC VARIABLES//

        static string resourcePath;


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

            foreach(string file in filesRaw){
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
            foreach (string path in FilesUnderDirectory(resourcePath)) {
                Debug.Log(path);
            }
        }
    }

    public class Material : Asset {

        public readonly float strength = 2.0;

        public readonly float sheilding = 1.0;
        public readonly float radiation = 0.0;

        public readonly bool isMagnetic = false;
        public readonly bool isOrganic = false;
        public readonly bool isConductive = false;

        public Dictionary<Element, float> makeup;
    }

    public class Item : Asset {
        //item in inventory or elsewhere
        public readonly float density;
    }

    public class Ingot : Item {
        //special item that represents a material
        public readonly Material material;
    }

    public class Element : Asset {

        public readonly int number;
        public readonly float mass;
        public readonly string symbol;

        public readonly types type;
        public readonly states state;

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
