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

        private enum Filetype {
            Other,
            Sql,
            Mesh,
            Texture,
            Code

        }

        private static Dictionary<string, Filetype> extensions = new Dictionary<string, Filetype> {
            {".sql", Filetype.Sql},
            {".fbx", Filetype.Mesh},
            {".blend", Filetype.Mesh},
            {".png", Filetype.Texture}
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



        static private void LoadAsset(Dictionary<string, string> properties) {

            string ext = properties["extension"].ToLower();
            string path = properties["path"];


            Filetype filetype;

            if (extensions.ContainsKey(ext)) {
                filetype = extensions[ext];
            } else {
                filetype = Filetype.Other;
            }

            switch (filetype)
            {
                case Filetype.Sql:
                    LoadSQL(path);
                    break;
            }
        }



        //Sets up database for assets//
        static public void Setup() {
            resourcePath = Parent(Application.dataPath) + "/Resources";
            innerResourcePath = Application.dataPath + "/Resources";

            SQLite.SetDbName("Voxelgon");
            SQLite.Setup();
        }



        //imports assets (all testing code right now)//
        static public void Import() {

            //announce Import is beginning in log
            Log("Importing Assets...");

            //import Schema and Voxelgon resources first
            SQLite.RunFile(innerResourcePath + "/Schema.sql", null, false);
            ImportSQL(innerResourcePath + "/Voxelgon.sql");

            string fileListQuery;


            //List the number of each kind of asset imported into the SQL database in log
            elementCount = SQLite.Count("elements", "atomic_number");
            materialCount = SQLite.Count("materials", "material_id");
            makeupCount = SQLite.Count("materials_makeup", "makeup_id");

            totalCount = _elementCount + _materialCount + _makeupCount;

            string counts = "Imported:\n";
            counts += _indent + totalCount + " Total assets,\n";
            counts += _indent + elementCount + " Elements,\n";
            counts += _indent + materialCount + " Materials,\n";
            counts += _indent + makeupCount + " Material Makeup Objects,\n";
            Log(counts);
        }



        static public void Load() {
        }



        //PRIVATE FUNCTIONS

        //runs a .sql file with the appropriate parameters for later loading
        static private void ImportSQL(string path) {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("@path", path);
            SQLite.RunFile(path, parameters, false);
        }



        //reads a .obj file and returns a Mesh object
        static private Mesh ImportMesh(string path) {
            Mesh mesh = new Mesh();

            List<int> triangles = new List<int>();
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();
            List<Vector3> normals = new List<Vector3>();
            List<int[]> facedata = new List<int[]>();

            StreamReader sr = new StreamReader(path);
            string objContents = sr.ReadToEnd();
            sr.Close();

            using (StringReader reader = new StringReader(objContents)){
                string line = "";
                char[] splitID = {' '};
                char[] splitID2 = {'/'};
                string[] brokenString;

                line = reader.ReadLine();

                while (line != null){
                    line = line.Replace("  "," ");
                    line = line.Trim();

                    brokenString = line.Split(splitID, 50);

                    switch (brokenString[0]) {
                        case "v":
                            Vector3 vertexVector = new Vector3();
                            vertexVector.x = System.Convert.ToSingle(brokenString[1]);
                            vertexVector.y = System.Convert.ToSingle(brokenString[2]);
                            vertexVector.z = System.Convert.ToSingle(brokenString[3]);

                            vertices.Add(vertexVector);
                            break;

                        case "vt":
                        case "vt1":
                        case "vt2":
                            Vector2 uvVector = new Vector2();
                            uvVector.x = System.Convert.ToSingle(brokenString[1]);
                            uvVector.y = System.Convert.ToSingle(brokenString[2]);

                            uv.Add(uvVector);
                            break;

                        case "vn":
                            Vector3 normalVector = new Vector3();
                            normalVector.x = System.Convert.ToSingle(brokenString[1]);
                            normalVector.y = System.Convert.ToSingle(brokenString[2]);
                            normalVector.z = System.Convert.ToSingle(brokenString[3]);

                            normals.Add(normalVector);
                            break;

                        case "f":
                            List<int> face = new List<int>();

                            for (int j = 1; j < brokenString.Length && ("" + brokenString[j]).Length > 0; j++) {
                                int[] faceDataObject = new int[3];
                                string[] facePolyString;
                                facePolyString = brokenString[j].Split(splitID2, 3);

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

            Vector3[] vertexArray = vertices.ToArray();
            Vector2[] uvArray = new Vector2[vertexArray.Length];
            Vector3[] normalArray = new Vector3[vertexArray.Length];

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

            mesh.vertices = vertexArray;
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
