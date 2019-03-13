using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HillsController : MonoBehaviour {

    public Transform sprite1;
    public Transform sprite2;

    const float width = 18.58f;
    public float parallaxSpeed = 0.4f;
    public float scrollSpeed = 0.06f;


    public int count = 1;

    void Update() {
        Vector3 pos = transform.localPosition;
        pos.x = (-transform.parent.position.x * parallaxSpeed) - (scrollSpeed * Time.time);
        transform.localPosition = pos;

        if (pos.x <= (-width)*count) {
            Vector3 spritePos;
            if (count % 2 == 1) {
                spritePos = sprite1.localPosition; 
                spritePos.x = width * (count+1);
                sprite1.localPosition = spritePos;
            } else {
                spritePos = sprite2.localPosition;
                spritePos.x = width * (count+1);
                sprite2.localPosition = spritePos;
            }

            count++;
        } 

        if(pos.x > (-width)*(count-1)) {
            Vector3 spritePos;
            if (count % 2 == 0) {
                spritePos = sprite1.localPosition; 
                spritePos.x = width * (count-2);
                sprite1.localPosition = spritePos;
            } else {
                spritePos = sprite2.localPosition;
                spritePos.x = width * (count-2);
                sprite2.localPosition = spritePos;
            }

            count--;
        }
    }
}
