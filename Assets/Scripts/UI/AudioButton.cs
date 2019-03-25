using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AudioButton : MonoBehaviour
{
    public Image onImage;
    public Image offImage;

    // Start is called before the first frame update
    void Start() {
        AudioController.instance.ToggleSoundEvent += ToggleOnOff;
        ToggleOnOff(AudioController.instance.Sound);
    }

    private void OnDestroy() {
        AudioController.instance.ToggleSoundEvent -= ToggleOnOff;
    }

    void ToggleOnOff(bool state) {
        if(state) {
            onImage.gameObject.SetActive(true);
            offImage.gameObject.SetActive(false);
        } else {
            onImage.gameObject.SetActive(false);
            offImage.gameObject.SetActive(true);
        }
    }

}
