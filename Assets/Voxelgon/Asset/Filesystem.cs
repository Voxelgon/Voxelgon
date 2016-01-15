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

        private static readonly string[] _ignoredFiles= {
            ".[Dd][Ss]_[Ss]tore$",
            ".[Mm]eta$",
            ".[Ss]wp$"
        };

        private static readonly Dictionary<string, Filetype> _extensions = new Dictionary<string, Filetype> {
            {".obj", Filetype.Mesh},
            {".yml", Filetype.YAML},
            {".yaml", Filetype.YAML}
        };

        private static readonly Dictionary<string, System.Type> _yamlTypes = new Dictionary<string, System.Type> {
            {"part", typeof(Part)},
            {"model", typeof(ModelComponent)},
            {"sphere_collider", typeof(SphereColliderComponent)}
        };

        private static readonly Deserializer _yamlDeserializer = new Deserializer(ignoreUnmatched: true);


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
        public static bool Ignored(string path){

            foreach (string r in _ignoredFiles) {
                if (Regex.Match(path, r).Success) {
                    return true;
                }
            }

            return false;
        }


        // Returns a list of files under the given path directory
        public static List<string> GetFiles(string path, string extension = null) {

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
        public static List<string> GetFiles(string path, Filetype filetype) {

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
        public static List<string> GetDirectories(string path) {

            return new List<string>(Directory.GetDirectories(path));
        }


        // Returns a list of all files under the given directory in the file tree
        public static List<string> FilesUnderDirectory(string path, string extension = null) {

            List<string> directories = GetDirectories(path);

            List<string> files = GetFiles(path, extension);

            foreach (string dir in directories){
                files.AddRange(FilesUnderDirectory(dir));
            }

            return files;
        }

        // Returns a list of all files under the given directory in the file tree
        public static List<string> FilesUnderDirectory(string path, Filetype filetype) {

            List<string> directories = GetDirectories(path);

            List<string> files = GetFiles(path, filetype);

            foreach (string dir in directories){
                files.AddRange(FilesUnderDirectory(dir, filetype));
            }

            return files;
        }


        // Sets up database for assets
        public static void Setup() {
            //Populate tag mappings for deserializer
            var baseURI = "tag:yaml.org,2002:";
            foreach (KeyValuePair<string, System.Type> entry in _yamlTypes) {
                _yamlDeserializer.RegisterTagMapping(baseURI + entry.Key, entry.Value);
            }

            resourcePath = Path.GetDirectoryName(Application.dataPath) + "/Resources";
            AssetDatabase.Populate();

        }


        // Imports assets
        public static void Import() {

            Log("Importing Assets...");

            var YAMLPaths = FilesUnderDirectory(resourcePath, Filetype.YAML);
            var imported = new List<Asset>();

            foreach(string s in YAMLPaths) {
                imported.AddRange(ImportYaml(s));
            }

            foreach(Asset a in imported) {
                Log("\n" + a);
            }
        }


        // PRIVATE FUNCTIONS

        // reads a .yaml file and returns the objects
        private static List<Asset> ImportYaml(string path) {
            var imported = new List<Asset>();

            var input = new StreamReader(path);
            var reader = new EventReader(new Parser(input));

            reader.Expect<StreamStart>();

            while(reader.Accept<DocumentStart>()) {
                var asset = _yamlDeserializer.Deserialize<Asset>(reader);
                asset.SetYamlPath(path);
                if (asset.GetType() == typeof(Part)) {
                    var part = (Part) asset;

                    part.Instantiate();
                }

                imported.Add(asset);
            }

            return imported;
        }

        //reads a .obj file and returns a Mesh object
        static private Mesh ImportObj(string path) {
            var mesh = new Mesh();

            var triangles = new List<int>();
            var vertices = new List<Vector3>();
            var uv = new List<Vector2>();
            var normals = new List<Vector3>();
            var facedata = new List<int[]>();

            using (var reader = new StreamReader(path)){
                string line;
                string[] brokenString;

                line = reader.ReadLine();

                while (line != null){
                    line = line.Replace("  "," ");
                    line = line.Trim();

                    brokenString = line.Split(' ');

                    switch (brokenString[0]) {
                        case "v":
                            var vertexVector = new Vector3();
                            vertexVector.x = System.Convert.ToSingle(brokenString[1]);
                            vertexVector.y = System.Convert.ToSingle(brokenString[2]);
                            vertexVector.z = System.Convert.ToSingle(brokenString[3]);

                            vertices.Add(vertexVector);
                            break;

                        case "vt":
                        case "vt1":
                        case "vt2":
                            var uvVector = new Vector2();
                            uvVector.x = System.Convert.ToSingle(brokenString[1]);
                            uvVector.y = System.Convert.ToSingle(brokenString[2]);

                            uv.Add(uvVector);
                            break;

                        case "vn":
                            var normalVector = new Vector3();
                            normalVector.x = System.Convert.ToSingle(brokenString[1]);
                            normalVector.y = System.Convert.ToSingle(brokenString[2]);
                            normalVector.z = System.Convert.ToSingle(brokenString[3]);

                            normals.Add(normalVector);
                            break;

                        case "f":
                            var face = new List<int>();

                            for (int j = 1; j < brokenString.Length && ("" + brokenString[j]).Length > 0; j++) {
                                var faceDataObject = new int[3];
                                string[] facePolyString;
                                facePolyString = brokenString[j].Split('/');

                                faceDataObject[0] = System.Convert.ToInt32(facePolyString[0]);

                                if (facePolyString.Length > 1){
                                    if (facePolyString[1] != "") {
                                        faceDataObject[1] = System.Convert.ToInt32(facePolyString[1]);
                                    }
                                    if (facePolyString.Length > 2){
                                        faceDataObject[2] = System.Convert.ToInt32(facePolyString[2]);
                                    }
                                }
                                facedata.Add(faceDataObject);
                                face.Add(faceDataObject[0]);
                            }

                            for(int k = 2; k < face.Count; k++) {
                                triangles.Add(face[0]-1);
                                triangles.Add(face[k - 1]-1);
                                triangles.Add(face[k]-1);
                            }

                            break;
                    }
                    line = reader.ReadLine();

                }
            }

            var uvArray = new Vector2[vertices.Count];
            var normalArray = new Vector3[vertices.Count];

            int i = 0;
            foreach (int[] f in facedata) {
                if( f[1] >= 1 ) {
                    uvArray[f[0] - 1] = uv[f[1] - 1];
                }
                if( f[2] >= 1 ) {
                    normalArray[f[0] - 1] = normals[f[2] - 1];
                }
                Debug.Log(f[0]-1);
                Debug.Log(f[1]-1);
                i++;
            }

            mesh.SetVertices(vertices);
            mesh.uv = uvArray;
            mesh.normals = normalArray;
            mesh.triangles = triangles.ToArray();

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.Optimize();

            return mesh;
        }
    }
}
