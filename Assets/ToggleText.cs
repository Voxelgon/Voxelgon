using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToggleText : MonoBehaviour {

    public Text text;

    public void Start() {
        text = gameObject.GetComponent<Text>();
    }

    public void Toggle() {
        text.enabled = !text.enabled;
    }

}
