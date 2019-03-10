using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour {
    public static PlatformController instance = null;

    public GameObject platformPrefab;
    public GameObject endPlatformPrefab;

    const int amount = 4;
    const int leftMargin = 100;

    int level = 0;

    GameObject[] platforms;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy (gameObject);
        }

        DontDestroyOnLoad (gameObject);
    }

    void Start() {
        platforms = new GameObject[(int)amount];
        for (int i = 0; i < platforms.Length; i++) {
            
            platforms[i] = Instantiate (platformPrefab);
            if (i == 0) {
                platforms [i].tag = "StartPlatform";
            }
        }

        GeneratePlatforms ();
        //CameraController.instance.MoveTo (Frog.instance.transform);
    }



    void GeneratePlatforms(float start = 0.5f) {
        for (int i = 0; i < amount; i++) {
            
            Vector3 position = new Vector3 (start + (i*4), Random.Range(2,8) , 0);
            platforms [i].transform.position = position;

            if (level > 0 && i == 0) {
                platforms [i].transform.position = platforms [amount - 1].transform.position;
            }

            if (i == 0) {
                platforms [i].tag = "StartPlatform";
            } else if (i == amount - 1) {
                platforms [i].tag = "EndPlatform";
            } else {
                platforms [i].tag = "Platform";
            }
        }
    }

    void RegeneratePlatforms() {

        GeneratePlatforms (platforms[platforms.Length-1].transform.position.x);
    }


    public void EndPlatformAction() {
        level++;
        RegeneratePlatforms ();
        Vector3 pos = Camera.main.transform.position;
        pos.x += 12;
        CameraController.instance.Move (pos);
    }


}
