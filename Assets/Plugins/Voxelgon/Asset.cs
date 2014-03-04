using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.IO;

using SimpleJSON;

namespace Voxelgon{

    static class Asset{

        static string resourcePath;

        static public string Parent(string path) {
            string parent = Directory.GetParent(path).FullName;
            return parent;
        }

        static public List<string> GetFiles(string path) {
            List<string> files = new List<string>(Directory.GetFiles(path));
            return files;
        }

        static public List<string> GetDirectories(string path) {
            List<string> files = new List<string>(Directory.GetDirectories(path));
            return files;
        }

        static public List<string> FilesUnderDirectory(string path) {
            List<string> directories = GetDirectories(path);

            List<string> files = GetFiles(path);

            foreach (string dir in directories){
                files.AddRange(FilesUnderDirectory(dir));
            }

            return files;
        }

        static public void Import() {
            resourcePath = Parent(Application.dataPath) + "/Resources";
            Debug.Log(resourcePath);
            foreach (string path in FilesUnderDirectory(resourcePath)) {
                Debug.Log(path);
            }
        }
    }
}
