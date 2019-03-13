using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour {
    public static PlatformController instance = null;

    public GameObject platformPrefab;
    public GameObject endPlatformPrefab;


    const float startPosition = 0f;
    const int amount = 6;
    const float platformSeparation = 4f;
    const int transitionPlatformIndex = 3;
    const float depth = -2.45f;

    GameObject[] platforms;

    int fadeOutHash = Animator.StringToHash ("Fade Out");
    int fadeInHash = Animator.StringToHash ("Fade In");
    int invisibleHash = Animator.StringToHash ("Invisible");
    int visibleHash = Animator.StringToHash ("Visible");

    public bool transitioning = false;

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
            if (GameController.instance.level == 0) {
                Vector3 position = new Vector3 (start + (i * 4), Random.Range (2, 8), -2.45f);
                platforms [i].transform.position = position;
            }


            if (i == 0) {
                platforms [i].tag = "StartPlatform";
            } else if (i >= transitionPlatformIndex) {
                platforms [i].tag = "TransitionPlatform";
            } else {
                platforms [i].tag = "Platform";
            }
        }
        //platforms [transitionPlatformIndex].GetComponent<Animator> ().SetTrigger (fadeInHash);
    }

    void RepositionPlatforms(int index) {
        float start = platforms[index-2].transform.position.x;
        int repositionIndex = 8 - index;
        for (int i = 0; i < amount; i++) {

            if(i < amount-(index-2)) {
                platforms [i].transform.position = platforms [i + (index-2)].transform.position;
            }
            if (i >= repositionIndex) {
                Vector3 position = new Vector3 (start + (i * 4), Random.Range (2, 8), depth);
                platforms [i].transform.position = position;
            }
        }
        //platforms [transitionPlatformIndex].GetComponent<Animator> ().SetTrigger (fadeInHash);
    }


    public void EndPlatformAction(GameObject platform) {
        StartCoroutine (EndPlatformActionCoroutine (platform));
    }

    IEnumerator EndPlatformActionCoroutine(GameObject platform) {
        transitioning = true;

        int index = GetIndexOfPlatform (platform);

        platforms [index].tag = "TransitionPlatform__";

        int colliderSize = 2;
        bool transitionPlatform = false;

        yield return new WaitForSeconds (1f);
       
        Collider2D[] colliders = new Collider2D[colliderSize];
        Frog.instance.rigidBody.GetContacts (colliders);
        for (int i = 0; i < colliderSize; i++) {
            if(colliders[i] != null && colliders [i].CompareTag ("TransitionPlatform__"))
                transitionPlatform = true;
        }

        Vector2 velocity = Frog.instance.rigidBody.velocity;

        if (transitionPlatform == true && velocity.x < 0.00001f && velocity.y < 0.00001f) {
            GameController.instance.NextLevel ();
            //platforms [0].GetComponent<Animator> ().SetTrigger (fadeOutHash);
            //yield return new WaitUntil (() => platforms [0].transform.localScale.x == 0);

            RepositionPlatforms (index);

            //Vector3 pos = Camera.main.transform.position;
            //pos.x += platformSeparation * (amount-1);
            //CameraController.instance.Move (pos);
        }
        platforms [index].tag = "TransitionPlatform";
        transitioning = false;
    }

    int GetIndexOfPlatform(GameObject platform) {
        for (int i = 0; i < platforms.Length; i++) {
            if(platforms[i].Equals(platform)){
                return i;
            }
        }
        return -1;
    }

}
