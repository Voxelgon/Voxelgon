using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FPScounter : MonoBehaviour {

    public Text text;
    public int frameCount;
    public float dt;
    public float fps;
    public float updateRate = 1.0f;

    public void UpdateFPS() {
        //fps = 1.0f / deltatime;

        text.text = (int) fps + " FPS";
        Debug.Log(fps);
    }

    public void Update() {
        //deltatime = Time.smoothDeltaTime;
        frameCount++;
        dt += Time.deltaTime;
        if (dt > 1.0 / updateRate) {
            fps = frameCount / dt ;
            frameCount = 0;
            dt -= 1.0f / updateRate;

            text.text = (int) fps + " FPS";
        }
    }

    public void Start() {
        text = gameObject.GetComponent<Text>();
        //InvokeRepeating("UpdateFPS", 0.0f, 0.5f);
        //Debug.Log(fps);
    }

}
