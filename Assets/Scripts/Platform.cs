using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {
    public int Id { get; set; }

    readonly int fadeOutHash = Animator.StringToHash("Fade Out");
    readonly int fadeInHash = Animator.StringToHash("Fade In");
    readonly int invisibleHash = Animator.StringToHash("Invisible");
    readonly int visibleHash = Animator.StringToHash("Visible");
    readonly int bounceHash = Animator.StringToHash("Bounce");

    Animator animator;

    public float bounceTime { get; set; } = 0;
    readonly float bounceWaitTime = 2f;

    private void Start() {
        animator = GetComponent<Animator>();    
    }

    public void AnimateFadeOut() {
        animator.SetTrigger(fadeOutHash);
    }

    public void AnimateFadeIn() {
        animator.SetTrigger(fadeInHash);
    }

    public void AnimateVisible() {
        animator.SetTrigger(visibleHash);
    }

    public void AnimateInvisible() {
        animator.SetTrigger(invisibleHash);
    }

    public void AnimateBounce() {
        if (CanBounce()) {
            bounceTime = Time.time;
            animator.SetTrigger(bounceHash);
        }
    }

    bool CanBounce() {
        if(Time.time > bounceTime + bounceWaitTime) {
            return true;
        }
        return false;
    }
}
