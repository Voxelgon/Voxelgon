using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Voxelgon.Asset{

    public static class Filesystem{

        // STATIC VARIABLES

        static string resourcePath;

        private static string[] ignoredFiles= {
            ".[Dd][Ss]_[Ss]tore$",
            ".[Mm]eta$",
            ".[Ss]wp$"
        };

        public static Dictionary<string, Filetype> _extensions = new Dictionary<string, Filetype> {
            {".obj", Filetype.Mesh},
            {".yml", Filetype.YAML},
            {".yaml", Filetype.YAML}
        };


        // ENUMERATORS

        public enum Filetype {
            Other,
            YAML,
            Mesh,
            Code

        }


        // STATIC FUNCTIONS

        // logs a message tagged with [Assets]
        private static void Log(string text) {
            Debug.Log("[Assets] " + text);
        }


        // Returns TRUE if the given path is ignored
        static public bool Ignored(string path){

            foreach (string r in ignoredFiles) {
                if (Regex.Match(path, r).Success) {
                    return true;
                }
            }

            return false;
        }


        // Returns a list of files under the given path directory
        static public List<string> GetFiles(string path, string extension = null) {

            var filesRaw = new List<string>(Directory.GetFiles(path));
            var files = new List<string>();

            foreach(string file in filesRaw) {
                if(!Ignored(file)){
                    if(extension != null) {
                        if(Path.GetExtension(file) == extension) {
                            files.Add(file);
                        }
                    } else {
                        files.Add(file);
                    }
                }
            }

            return files;
        }

        // Returns a list of files under the given path directory
        static public List<string> GetFiles(string path, Filetype filetype) {

            var filesRaw = new List<string>(Directory.GetFiles(path));
            var files = new List<string>();

            foreach(string file in filesRaw) {
                if(!Ignored(file)){
                    string extension = Path.GetExtension(file);
                    if((_extensions.ContainsKey(extension)) && (_extensions[extension] == filetype)) {
                        files.Add(file);
                    }
                }
            }

            return files;
        }


        // Returns a list of directories under the given directory
        static public List<string> GetDirectories(string path) {

            return new List<string>(Directory.GetDirectories(path));
        }


        // Returns a list of all files under the given directory in the file tree
        static public List<string> FilesUnderDirectory(string path, string extension = null) {

            List<string> directories = GetDirectories(path);

            List<string> files = GetFiles(path, extension);

            foreach (string dir in directories){
                files.AddRange(FilesUnderDirectory(dir));
            }

            return files;
        }

        // Returns a list of all files under the given directory in the file tree
        static public List<string> FilesUnderDirectory(string path, Filetype filetype) {

            List<string> directories = GetDirectories(path);

            List<string> files = GetFiles(path, filetype);

            foreach (string dir in directories){
                files.AddRange(FilesUnderDirectory(dir, filetype));
            }

            return files;
        }


        // Sets up database for assets
        static public void Setup() {
            resourcePath = Path.GetDirectoryName(Application.dataPath) + "/Resources";
            AssetDatabase.Populate();

        }


        // Imports assets
        static public void Import() {

            Log("Importing Assets...");

            var YAMLPaths = FilesUnderDirectory(resourcePath, Filetype.YAML);
            var imported = new List<Asset>();

            foreach(string s in YAMLPaths) {
                imported.AddRange(ImportYAML(s));
            }

            foreach(Asset a in imported) {
                Log("\n" + a);
            }
        }


        // PRIVATE FUNCTIONS

        // reads a .yaml file and returns the objects
        static private List<Asset> ImportYAML(string path) {
            var imported = new List<Asset>();

            var input = new StreamReader(path);
            var reader = new EventReader(new Parser(input));
            var deserializer = new Deserializer(ignoreUnmatched: true);
            //So I dont typo it again...
            var baseURI = "tag:yaml.org,2002:";

            deserializer.RegisterTagMapping(baseURI + "part", typeof(Part));
            deserializer.RegisterTagMapping(baseURI + "model", typeof(ModelComponent));
            deserializer.RegisterTagMapping(baseURI + "sphere_collider", typeof(SphereColliderComponent));

            reader.Expect<StreamStart>();

            while(reader.Accept<DocumentStart>()) {
                var asset = deserializer.Deserialize<Asset>(reader);
                asset.SetYamlPath(path);
                if (asset.GetType() == typeof(Part)) {
                    var part = (Part) asset;

                    part.Instantiate();
                }

                imported.Add(asset);
            }

            return imported;

        }


    }
}
