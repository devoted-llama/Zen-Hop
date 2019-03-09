using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour {
    public static PlatformController instance = null;

    public GameObject platformPrefab;
    public GameObject endPlatformPrefab;

    const float amount = 5;
    const int leftMargin = 100;

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

    }



    void GeneratePlatforms(int start = 0, int leftPixels = leftMargin, int rightPixels = 0) {
        float gap = 1.8f;
        if (rightPixels == 0) {
            rightPixels = Camera.main.pixelWidth;
        }

        float minHeight = Camera.main.ScreenToWorldPoint (new Vector3 (0, 0, 0)).y + 1;
        float maxHeight = Camera.main.ScreenToWorldPoint (new Vector3 (0, Camera.main.pixelHeight, 0)).y - 1;

        float minWidth = Camera.main.ScreenToWorldPoint (new Vector3 (leftPixels, 0, 0)).x;
        float maxWidth = Camera.main.ScreenToWorldPoint (new Vector3 (rightPixels, 0, 0)).x;

        //Debug.LogFormat ("{0},{1}", minWidth, maxWidth);

        float widthDiff = maxWidth - minWidth;
        float fractionalWidth = widthDiff / amount;

        for (int i = start; i < amount; i++) {
            Vector3 randomPosition = new Vector3 (minWidth+(fractionalWidth*i)+gap, Random.Range(minHeight,maxHeight), 0);
            platforms [i].transform.position = randomPosition;
            if (i == amount-1) {
                platforms [i].tag = "EndPlatform";
            } 
        }
    }

    void RegeneratePlatforms() {
        platforms [0].transform.position = platforms [(int)amount-1].transform.position;
        platforms [0].tag = "StartPlatform";
        int leftPixels = Camera.main.pixelWidth;
        int rightPixels = leftPixels * 2;
        int reduction = leftPixels / (int)amount;
        leftPixels -=  reduction;
        rightPixels -= reduction;
        GeneratePlatforms (1, leftPixels, rightPixels-leftMargin);
    }


    public void EndPlatformAction() {
        RegeneratePlatforms ();
        float endWidth = Camera.main.ScreenToWorldPoint (new Vector3 (Camera.main.pixelWidth, 0, 0)).x;
        float startWidth = Camera.main.ScreenToWorldPoint (new Vector3 (0, 0, 0)).x;
        Vector3 startPos = Camera.main.transform.position;
        float maxWidth = endWidth - startWidth;
        Vector3 endPos = startPos;
        endPos.x += maxWidth - (maxWidth / amount);
        //Debug.LogFormat ("{0},{1}", startPos, endPos);
        endPos.x -= 2f;
        CameraController.instance.Move (endPos);
    }


}
