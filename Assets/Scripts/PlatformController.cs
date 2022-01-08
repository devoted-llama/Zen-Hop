using UnityEngine;

public class PlatformController : MonoBehaviour {
    public static PlatformController Instance { get; private set; } = null;

    public bool Transitioning { get; private set; } = false;

    [SerializeField]
    Platform platformPrefab;
    [SerializeField]
    int numberOfPlatforms = 10;
    [SerializeField]
    float platformSeparation = 10f;
    [SerializeField]
    float minHeight = 0f;
    [SerializeField]
    float maxHeight = 12f;

    int TransitionPlatformIndex { get { return numberOfPlatforms / 2; } }
    readonly float depth = -2.45f;
    readonly float startPosition = 0f;
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
        SubscribeToPlayerPlatformLanded();
    }

    void SubscribeToPlayerPlatformLanded() {
        Player.Instance.OnPlatformLanded += DoPlayerPlatformLandedActions;
    }

    void DoPlayerPlatformLandedActions(int platformId) {
        DoTransitionPlatformAction(platformId);
    }

    void InstantiatePlatforms() {
        platforms = new Platform[numberOfPlatforms];
        for (int i = 0; i < platforms.Length; i++) {
            platforms[i] = Instantiate(platformPrefab);
        }
    }

    void PositionPlatformBasedOnIndex(int i) {
        Vector3 position = new Vector3(startPosition + (i * platformSeparation), Random.Range(minHeight, maxHeight), depth);
        platforms[i].transform.position = position;
    }

    void SetPlatformTagBasedOnIndex(int i) {
        if (i == 0) {
            platforms[i].tag = START_PLATFORM;
        } else if (i >= TransitionPlatformIndex) {
            platforms[i].tag = TRANSITION_PLATFORM;
        } else {
            platforms[i].tag = PLATFORM;
        }
    }

    public void PositionStartingPlatforms() {
        for (int i = 0; i < numberOfPlatforms; i++) {

            PositionPlatformBasedOnIndex(i);
            platforms[i].Id = i;


            SetPlatformTagBasedOnIndex(i);
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
            platforms[i].Id += numberOfNewPlatforms;
            RepositionExistingPlatform(i );
            RepositionNewPlatform(i, triggerPlatformIndex, startX);
        }
    }



    void DoTransitionPlatformAction(int platformId) {
        for (int i = 0; i < platforms.Length; i++) {
            if(platforms[i].Id == platformId) {
                DoTransitionPlatformAction(platforms[i]);
                return;
            }
        }
    }

    void DoTransitionPlatformAction(Platform platform) {
        if(platform.CompareTag(TRANSITION_PLATFORM) == false) {
            return;
        }
        Transitioning = true;
        int platformIndex = GetIndexOfPlatform(platform);
        
        RepositionPlatforms(platformIndex);
        
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
            if(platforms[i].Id == id) {
                return platforms[i];
            }
        }
        return null;
    }

}
