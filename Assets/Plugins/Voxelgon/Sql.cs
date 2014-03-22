using UnityEngine;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Mono.Data.SqliteClient;

namespace Voxelgon {

    public static class Sql {

        public static string connection = "URI=file:Voxelgon.db";

        public static IDbConnection dbcon = (IDbConnection) new SqliteConnection(connection);
        public static IDbCommand dbcmd;

        public static Regex commentFix = new Regex ("#");


        ///runs the given query and returns the reader///
        public static IDataReader Query(string query) {
            dbcon.Open();
            dbcmd = dbcon.CreateCommand();
            dbcmd.CommandText = query;

            return dbcmd.ExecuteReader();
        }

        ///cleans up SQL command///
        public static void Cleanup(){
            dbcmd.Dispose();
            dbcmd = null;
        }

        public static void RunFile(string path) {
            StreamReader sr = new StreamReader(path);
            string contents;

            //Read file ***Move this to another function later, I do this a LOT***
            try {
                contents = sr.ReadToEnd();
            } catch {
                Debug.LogError(string.Format("[SQL] Could not load file at path: {0}", path));
                return;
            } finally {
                sr.Close();
            }

            Debug.Log(string.Format("[SQL] Loading SQL query {0} \n full path: {1}", Asset.Filename(path), path));

            try {
                Query(contents);
            } catch {
                Debug.LogError(string.Format("[SQL] Error running SQL query in {0} \n full path: {1}", Asset.Filename(path), path));
            } finally {
                Debug.Log(string.Format("[SQL] Success! \n loaded '{0}' at path '{1}'", Asset.Filename(path), path));
            }
        }
    }
}
