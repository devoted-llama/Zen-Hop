using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour {
    public static PlatformController instance = null;

    public Platform platformPrefab;

    float startPosition = 0f;
    public int amount = 20;
    public float platformSeparation = 4f;
    int transitionPlatformIndex { get { return amount / 2; } }
    float depth = -2.45f;

    public float minHeight;
    public float maxHeight;

    Platform[] platforms;

    public bool transitioning = false;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy (gameObject);
        }
    }

    void Start() {
        InitialisePlatforms();
        GeneratePlatforms ();
    }

    void InitialisePlatforms() {
        platforms = new Platform[(int)amount];
        for (int i = 0; i < platforms.Length; i++) {
            platforms[i] = Instantiate(platformPrefab);
        }
    }

    public void GeneratePlatforms() {
        GeneratePlatforms(startPosition);
    }

    void GeneratePlatforms(float start) {
        for (int i = 0; i < amount; i++) {
            if (GameController.instance.Score == 0) {
                Vector3 position = new Vector3 (start + (i * platformSeparation), Random.Range (minHeight, maxHeight), depth);
                platforms [i].transform.position = position;
                platforms[i].id = i;
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
        int newPlatforms = index - transitionPlatformIndex;

        float startX = platforms[newPlatforms].transform.position.x;
        
        int repositionIndex = amount + transitionPlatformIndex - index;
        for (int i = 0; i < amount; i++) {
            
            platforms[i].id += newPlatforms;

            if(i < newPlatforms) {
                platforms[i].AnimateFadeOut();
                yield return new WaitForSecondsRealtime(.2f);
                platforms[i].AnimateVisible();
            }
            if(i < amount-newPlatforms) {
                platforms[i].bounceTime = Time.time;
                platforms [i].transform.position = platforms [i + (newPlatforms)].transform.position;
            }
            if (i >= repositionIndex) {
                platforms[i].AnimateInvisible();
                Vector3 position = new Vector3 (startX + (i * platformSeparation), Random.Range (minHeight, maxHeight), depth);
                platforms [i].transform.position = position;
                platforms[i].AnimateFadeIn();
            }
            
        }
        transitioning = false;
    }

    bool CheckRigidbodyContactsHasPlatform(Collider2D[] contacts, Platform platform) {
        for (int i = 0; i < contacts.Length; i++) {
            if (contacts[i] != null) {
                Platform platformContact = contacts[i].GetComponent<Platform>();
                if (platformContact != null && platform.Equals(platformContact)) {
                    return true;
                }
            }
        }
        return false;
    }

    bool GetFrogIsTouchingPlatform(Platform platform) {
        Collider2D[] frogContacts = Frog.instance.GetRigidbodyContacts(2);
        return CheckRigidbodyContactsHasPlatform(frogContacts, platform);
    }


    public void TransitionPlatformAction(Platform platform) {
        if(platform.CompareTag("TransitionPlatform") == false) {
            return; // oops, we're not a transition platform!
        }
        transitioning = true;
        int index = GetIndexOfPlatform(platform);
        
        if (GetFrogIsTouchingPlatform(platform) == true) {
            RepositionPlatforms(index);
        }
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
