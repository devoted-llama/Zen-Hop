using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour {
    public static PlatformController instance = null;

    public Platform platformPrefab;

    float startPosition = 0f;
    public int numberOfPlatforms = 20;
    public float platformSeparation = 4f;
    int transitionPlatformIndex { get { return numberOfPlatforms / 2; } } 
    float depth = -2.45f;

    public float minHeight;
    public float maxHeight;

    Platform[] platforms;

    public bool transitioning = false;

    int numberOfNewPlatforms;

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
        platforms = new Platform[(int)numberOfPlatforms];
        for (int i = 0; i < platforms.Length; i++) {
            platforms[i] = Instantiate(platformPrefab);
        }
    }

    public void GeneratePlatforms() {
        GeneratePlatforms(startPosition);
    }

    void GeneratePlatforms(float start) {
        for (int i = 0; i < numberOfPlatforms; i++) {
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

    void RepositionNewPlatform(int index, int triggerPlatformIndex, float startX) {
        int repositionIndex = numberOfPlatforms + transitionPlatformIndex - triggerPlatformIndex - 1;
        
        if (index >= repositionIndex) {
            platforms[index].AnimateInvisible();
            Vector3 position = new Vector3(startX + (index * platformSeparation), Random.Range(minHeight, maxHeight), depth);
            platforms[index].transform.position = position;
            platforms[index].AnimateFadeIn();
        }
    }

    void RepositionExistingPlatform(int index) {
        if (index < numberOfPlatforms - numberOfNewPlatforms) {
            platforms[index].bounceTime = Time.time;
            platforms[index].transform.position = platforms[index + (numberOfNewPlatforms)].transform.position;
        }
    }

    void RepositionPlatforms(int triggerPlatformIndex) {
        numberOfNewPlatforms = triggerPlatformIndex - transitionPlatformIndex + 1;

        if(numberOfNewPlatforms == 0) {
            return;
        }
        float startX = platforms[numberOfNewPlatforms].transform.position.x;
        for (int i = 0; i < numberOfPlatforms; i++) {
            platforms[i].id += numberOfNewPlatforms;
            RepositionExistingPlatform(i );
            RepositionNewPlatform(i, triggerPlatformIndex, startX);
            
        }
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
        int platformIndex = GetIndexOfPlatform(platform);
        
        if (GetFrogIsTouchingPlatform(platform) == true) {
            RepositionPlatforms(platformIndex);
        }
        transitioning = false;
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
