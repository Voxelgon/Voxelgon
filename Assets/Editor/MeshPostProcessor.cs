using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Voxelgon.Editor {
    public class MeshPostProcessor : AssetPostprocessor {

        bool Exclude() {
            return assetPath.Contains("Unmapped");
        }

        void OnPreprocessModel() {
            if (Exclude()) return;

            ModelImporter modelImporter = assetImporter as ModelImporter;
            modelImporter.weldVertices = false;
            modelImporter.importNormals = ModelImporterNormals.Import;
            modelImporter.animationType = ModelImporterAnimationType.None;
            modelImporter.importAnimation = false;
            modelImporter.isReadable = true;
            modelImporter.addCollider = false;
            modelImporter.importBlendShapes = false;
            modelImporter.generateSecondaryUV = false;
        }

        void OnPostprocessModel(GameObject g) {
            if (Exclude()) return;

            Colorize(g);
        }

        Material OnAssignMaterialModel(Material mat, Renderer rend) {
            if (Exclude()) return null;

            return AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Standard.mat");
        }

        void Colorize(GameObject g) {
            var filter = g.GetComponent<MeshFilter>();
            if (filter != null) {
                var mesh = filter.sharedMesh;
                var tex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Colormap.png");
                var newMesh = Colormaps.ColorMesh(mesh, tex);
                var rotation = Quaternion.FromToRotation(new Vector3(0,0,1), new Vector3(0,1,0));
                mesh.vertices = newMesh.vertices.Select(o => rotation * o).ToArray();
                mesh.normals = newMesh.normals;
                mesh.triangles = newMesh.triangles;
                mesh.uv = newMesh.uv;
                mesh.colors32 = newMesh.colors32;
            }

            foreach(Transform child in g.transform) {
                Colorize(child.gameObject);
            }
        }
    }
}
