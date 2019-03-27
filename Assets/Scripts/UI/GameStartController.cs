using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartController : MonoBehaviour {
    public static GameStartController instance = null;

    public GameObject gamePanel;
    public GameObject gameStartPanel;
    public Frog frog;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }

        //DontDestroyOnLoad(gameObject);
    }

    public void Play() {
        CameraController.instance.finishMoving += SetPlayActive;
        CameraController.instance.MoveTo(PlatformController.instance.GetPlatformById(0).transform,5);
        gameStartPanel.SetActive(false);

    }

    void SetPlayActive() {
        frog.gameObject.SetActive(true);
        gamePanel.SetActive(true);
        GameController.instance.playing = true;
        CameraController.instance.finishMoving -= SetPlayActive;
    }
}
