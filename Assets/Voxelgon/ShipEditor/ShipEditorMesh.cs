using UnityEngine;

namespace Voxelgon.Assets.Voxelgon.ShipEditor
{
    public class ShipEditorMesh : MonoBehaviour {

<<<<<<< HEAD
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
=======
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
    
>>>>>>> d2b354f9a1be5ce77b5b224eb67a7d8de87b4434
}
