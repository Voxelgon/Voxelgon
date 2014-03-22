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
        static string innerResourcePath;

        static string schemaPath;

      //STATIC FUNCTIONS//

        private static string[] ignoredFiles= new string[] {
            ".[Dd][Ss]_[Ss]tore$",
            ".[Mm]eta$",
            ".[Ss]wp$"
        };

        public static string Filename(string path) {
            return Path.GetFileName(path);
        }

        //returns TRUE if the given path is ignored//
        static public bool Ignored(string path){
            foreach (string r in ignoredFiles) {
                if (Regex.Match(path, r).Success) {
                    return true;
                }
            }

            return false;
        }

        //returns TRUE if the given path matches the given extension//
        static public bool ExtensionMatch(string path, string extension){
            string regex = extension + "$";
            if (Regex.Match(path, regex).Success) {
                 return true;
            }

            return false;
        }

        //returns the parent (../) directory of the given path
        static public string Parent(string path) {
            string parent = Directory.GetParent(path).FullName;
            return parent;
        }

        //returns a list of files under the given path directory//
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

        //returns a list of files with the given extension under the given path directory//
        static public List<string> GetFiles(string path, string extension) {
            List<string> filesRaw = new List<string>(Directory.GetFiles(path));
            List<string> files = new List<string>();

            foreach(string file in filesRaw) {
                if((!Ignored(file)) && (!ExtensionMatch(file, extension))){
                    files.Add(file);
                }
            }

            return files;
        }

        //returns a list of directories under the given directory//
        static public List<string> GetDirectories(string path) {
            List<string> dirs = new List<string>(Directory.GetDirectories(path));

            return dirs;
        }

        //returns a list of all files under the given directory in the file tree//
        static public List<string> FilesUnderDirectory(string path) {
            List<string> directories = GetDirectories(path);

            List<string> files = GetFiles(path);

            foreach (string dir in directories){
                files.AddRange(FilesUnderDirectory(dir));
            }

            return files;
        }


        //imports assets (all testing code right now)//
        static public void Import() {
            resourcePath = Parent(Application.dataPath) + "/Resources";
            innerResourcePath = Application.dataPath + "/Resources";

            Sql.RunFile(innerResourcePath + "/Schema.sql");
            Sql.RunFile(innerResourcePath + "/Voxelgon.sql");

        }

        //imports an asset
        static public void ImportAsset(string path) {

        }
    }
}
