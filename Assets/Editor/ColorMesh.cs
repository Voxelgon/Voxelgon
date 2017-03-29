using UnityEditor;
using UnityEngine;
using System.IO;

namespace Voxelgon.Editor {

    public class ColorMesh : EditorWindow {

        // FIELDS

        private Mesh _currMesh;
        private Texture2D _currTexture;
        private string _basePath = "Assets/Models/Imported/";

        [MenuItem("Window/Voxelgon/Mesh Colorizer")]
        public static void ShowWindow() {
            EditorWindow.GetWindow(typeof(ColorMesh), false, "Mesh Colorizer", true);
        }

        void OnEnable() {
            //_currTexture = AssetDatabase.LoadAssetAtPath("Assets/Textures/Colormap.png", typeof(Texture2D)) as Texture2D;
        }
        void OnGUI() {
            GUILayout.Label("Objects", EditorStyles.boldLabel);
            _currMesh = EditorGUILayout.ObjectField("Mesh", _currMesh, typeof(Mesh), false) as Mesh;
            _currTexture = EditorGUILayout.ObjectField("Color Map", _currTexture, typeof(Texture2D), false) as Texture2D;
            _basePath = EditorGUILayout.TextField("Base Path", _basePath);
            if (GUILayout.Button("Build")) {
                Build();
            }
        }

        void Build() {
            var path = AssetDatabase.GetAssetPath(_currMesh);
            var name = Path.GetFileNameWithoutExtension(path);
            var newMesh = Colormaps.ColorMesh(_currMesh, _currTexture);
            AssetDatabase.CreateAsset(newMesh, _basePath + name + ".asset");
            AssetDatabase.SaveAssets();
        }
    }
}