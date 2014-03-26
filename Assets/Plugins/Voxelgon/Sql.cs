using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Mono.Data.SqliteClient;

namespace Voxelgon {

    public static class Sql {

        public static string connection = "URI=file:Voxelgon.db";

        public static IDbConnection dbcon = (IDbConnection) new SqliteConnection(connection);
        public static IDbCommand dbcmd;


        private static void Log(string text) {
            Debug.Log("[SQL] " + text);
        }


        private static void LogError(string text) {
            Debug.LogError("[SQL] " + text);
        }


        //runs the given query and returns the reader//
        public static IDataReader Query(string query) {
            dbcon.Open();
            dbcmd = dbcon.CreateCommand();
            dbcmd.CommandText = query;

            return dbcmd.ExecuteReader();
        }


        //runs the given query and returns the first column as an array of strings//
        public static string[] QueryArray(string query) {
            IDataReader reader = Query(query);
            List<string> list = new List<string>();

            for(int i = 0; i < reader.FieldCount; i++) {
                reader.Read();
                list.Add(reader.GetString(0));
            }
            return list.ToArray();
        }

        //cleans up after running a SQL query//
        public static void Cleanup(){
            dbcmd.Dispose();
            dbcmd = null;
        }


        //returns the number of rows in the given column//
        public static int Count(string table, string column) {

            string sql = string.Format("SELECT COUNT(`{0}`) c FROM {1}", column, table);
            IDataReader reader = Query(sql);

            reader.Read();
            int count = reader.GetInt32(0);

            return count;
        }


        //Runs the file at `path`//
        public static void RunFile(string path) {
            StreamReader sr = new StreamReader(path);
            string contents;

            //Read file ***Move this to another function later, I do this a LOT***
            try {
                contents = sr.ReadToEnd();
            } catch {
                LogError(string.Format("Could not load file at path: {0}", path));
                return;
            } finally {
                sr.Close();
            }

            Log(string.Format("Loading SQL query {0} \n full path: {1}", Asset.Filename(path), path));

            try {
                Query(contents);
            } catch {
                LogError(string.Format("Error running SQL query in {0} \n full path: {1}", Asset.Filename(path), path));
                return;
            }

            Log(string.Format("Success! \n loaded '{0}' at path '{1}'", Asset.Filename(path), path));
        }


        //Runs the file at `path` with special treatment with parameters to fill in @path//
        public static void RunAsset(string path) {
            StreamReader sr = new StreamReader(path);
            string contents;

            //Read file ***Move this to another function later, I do this a LOT***
            try {
                contents = sr.ReadToEnd();
            } catch {
                LogError(string.Format("Could not load file at path: {0}", path));
                return;
            } finally {
                sr.Close();
            }

            Log(string.Format("Loading SQL query {0} \n full path: {1}", Asset.Filename(path), path));

            try {
                dbcon.Open();
                dbcmd = dbcon.CreateCommand();
                dbcmd.CommandText = contents;
                dbcmd.CommandType = CommandType.Text;

                dbcmd.Parameters.Add(new SqliteParameter("@path", Asset.Parent(path)));

                dbcmd.ExecuteReader();
            } catch {
                LogError(string.Format("Error running SQL query in {0} \n full path: {1}", Asset.Filename(path), path));
                return;
            }

            Log(string.Format("Success! \n loaded '{0}' at path '{1}'", Asset.Filename(path), path));
        }
    }
}
