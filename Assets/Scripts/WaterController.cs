using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterController : MonoBehaviour
{
    Transform sprite1;
    Transform sprite2;

    public float scrollSpeed = 0.05f;
    public float width;

    public int count = 1;

    float startPos;
    float hangPosition;

    private void Awake() {
        startPos = transform.position.x;
        sprite1 = transform.GetChild(0).transform;
        width = sprite1.GetComponent<SpriteRenderer>().bounds.size.x;
        Vector3 sprite2Pos = sprite1.localPosition;
        Debug.Log(sprite2Pos.x);
        sprite2Pos.x -= width;
        Debug.Log(sprite2Pos.x);
        sprite2 = Instantiate(sprite1.GetComponent<SpriteRenderer>(), transform).transform;
        sprite2.transform.localPosition = sprite2Pos;
    }

    private void Update() {
        GoRight();
    }

    void GoRight() {
        Vector3 pos = transform.localPosition;

        pos.x =  startPos + (scrollSpeed * Time.time);
        transform.localPosition = pos;

        hangPosition = Camera.main.transform.position.x;

        if (pos.x > hangPosition + (width * count)) {
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

        if (pos.x < hangPosition + (width * (count - 1))) {
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
