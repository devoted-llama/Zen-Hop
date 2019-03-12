using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour {
    public static PlatformController instance = null;

    public GameObject platformPrefab;
    public GameObject endPlatformPrefab;

    public bool transitioning = false;

    const float startPosition = 1.5f;
    const int amount = 4;
    const int leftMargin = 100;

    GameObject[] platforms;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy (gameObject);
        }

        //DontDestroyOnLoad (gameObject);
    }

    void Start() {
        platforms = new GameObject[(int)amount];
        for (int i = 0; i < platforms.Length; i++) {
            platforms[i] = Instantiate (platformPrefab);
        }

        GeneratePlatforms (startPosition);
        //CameraController.instance.MoveTo (Frog.instance.transform);
    }



    void GeneratePlatforms(float start) {
        for (int i = 0; i < amount; i++) {
            
            Vector3 position = new Vector3 (start + (i*4), Random.Range(2,8) , 0);
            platforms [i].transform.position = position;

            if (GameController.instance.level > 0 && i == 0) {
                platforms [i].transform.position = platforms [amount - 1].transform.position;
            }

            switch (i) {
                case 0 :
                    platforms [i].tag = "StartPlatform";
                    break;
                case (amount - 1) :
                    platforms [i].tag = "EndPlatform";
                    break;
                default :
                    platforms [i].tag = "Platform";
                    break;
            }
        }
    }

    void RegeneratePlatforms() {
        GeneratePlatforms (platforms[platforms.Length-1].transform.position.x);
    }


    public void EndPlatformAction() {
        StartCoroutine (EndPlatformActionCoroutine ());
    }

    IEnumerator EndPlatformActionCoroutine() {
        platforms [amount - 1].tag = "EndPlatformTransition";
        transitioning = true;
        int colliderSize = 2;
        bool endPlatform = false;

        yield return new WaitForSeconds (2f);

        Collider2D[] colliders = new Collider2D[colliderSize];
        Frog.instance.rigidBody.GetContacts (colliders);
        for (int i = 0; i < colliderSize; i++) {
            if(colliders[i] != null && colliders [i].CompareTag ("EndPlatformTransition"))
                endPlatform = true;
        }

        Vector2 velocity = Frog.instance.rigidBody.velocity;

        if (endPlatform == true && velocity.x < 0.00001f && velocity.y < 0.00001f) {
            GameController.instance.NextLevel ();
            RegeneratePlatforms ();
            Vector3 pos = Camera.main.transform.position;
            pos.x += 12;
            CameraController.instance.Move (pos);
        } else {
            platforms [amount - 1].tag = "EndPlatform";
            transitioning = false;
        }

    }


}
