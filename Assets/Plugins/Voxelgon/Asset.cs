using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using UnitySQL;

namespace Voxelgon{

    public class Asset{
        public string name;

        //STATIC VARIABLES//

        static string resourcePath;
        static string innerResourcePath;

        static string schemaPath;

        private static int _elementCount;
        private static int _materialCount;
        private static int _makeupCount;
        private static int _totalCount;

        private static string _indent = "        ";

        private static string[] ignoredFiles= new string[] {
            ".[Dd][Ss]_[Ss]tore$",
            ".[Mm]eta$",
            ".[Ss]wp$"
        };


        //STATIC FUNCTIONS//

        //logs a message tagged with [Assets]//
        private static void Log(string text) {
            Debug.Log("[Assets] " + text);
        }


        //returns the filename of path//
        public static string Filename(string path) {

            return Path.GetFileName(path);
        }


        //returns the extension of path//
        public static string Extension(string path) {

            return Path.GetExtension(path);
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


        //returns the parent (/..) directory of the given path//
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

        static private void LoadAsset(string path) {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("@path", path);
            SQLite.RunFile(path, parameters);
        }


        //Sets up database for assets//
        static public void Setup() {
            resourcePath = Parent(Application.dataPath) + "/Resources";
            innerResourcePath = Application.dataPath + "/Resources";

            SQLite.SetDbName("Voxelgon");
            SQLite.Setup();
        }

        //imports assets (all testing code right now)//
        static public void Load() {

            Log("Loading Assets...");

            SQLite.RunFile(innerResourcePath + "/Schema.sql");
            LoadAsset(innerResourcePath + "/Voxelgon.sql");

            List<string> files = FilesUnderDirectory(resourcePath);

            foreach (string path in files) {
                string sql = "INSERT INTO `resources` (`path`, `filename`, `extension`)\n"; 
                sql = sql + string.Format("VALUES ('{0}', '{1}', '{2}')", path, Filename(path), Extension(path));

                SQLite.Query(sql);
            }

            _elementCount = SQLite.Count("elements", "atomic_number");
            _materialCount = SQLite.Count("materials", "material_id");
            _makeupCount = SQLite.Count("materials_makeup", "makeup_id");

            _totalCount = _elementCount + _materialCount + _makeupCount;

            string counts = "Loaded:\n";
            counts += _indent + _totalCount + " Total assets,\n";
            counts += _indent + _elementCount + " Elements,\n";
            counts += _indent + _materialCount + " Materials,\n";
            counts += _indent + _makeupCount + " Material Makeup Objects,\n";
            Log(counts);
        }


        static public void Import() {
        }
    }
}
