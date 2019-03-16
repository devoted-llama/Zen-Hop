using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour {
    public static PlatformController instance = null;

    public Platform platformPrefab;

    const float startPosition = 0f;
    const int amount = 6;
    const float platformSeparation = 4f;
    const int transitionPlatformIndex = 3;
    const float depth = -2.45f;

    Platform[] platforms;

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
    }

    void Start() {
        platforms = new Platform[(int)amount];
        for (int i = 0; i < platforms.Length; i++) {
            platforms[i] = Instantiate (platformPrefab);
            platforms[i].id = i;
        }

        GeneratePlatforms (startPosition);
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
    }

    void RepositionPlatforms(int index) {
        StartCoroutine(RepositionPlatformsCoroutine(index));
    }

    IEnumerator RepositionPlatformsCoroutine(int index) {
        //index - 2 = the number of "new" platforms to create
        int newPlatforms = index - 2;

        float startX = platforms[newPlatforms].transform.position.x;

        // 8 - index = the starting index of the "new" platforms
        int repositionIndex = 8 - index;
        for (int i = 0; i < amount; i++) {
            
            platforms[i].id += newPlatforms;

            if(i < newPlatforms) {
                platforms[i].GetComponent<Animator>().SetTrigger(fadeOutHash);
                yield return new WaitForSecondsRealtime(.2f);
                platforms[i].GetComponent<Animator>().SetTrigger(visibleHash);
            }
            if(i < amount-newPlatforms) {
                platforms [i].transform.position = platforms [i + (newPlatforms)].transform.position;
            }
            if (i >= repositionIndex) {
                platforms[i].GetComponent<Animator>().SetTrigger(invisibleHash);
                Vector3 position = new Vector3 (startX + (i * 4), Random.Range (2, 8), depth);
                platforms [i].transform.position = position;
                platforms[i].GetComponent<Animator>().SetTrigger(fadeInHash);
            }
            
        }
        transitioning = false;
    }


    public void TransitionPlatformAction(Platform platform) {
        transitioning = true;

        int index = GetIndexOfPlatform(platform);

        platforms[index].tag = "TransitionPlatform__";

        int colliderSize = 2;
        bool transitionPlatform = false;

        Collider2D[] colliders = new Collider2D[colliderSize];
        Frog.instance.rigidBody.GetContacts(colliders);
        for (int i = 0; i < colliderSize; i++) {
            if (colliders[i] != null && colliders[i].CompareTag("TransitionPlatform__"))
                transitionPlatform = true;
        }

        if (transitionPlatform == true) {
            RepositionPlatforms(index);
        }
        platforms[index].tag = "TransitionPlatform";
    }

    int GetIndexOfPlatform(Platform platform) {
        for (int i = 0; i < platforms.Length; i++) {
            if(platforms[i].Equals(platform)){
                return i;
            }
        }
        return -1;
    }

    public Platform GetPlatformById(int id) {
        for(int i = 0; i<platforms.Length; i++) {
            if(platforms[i].id == id) {
                return platforms[i];
            }
        }
        return null;
    }

}
