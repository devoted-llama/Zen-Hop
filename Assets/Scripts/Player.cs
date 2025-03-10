﻿using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour {
    public static Player Instance { get; private set; } = null;
    public Rigidbody2D RigidBody { get; private set; }
    float PowerWithMultiplier { get { return powerAmount * powerForceMultiplier; } }

    [SerializeField] float powerForceMultiplier = 1000f;
    float powerAmount = 0;
    int currentPlatformId = 0;
    Vector3 startPosition;
    float jumpAngle = 0;
    bool doingPlatformActionsCoroutine = false;
    Coroutine platformActionCoroutine;

    public delegate void PlatformLandedAction(int platformId);
    public event PlatformLandedAction OnPlatformLanded;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }

        GameStartSetActive();

        startPosition = transform.position;
    }

    public void ResetToStartPosition() {
        transform.position = startPosition;
    }

    void GameStartSetActive() {
        if(GameController.Instance.Playing == false) {
            gameObject.SetActive(false);
        }
    }

    void Start() {
        RigidBody = GetComponent<Rigidbody2D>();
        SetPower(0);

    }

    public void Jump() {
        if (gameObject.activeSelf && GetRigidBodyVelocityEquals(0)) {
            StartCoroutine(JumpCoroutine());
        }
    }

    IEnumerator JumpCoroutine() {
        yield return new WaitForSeconds(0.5f);
        RigidBody.AddForce(GetForce());

        SetPower(0);
    }

    public void SetJumpAngle(float angle, float modifier = 0) {
        if (!float.IsNaN(angle)) {
            jumpAngle = modifier + angle;
        }
    }


    Vector2 GetForce() {
        float angle = jumpAngle;

        float ratio = 0f;
        float forceX = 0f;
        float forceY = 0f;

        if (angle >= 0 && angle <= 90) {
            ratio = angle / 90f;
            forceX = ratio * PowerWithMultiplier;
            forceY = (1 - ratio) * PowerWithMultiplier;
        } else if (angle > 90 && angle <= 180) {
            ratio = (angle - 90) / 90f;
            forceX = (1 - ratio) * PowerWithMultiplier;
            forceY = -(ratio) * PowerWithMultiplier;
        } else if (angle > 180 && angle <= 270) {
            ratio = (angle - 180) / 90f;
            forceX = -ratio * PowerWithMultiplier;
            forceY = -(1 - ratio) * PowerWithMultiplier;
        } else if (angle > 270 && angle <= 360) {
            ratio = (angle - 270) / 90f;
            forceX = -(1 - ratio) * PowerWithMultiplier;
            forceY = ratio * PowerWithMultiplier;
        }
        
        Vector2 force = new Vector2(forceX, forceY);

        return force;
    }

    public void SetPower(float power) {
        powerAmount = power > 1 ? 1 : power;
    }

    /* PLatforms should care something lands on them, the player shouldn't care */
    void OnCollisionEnter2D(Collision2D collision) {

        Platform platform = collision.gameObject.GetComponent<Platform>();
        if (platform != null) {
            platform.AnimateBounce();
            DoPlatformActions(platform);
        }
    }

    void DoPlatformActions(Platform platform) {
        platformActionCoroutine = StartCoroutine(DoPlatformActionsCoroutine(platform));
    }

    bool GetRigidBodyVelocityEquals(float velocity) {
        return RigidBody.linearVelocity.x == velocity &&
            RigidBody.linearVelocity.y == velocity;
    }

    bool GetRigidBodyVelocityLessThan(float velocity) {
        return RigidBody.linearVelocity.x < velocity && 
            RigidBody.linearVelocity.y < velocity && 
            RigidBody.linearVelocity.x > -velocity && 
            RigidBody.linearVelocity.y > -velocity;
    }

    bool GetHasLandedOnPlatform() {
        if (Helper.CheckRigidBodyContactsParentsHasComponent<Platform>(RigidBody)) {
            return true;
        }
        return false;
    }

    bool GetHasLandedOnPlatformAndStopped() {
        return GetHasLandedOnPlatform() && GetRigidBodyVelocityLessThan(0.1f);
    }

    IEnumerator DoPlatformActionsCoroutine(Platform platform) {
       
        if(doingPlatformActionsCoroutine == true) {
            yield break;
        }
        doingPlatformActionsCoroutine = true;

        while (GetHasLandedOnPlatformAndStopped() == false) {
            yield return new WaitForSecondsRealtime(.1f);
        }

        currentPlatformId = platform.Id;
        OnPlatformLanded(currentPlatformId);
        /* The game controller should be the one consulted about the state of the current platform */

        doingPlatformActionsCoroutine = false;
    }

    IEnumerator RespawnCoroutine() {
        Platform currentPlatform = PlatformController.Instance.GetPlatformById(0);
        Vector3 pos = currentPlatform.transform.position;
        /* Why are we telling the Camera where to go? */
        CameraController.Instance.MoveToInstant(currentPlatform.transform);
        yield return new WaitUntil (() => Camera.main.transform.position.x == pos.x);
        pos.y = GameController.Instance.RespawnHeight;
        Vector2 velocity = new Vector2 (0, -GameController.Instance.FallSpeed);
        RigidBody.linearVelocity = velocity;
        transform.position = pos;
        doingPlatformActionsCoroutine = false;
        if (platformActionCoroutine != null) {
            StopCoroutine(platformActionCoroutine);
        }
    }

    public void Respawn() {
        StartCoroutine(RespawnCoroutine());
    }

    public bool IsReady() {
        if (gameObject != null && RigidBody != null) {
            return (gameObject.activeSelf && RigidBody.linearVelocity.x == 0 && RigidBody.linearVelocity.y == 0);
        }
        return false;
    }
}
