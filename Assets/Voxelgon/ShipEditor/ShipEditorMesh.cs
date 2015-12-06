using UnityEngine;

namespace Voxelgon.Assets.Voxelgon.ShipEditor
{
    public class ShipEditorMesh : MonoBehaviour {

        private MeshFilter filter;
        private ShipEditor editor;


        // Use this for initialization
        void Start () {
            filter = gameObject.GetComponent<MeshFilter>();
            editor = GameObject.Find("ShipEditor").GetComponent<ShipEditor>();
        }
    
        // Update is called once per frame
        void Update () {
            filter.mesh = editor.SimpleHullMesh;

        }
    
    }
}
