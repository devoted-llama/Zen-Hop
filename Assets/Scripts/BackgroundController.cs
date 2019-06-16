using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour {

    Transform sprite1;
    Transform sprite2;

    public float width;
    public float parallaxSpeed = 0.4f;
    public float scrollSpeed = 0.05f;


   public int count = 1;

    private void Awake() {
        sprite1 = transform.GetChild(0).transform;
        width = sprite1.GetComponent<SpriteRenderer>().bounds.size.x;
        float x = width + sprite1.transform.position.x;
        Vector3 sprite2Pos = sprite1.position;
        sprite2Pos.x = width;
        sprite2 = Instantiate(sprite1.GetComponent<SpriteRenderer>(), transform).transform;
        sprite2.transform.position = sprite2Pos;
        
    }

    void Update() {
        GoRight();
    }

    void GoLeft() {
        Vector3 pos = transform.localPosition;
        pos.x = (-transform.parent.localPosition.x * parallaxSpeed) - (scrollSpeed * Time.time);
        transform.localPosition = pos;

        if (pos.x <= (-width) * count) {
            Vector3 spritePos;
            if (count % 2 == 1) {
                spritePos = sprite1.localPosition;
                spritePos.x = width * (count + 1);
                sprite1.localPosition = spritePos;
            } else {
                spritePos = sprite2.localPosition;
                spritePos.x = width * (count + 1);
                sprite2.localPosition = spritePos;
            }

            count++;
        }

        if (pos.x > (-width) * (count - 1)) {
            Vector3 spritePos;
            if (count % 2 == 0) {
                spritePos = sprite1.localPosition;
                spritePos.x = width * (count - 2);
                sprite1.localPosition = spritePos;
            } else {
                spritePos = sprite2.localPosition;
                spritePos.x = width * (count - 2);
                sprite2.localPosition = spritePos;
            }

            count--;
        }
    }

    void GoRight() {
        Vector3 pos = transform.localPosition;
        
        pos.x = (transform.parent.localPosition.x * parallaxSpeed) + (scrollSpeed * Time.time);
        transform.localPosition = pos;

        if (pos.x > width * count) {
            Vector3 spritePos;
            if (count % 2 == 1) {
                spritePos = sprite1.localPosition;
                spritePos.x = (-width) * (count+1);
                sprite1.localPosition = spritePos;
            } else {
                spritePos = sprite2.localPosition;
                spritePos.x = (-width) * (count+1);
                sprite2.localPosition = spritePos;
            }

            count++;
        }

       if (pos.x < width * (count-1)) {
            Vector3 spritePos;
            if (count % 2 == 0) {
                spritePos = sprite1.localPosition;
                spritePos.x = (-width) * (count - 2);
                sprite1.localPosition = spritePos;
            } else {
                spritePos = sprite2.localPosition;
                spritePos.x = (-width) * (count - 2);
                sprite2.localPosition = spritePos;
            }

            count--;
        }
    }
}
