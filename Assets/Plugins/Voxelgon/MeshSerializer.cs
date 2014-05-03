using UnityEngine;
using System.IO;
namespace Voxelgon {
    public static class MeshSerializer {
//TODO fix all useage of var, fix all bracketing
        public static Mesh ReadMesh( byte[] bytes ) {
            if(bytes.Length < 5 ) {
                Debug.Log( "Invalid mesh file!" );
                return null;
            }

            var buf = new BinaryReader( new MemoryStream( bytes ) );

            // read header
            var vertCount = buf.ReadUInt16();
            var triCount = buf.ReadUInt16();
            byte format = buf.ReadByte();

            // sanity check
            if (vertCount < 0 || vertCount > 64000) {
                Debug.Log("Invalid vertex count in the mesh data!");
                return null;
            }
            if (triCount < 0 || triCount > 64000) {
                Debug.Log("Invalid triangle count in the mesh data!");
                return null;
            }
            if (format < 1 || (format&1) == 0 || format > 15) {
                Debug.Log("Invalid vertex format in the mesh data!");
                return null;
            }

            var mesh = new Mesh();
            int i;

            // positions
            Vector3[] verts = new Vector3[vertCount];
            ReadVector3Array16bit (verts, buf);
            mesh.vertices = verts;

            if((format & 0x2) == 0x2) { // have normals
                Vector3[] normals = new Vector3[vertCount];
                ReadVector3ArrayBytes (normals, buf);
                mesh.normals = normals;
            }

            if((format & 0x4) == 0x4) { // have tangents
                Vector4[] tangents = new Vector4[vertCount];
                ReadVector4ArrayBytes (tangents, buf);
                mesh.tangents = tangents;
            }

            if((format & 0x8) == 0x8) { // have UVs
                Vector2[] uvs = new Vector2[vertCount];
                ReadVector2Array16bit (uvs, buf);
                mesh.uv = uvs;
            }

            // triangle indices
            int[] tris = new int[triCount * 3];
            for( i = 0; i < triCount; ++i ) {
                tris[i*3+0] = buf.ReadUInt16();
                tris[i*3+1] = buf.ReadUInt16();
                tris[i*3+2] = buf.ReadUInt16();
            }
            mesh.triangles = tris;

            buf.Close();

            return mesh;
        }

        public static void ReadVector3Array16bit (Vector3[] arr, BinaryReader buf) {
            int n = arr.Length;
            if (n == 0)
                return;

            // Read bounding box
            Vector3 bmin;
            Vector3 bmax;

            bmin.x = buf.ReadSingle();
            bmax.x = buf.ReadSingle();
            bmin.y = buf.ReadSingle();
            bmax.y = buf.ReadSingle();
            bmin.z = buf.ReadSingle();
            bmax.z = buf.ReadSingle();

            // Decode vectors as 16 bit integer components between the bounds
            for (int i = 0; i < n; i++)  {
                System.UInt16 ix= buf.ReadUInt16 ();
                System.UInt16 iy = buf.ReadUInt16 ();
                System.UInt16 iz = buf.ReadUInt16 ();
                double xx = ix / 65535.0 * (bmax.x - bmin.x) + bmin.x;
                double yy = iy / 65535.0 * (bmax.y - bmin.y) + bmin.y;
                double zz = iz / 65535.0 * (bmax.z - bmin.z) + bmin.z;
                arr[i] = new Vector3((float) xx,(float) yy,(float) zz);
            }
        }

        public static void WriteVector3Array16bit (Vector3[] arr, BinaryWriter buf) {
            if (arr.Length == 0)
                return;

            // Calculate bounding box of the array
            Bounds bounds = new Bounds(arr[0], new Vector3(0.001f, 0.001f, 0.001f));
            foreach (Vector3 v in arr)
                bounds.Encapsulate (v);

            // Write bounds to stream
            Vector3 bmin = bounds.min;
            Vector3 bmax = bounds.max;
            buf.Write (bmin.x);
            buf.Write (bmax.x);
            buf.Write (bmin.y);
            buf.Write (bmax.y);
            buf.Write (bmin.z);
            buf.Write (bmax.z);

            // Encode vectors as 16 bit integer components between the bounds
            foreach(Vector3 v in arr)  {
                float xx = Mathf.Clamp ((float) ((v.x - bmin.x) / (bmax.x - bmin.x) * 65535.0), 0.0f, 65535.0f);
                float yy = Mathf.Clamp ((float) ((v.y - bmin.y) / (bmax.y - bmin.y) * 65535.0), 0.0f, 65535.0f);
                float zz = Mathf.Clamp ((float) ((v.z - bmin.z) / (bmax.z - bmin.z) * 65535.0), 0.0f, 65535.0f);
                System.UInt16 ix = (System.UInt16) xx;
                System.UInt16 iy = (System.UInt16) yy;
                System.UInt16 iz = (System.UInt16) zz;
                buf.Write (ix);
                buf.Write (iy);
                buf.Write (iz);
            }
        }


        public static void ReadVector2Array16bit (Vector2[] arr, BinaryReader buf) {
            int n = arr.Length;
            if (n == 0)
                return;

            // Read bounding box
            Vector2 bmin;
            Vector2 bmax;
            bmin.x = buf.ReadSingle ();
            bmax.x = buf.ReadSingle ();
            bmin.y = buf.ReadSingle ();
            bmax.y = buf.ReadSingle ();

            // Decode vectors as 16 bit integer components between the bounds
            for (int i = 0; i < n; i++)  {
                System.UInt16 ix = buf.ReadUInt16 ();
                System.UInt16 iy = buf.ReadUInt16 ();
                float xx = (float) (ix / 65535.0 * (bmax.x - bmin.x) + bmin.x);
                float yy = (float) (iy / 65535.0 * (bmax.y - bmin.y) + bmin.y);
                arr[i] = new Vector2(xx,yy);
            }
        }

        public static void WriteVector2Array16bit (Vector2[] arr, BinaryWriter buf) {
            if (arr.Length == 0)
                return;

            // Calculate bounding box of the array
            Vector2 bmin = arr[0] - new Vector2(0.001f,0.001f);
            Vector2 bmax = arr[0] + new Vector2(0.001f,0.001f);
            foreach (var v in arr)  {
                bmin.x = Mathf.Min (bmin.x, v.x);
                bmin.y = Mathf.Min (bmin.y, v.y);
                bmax.x = Mathf.Max (bmax.x, v.x);
                bmax.y = Mathf.Max (bmax.y, v.y);
            }

            // Write bounds to stream
            buf.Write (bmin.x);
            buf.Write (bmax.x);
            buf.Write (bmin.y);
            buf.Write (bmax.y);

            // Encode vectors as 16 bit integer components between the bounds
            foreach (Vector2 v in arr)  {
                float xx = (float) ((v.x - bmin.x) / (bmax.x - bmin.x) * 65535.0);
                float yy = (float) ((v.y - bmin.y) / (bmax.y - bmin.y) * 65535.0);
                System.UInt16 ix = (System.UInt16) xx;
                System.UInt16 iy = (System.UInt16) yy;
                buf.Write (ix);
                buf.Write (iy);
            }
        }

        public static void ReadVector3ArrayBytes (Vector3[] arr, BinaryReader buf) {
           // Decode vectors as 8 bit integers components in -1.0 .. 1.0 range
            int n = arr.Length;
            for (var i = 0; i < n; ++i)  {
                byte ix = buf.ReadByte ();
                byte iy = buf.ReadByte ();
                byte iz = buf.ReadByte ();
                float xx = (float) ((ix - 128.0) / 127.0);
                float yy = (float) ((iy - 128.0) / 127.0);
                float zz = (float) ((iz - 128.0) / 127.0);
                arr[i] = new Vector3(xx,yy,zz);
            }
        }

        public static void WriteVector3ArrayBytes (Vector3[] arr, BinaryWriter buf) {
            // Encode vectors as 8 bit integers components in -1.0 .. 1.0 range
            foreach (Vector3 v in arr)  {
                byte ix = (byte) Mathf.Clamp ((float)(v.x * 127.0 + 128.0), 0.0f, 255.0f);
                byte iy = (byte) Mathf.Clamp ((float)(v.y * 127.0 + 128.0), 0.0f, 255.0f);
                byte iz = (byte) Mathf.Clamp ((float)(v.z * 127.0 + 128.0), 0.0f, 255.0f);
                buf.Write (ix);
                buf.Write (iy);
                buf.Write (iz);
            }
        }

        public static void ReadVector4ArrayBytes (Vector4[] arr, BinaryReader buf) {
            // Decode vectors as 8 bit integers components in -1.0 .. 1.0 range
            var n = arr.Length;
            for (int i = 0; i < n; ++i)  {
                byte ix = buf.ReadByte ();
                byte iy = buf.ReadByte ();
                byte iz = buf.ReadByte ();
                byte iw = buf.ReadByte ();
                float xx = (float) ((ix - 128.0) / 127.0);
                float yy = (float) ((iy - 128.0) / 127.0);
                float zz = (float) ((iz - 128.0) / 127.0);
                float ww = (float) ((iw - 128.0) / 127.0);
                arr[i] = new Vector4(xx,yy,zz,ww);
            }
        }

        public static void WriteVector4ArrayBytes (Vector4[] arr, BinaryWriter buf) {
            // Encode vectors as 8 bit integers components in -1.0 .. 1.0 range
            foreach (Vector4 v in arr)  {
                byte ix = (byte) Mathf.Clamp ((float) (v.x * 127.0 + 128.0), 0.0f, 255.0f);
                byte iy = (byte) Mathf.Clamp ((float) (v.y * 127.0 + 128.0), 0.0f, 255.0f);
                byte iz = (byte) Mathf.Clamp ((float) (v.z * 127.0 + 128.0), 0.0f, 255.0f);
                byte iw = (byte) Mathf.Clamp ((float) (v.w * 127.0 + 128.0), 0.0f, 255.0f);
                buf.Write (ix);
                buf.Write (iy);
                buf.Write (iz);
                buf.Write (iw);
            }
        }

        // Writes mesh to an array of bytes.
        public static byte[] WriteMesh(Mesh mesh, bool saveTangents) {
            if( !mesh ) {
                Debug.Log( "No mesh given!" );
                return null;
            }

            var verts = mesh.vertices;
            var normals = mesh.normals;
            var tangents = mesh.tangents;
            var uvs = mesh.uv;    
            var tris = mesh.triangles;

            // figure out vertex format
            byte format = 1;
            if( normals.Length > 0 )
                format |= 2;
            if( saveTangents && tangents.Length > 0 )
                format |= 4;
            if( uvs.Length > 0 )
                format |= 8;

            var stream = new MemoryStream();
            var buf = new BinaryWriter( stream );

            // write header
            System.UInt16 vertCount = (System.UInt16) verts.Length;
            System.UInt16 triCount = (System.UInt16) (tris.Length / 3);
            buf.Write( vertCount );
            buf.Write( triCount );
            buf.Write( format );
            // vertex components
            WriteVector3Array16bit (verts, buf);
            WriteVector3ArrayBytes (normals, buf);
            if (saveTangents)
                WriteVector4ArrayBytes (tangents, buf);
            WriteVector2Array16bit (uvs, buf);
            // triangle indices
            foreach(int idx in tris )  {
                System.UInt16 idx16 = (System.UInt16) idx;
                buf.Write( idx16 );
            }
            buf.Close();

            return stream.ToArray();
        }


        // Writes mesh to a local file, for loading with WWW interface later.
        public static void WriteMeshToFileForWeb(Mesh mesh, string name, bool saveTangents) {
            // Write mesh to regular bytes
            var bytes = WriteMesh( mesh, saveTangents );

            // Write to file
            var fs = new FileStream( name, FileMode.Create );
            fs.Write( bytes, 0, bytes.Length );
            fs.Close();
        }


        // Reads mesh from the given WWW (that is finished downloading already)
        public static Mesh ReadMeshFromWWW(WWW download) {
            if (download.error != null)  {
                Debug.Log("Error downloading mesh: " + download.error);
                return null;
            }

            if (!download.isDone)  {
                Debug.Log("Download must be finished already");
                return null;
            }

            var bytes = download.bytes;

            // Read and return the mesh from regular bytes.
            return ReadMesh( bytes );
        }
    }
}
