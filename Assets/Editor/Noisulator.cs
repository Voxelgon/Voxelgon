using UnityEditor;
using UnityEngine;
using System.IO;
using Voxelgon.Util.Geometry;

namespace Voxelgon.Editor {

    public class Noisulator : EditorWindow {

        // FIELDS

        private Vector2 _scrollPos;

        private Texture2D _tri_noiseTex;
        private string _tri_path = "/Textures/";
        private OutputType _tri_type = OutputType.PNG;

        private Texture2D _app_noiseTex;
        private string _app_name = "_TBlueNoise";

        enum OutputType {
            Asset,
            PNG
        }

        [MenuItem("Window/Voxelgon/Noise Triangulator")]
        public static void ShowWindow() {
            EditorWindow.GetWindow(typeof(Noisulator), false, "Noisulator™", true);
        }

        void OnEnable() {
            //_currTexture = AssetDatabase.LoadAssetAtPath("Assets/Textures/Colormap.png", typeof(Texture2D)) as Texture2D;
        }
        void OnGUI() {
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);

            GUILayout.Label("Triangulizer", EditorStyles.boldLabel);
            GUILayout.Label("Converts uniform noise to triangular noise", EditorStyles.wordWrappedLabel);

            _tri_noiseTex = EditorGUILayout.ObjectField("Input", _tri_noiseTex, typeof(Texture2D), false) as Texture2D;
            _tri_path = EditorGUILayout.TextField("Output Directory", _tri_path);
            _tri_type = (OutputType) EditorGUILayout.EnumPopup("Output Format", _tri_type);
            if (GUILayout.Button("▲▼▲Triangulimate!▲▼▲")) {
                Build();
            }
            GUILayout.Space(10);
            GUILayout.Label("Shader Global Setter", EditorStyles.boldLabel);
            GUILayout.Label("sets a global shader texture field", EditorStyles.wordWrappedLabel);

            _app_noiseTex = EditorGUILayout.ObjectField("Texture", _app_noiseTex, typeof(Texture2D), false) as Texture2D;
            _app_name = EditorGUILayout.TextField("Property Name", _app_name);
            if (GUILayout.Button("Apply!")) {
                Shader.SetGlobalTexture(_app_name, _app_noiseTex);
            }

            GUILayout.EndScrollView();
        }

        void Build() {
            var path = AssetDatabase.GetAssetPath(_tri_noiseTex);
            var name = System.IO.Path.GetFileNameWithoutExtension(path);
            var newTex = (Texture2D)Object.Instantiate(_tri_noiseTex);
            for (int x = 0; x < newTex.width; x++) {
                for (int y = 0; y < newTex.height; y++) {
                    Vector4 color = newTex.GetPixel(x, y);
                    for (int i = 0; i < 4; i++) {
                        var o = (color[i] * 2) - 1;
                        var t = Mathf.Max(-1, o / Mathf.Sqrt(Mathf.Abs(o)));
                        t -= Mathf.Sign(o);
                        t += 1f;
                        t /= 2;
                        color[i] = t;
                    }
                    newTex.SetPixel(x, y, color);
                }
            }
            switch (_tri_type) {
                case OutputType.PNG:
                    var bytes = newTex.EncodeToPNG();
                    File.WriteAllBytes(Application.dataPath + _tri_path + "T" + name + ".png", bytes);
                    break;
                case OutputType.Asset:
                    AssetDatabase.CreateAsset(newTex, "Assets" + _tri_path + "T" + name + ".asset");
                    AssetDatabase.SaveAssets();
                    break;
            }
        }
    }
}