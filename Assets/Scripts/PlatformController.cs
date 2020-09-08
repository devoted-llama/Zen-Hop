using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour {
    public static PlatformController Instance { get; private set; } = null;

    public bool Transitioning { get; private set; } = false;

    [SerializeField]
    Platform platformPrefab;
    float startPosition = 0f;
    [SerializeField]
    int numberOfPlatforms = 20;
    [SerializeField]
    float platformSeparation = 4f;
    [SerializeField]
    float minHeight = 0;
    [SerializeField]
    float maxHeight = 12;

    int TransitionPlatformIndex { get { return numberOfPlatforms / 2; } }
    float depth = -2.45f;
    Platform[] platforms;
    int numberOfNewPlatforms = 0;

    public static readonly string START_PLATFORM = "StartPlatform";
    public static readonly string TRANSITION_PLATFORM = "TransitionPlatform";
    public static readonly string PLATFORM = "Platform";

    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy (gameObject);
        }
    }

    void Start() {
        InstantiatePlatforms();
        PositionStartingPlatforms();
        Player.Instance.OnPlatformLanded += DoPlayerPlatformLandedActions;
    }

    void DoPlayerPlatformLandedActions(int platformId) {
        DoTransitionPlatformAction(platformId);
    }

    void InstantiatePlatforms() {
        platforms = new Platform[(int)numberOfPlatforms];
        for (int i = 0; i < platforms.Length; i++) {
            platforms[i] = Instantiate(platformPrefab);
        }
    }

    public void PositionStartingPlatforms() {
        PositionStartingPlatforms(startPosition);
    }

    void PositionStartingPlatforms(float start) {
        for (int i = 0; i < numberOfPlatforms; i++) {
            if (GameController.instance.Score == 0) {
                Vector3 position = new Vector3 (start + (i * platformSeparation), Random.Range (minHeight, maxHeight), depth);
                platforms [i].transform.position = position;
                platforms[i].id = i;
            }


            if (i == 0) {
                platforms [i].tag = START_PLATFORM;
            } else if (i >= TransitionPlatformIndex) {
                platforms [i].tag = TRANSITION_PLATFORM;
            } else {
                platforms [i].tag = PLATFORM;
            }
        }
    }


    void RepositionNewPlatform(int index, int triggerPlatformIndex, float startX) {
        int repositionIndex = numberOfPlatforms + TransitionPlatformIndex - triggerPlatformIndex - 1;
        
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
        
        numberOfNewPlatforms = triggerPlatformIndex - TransitionPlatformIndex + 1;

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

    bool GetPlayerIsTouchingPlatform(Platform platform) {
        return Helper.CheckRigidBodyContactsGameObjectHasComponent<Platform>(Player.Instance.RigidBody, platform.gameObject);
    }
    void DoTransitionPlatformAction(int platformId) {
        for (int i = 0; i < platforms.Length; i++) {
            if(platforms[i].id == platformId) {
                DoTransitionPlatformAction(platforms[i]);
                return;
            }
        }

    }

    void DoTransitionPlatformAction(Platform platform) {
        if(platform.CompareTag(TRANSITION_PLATFORM) == false) {
            return; // oops, we're not a transition platform!
        }
        Transitioning = true;
        int platformIndex = GetIndexOfPlatform(platform);
        
        if (GetPlayerIsTouchingPlatform(platform) == true) {
            RepositionPlatforms(platformIndex);
        }
        Transitioning = false;
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
