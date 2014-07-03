using UnityEngine;
using System.IO;

namespace Voxelgon {
    public static class MeshSerializer {
        public static Mesh ReadMesh( byte[] bytes ) {
            if(bytes.Length < 5 ) {
                Debug.Log( "Invalid mesh file!" );
                return null;
            }

            var buf = new BinaryReader( new MemoryStream( bytes ) );

            // read header
            System.UInt16 vertCount = buf.ReadUInt16();
            System.UInt16 triCount = buf.ReadUInt16();
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

            Mesh mesh = new Mesh();

            //create Vertices
            Vector3[] verts = new Vector3[vertCount];

            // VERTICES
            Vector3 vbmin;
            Vector3 vbmax;

            vbmin.x = buf.ReadSingle();
            vbmax.x = buf.ReadSingle();
            vbmin.y = buf.ReadSingle();
            vbmax.y = buf.ReadSingle();
            vbmin.z = buf.ReadSingle();
            vbmax.z = buf.ReadSingle();

            for (int i = 0; i < verts.Length; ++i)  {
                System.UInt16 ix = buf.ReadUInt16();
                System.UInt16 iy = buf.ReadUInt16();
                System.UInt16 iz = buf.ReadUInt16();
                float xx = (float) (ix / 65535.0 * (vbmax.x - vbmin.x)) + vbmin.x;
                float yy = (float) (iy / 65535.0 * (vbmax.y - vbmin.y)) + vbmin.y;
                float zz = (float) (iz / 65535.0 * (vbmax.z - vbmin.z)) + vbmin.z;
                verts[i] = new Vector3(xx, yy, zz);
            }

            mesh.vertices = verts;

            // CHECK FOR OTHER MESH VALUES
            if((format & 0x2) == 0x2) { // have normals
                Vector3[] normals = new Vector3[vertCount];

                for (var i = 0; i < normals.Length; ++i)  {
                    byte ix = buf.ReadByte();
                    byte iy = buf.ReadByte();
                    byte iz = buf.ReadByte();
                    float xx = (float) ((ix - 128.0) / 127.0);
                    float yy = (float) ((iy - 128.0) / 127.0);
                    float zz = (float) ((iz - 128.0) / 127.0);
                    normals[i] = new Vector3(xx,yy,zz);
                }

                mesh.normals = normals;
            }

            if((format & 0x4) == 0x4) { // have tangents
                Vector4[] tangents = new Vector4[vertCount];

                for (int i = 0; i < tangents.Length; ++i)  {
                    byte ix = buf.ReadByte();
                    byte iy = buf.ReadByte();
                    byte iz = buf.ReadByte();
                    byte iw = buf.ReadByte();
                    float xx = (float) ((ix - 128.0) / 127.0);
                    float yy = (float) ((iy - 128.0) / 127.0);
                    float zz = (float) ((iz - 128.0) / 127.0);
                    float ww = (float) ((iw - 128.0) / 127.0);
                    tangents[i] = new Vector4(xx,yy,zz,ww);
                }

                mesh.tangents = tangents;
            }

            if((format & 0x8) == 0x8) { // have UVs
                Vector2[] uvs = new Vector2[vertCount];

                Vector2 uvbmin;
                Vector2 uvbmax;
                uvbmin.x = buf.ReadSingle();
                uvbmax.x = buf.ReadSingle();
                uvbmin.y = buf.ReadSingle();
                uvbmax.y = buf.ReadSingle();

                // Decode vectors as 16 bit integer components between the bounds
                for (int i = 0; i < uvs.Length; ++i)  {
                    System.UInt16 ix = buf.ReadUInt16();
                    System.UInt16 iy = buf.ReadUInt16();
                    float xx = (float) (ix / 65535.0 * (uvbmax.x - uvbmin.x) + uvbmin.x);
                    float yy = (float) (iy / 65535.0 * (uvbmax.y - uvbmin.y) + uvbmin.y);
                    uvs[i] = new Vector2(xx,yy);
                }
                mesh.uv = uvs;
            }

            if((format & 0x10) == 0x10) { //have colors
                Color32[] colors = new Color32[vertCount];

                for (int i = 0; i < colors.Length; i++) {
                    byte r = buf.ReadByte();
                    byte g = buf.ReadByte();
                    byte b = buf.ReadByte();
                    byte a = buf.ReadByte();
                    colors[i] = new Color32(r,g,b,a);
                }
                mesh.colors32 = colors;
            }

            // triangle indices
            int[] tris = new int[triCount * 3];
            for(int i = 0; i < triCount; ++i ) {
                tris[(i * 3) + 0] = buf.ReadUInt16();
                tris[(i * 3) + 1] = buf.ReadUInt16();
                tris[(i * 3) + 2] = buf.ReadUInt16();
            }
            mesh.triangles = tris;

            buf.Close();

            return mesh;
        }

        // Writes mesh to an array of bytes.
        public static byte[] WriteMesh(Mesh mesh, bool saveTangents) {
            if( mesh == null ) {
                Debug.Log("No mesh given!");
                return null;
            }

            Vector3[] verts = mesh.vertices;
            Vector3[] normals = mesh.normals;
            Vector4[] tangents = mesh.tangents;
            Vector2[] uvs = mesh.uv;
            Color32[] colors = mesh.colors32;
            int[] tris = mesh.triangles;

            // figure out vertex format
            byte format = 0x1;
            if( normals.Length > 0 )
                format |= 0x2;
            if( saveTangents && tangents.Length > 0 )
                format |= 0x4;
            if( uvs.Length > 0 )
                format |= 0x8;
            if( colors.Length > 0 )
                format |= 0x10;

            var stream = new MemoryStream();
            var buf = new BinaryWriter(stream);

            // write header
            System.UInt16 vertCount = (System.UInt16) verts.Length;
            System.UInt16 triCount = (System.UInt16) (tris.Length / 3);
            buf.Write(vertCount);
            buf.Write(triCount);
            buf.Write(format);


            // VERTICES
            Bounds bounds = new Bounds(verts[0], new Vector3(0.001f, 0.001f, 0.001f));
            foreach (Vector3 v in verts){bounds.Encapsulate(v);}

            Vector3 vbmin = bounds.min;
            Vector3 vbmax = bounds.max;
            buf.Write(vbmin.x);
            buf.Write(vbmax.x);
            buf.Write(vbmin.y);
            buf.Write(vbmax.y);
            buf.Write(vbmin.z);
            buf.Write(vbmax.z);

            foreach(Vector3 v in verts)  {
                float xx = Mathf.Clamp((float) ((v.x - vbmin.x) / (vbmax.x - vbmin.x) * 65535.0), 0.0f, 65535.0f);
                float yy = Mathf.Clamp((float) ((v.y - vbmin.y) / (vbmax.y - vbmin.y) * 65535.0), 0.0f, 65535.0f);
                float zz = Mathf.Clamp((float) ((v.z - vbmin.z) / (vbmax.z - vbmin.z) * 65535.0), 0.0f, 65535.0f);
                System.UInt16 ix = (System.UInt16) xx;
                System.UInt16 iy = (System.UInt16) yy;
                System.UInt16 iz = (System.UInt16) zz;
                buf.Write(ix);
                buf.Write(iy);
                buf.Write(iz);
            }

            // NORMALS
            foreach (Vector3 v in normals)  {
                byte ix = (byte) Mathf.Clamp((float)(v.x * 127.0 + 128.0), 0.0f, 255.0f);
                byte iy = (byte) Mathf.Clamp((float)(v.y * 127.0 + 128.0), 0.0f, 255.0f);
                byte iz = (byte) Mathf.Clamp((float)(v.z * 127.0 + 128.0), 0.0f, 255.0f);
                buf.Write(ix);
                buf.Write(iy);
                buf.Write(iz);
            }

            // TANGENTS
            if (saveTangents) {
                foreach (Vector4 v in tangents)  {
                    byte ix = (byte) Mathf.Clamp((float) (v.x * 127.0 + 128.0), 0.0f, 255.0f);
                    byte iy = (byte) Mathf.Clamp((float) (v.y * 127.0 + 128.0), 0.0f, 255.0f);
                    byte iz = (byte) Mathf.Clamp((float) (v.z * 127.0 + 128.0), 0.0f, 255.0f);
                    byte iw = (byte) Mathf.Clamp((float) (v.w * 127.0 + 128.0), 0.0f, 255.0f);
                    buf.Write(ix);
                    buf.Write(iy);
                    buf.Write(iz);
                    buf.Write(iw);
                }
            }

            // UVS
            Vector2 uvbmin = uvs[0] - new Vector2(0.001f,0.001f);
            Vector2 uvbmax = uvs[0] + new Vector2(0.001f,0.001f);
            foreach (var v in uvs)  {
                uvbmin.x = Mathf.Min(uvbmin.x, v.x);
                uvbmin.y = Mathf.Min(uvbmin.y, v.y);
                uvbmax.x = Mathf.Max(uvbmax.x, v.x);
                uvbmax.y = Mathf.Max(uvbmax.y, v.y);
            }

            buf.Write(uvbmin.x);
            buf.Write(uvbmax.x);
            buf.Write(uvbmin.y);
            buf.Write(uvbmax.y);

            foreach (Vector2 v in uvs)  {
                float xx = (float) ((v.x - uvbmin.x) / (uvbmax.x - uvbmin.x) * 65535.0);
                float yy = (float) ((v.y - uvbmin.y) / (uvbmax.y - uvbmin.y) * 65535.0);
                System.UInt16 ix = (System.UInt16) xx;
                System.UInt16 iy = (System.UInt16) yy;
                buf.Write(ix);
                buf.Write(iy);
            }

            //COLORS
            foreach (Color32 c in colors) {
                buf.Write(c.r);
                buf.Write(c.g);
                buf.Write(c.b);
                buf.Write(c.a);
            }

            // TRIANGLE INDICES
            foreach(int idx in tris)  {
                System.UInt16 idx16 = (System.UInt16) idx;
                buf.Write(idx16);
            }
            buf.Close();

            return stream.ToArray();
        }
    }
}
