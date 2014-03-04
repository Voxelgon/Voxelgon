using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.IO;

using SimpleJSON;

namespace Voxelgon{

    static class Asset{

        static string resourcePath;


        //returns the parent (../) directory of the given path
        static public string Parent(string path) {
            string parent = Directory.GetParent(path).FullName;
            return parent;
        }

        //returns a list of files under the given path directory
        static public List<string> GetFiles(string path) {
            List<string> files = new List<string>(Directory.GetFiles(path));
            return files;
        }

        //returns a list of directories under the given directory
        static public List<string> GetDirectories(string path) {
            List<string> files = new List<string>(Directory.GetDirectories(path));
            return files;
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
}
