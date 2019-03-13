using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UIController : MonoBehaviour {
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    public Button powerButton;
	// Use this for initialization
	void Start () {
        m_Raycaster = GetComponent<GraphicRaycaster>();
        m_EventSystem = GetComponent<EventSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        if (PlatformController.instance.transitioning == false && Frog.instance.rigidBody.velocity.x == 0 && Frog.instance.rigidBody.velocity.y == 0) {
            powerButton.interactable = true;
        } else {
            powerButton.interactable = false;
        }
        m_PointerEventData = new PointerEventData (m_EventSystem);
        m_PointerEventData.position = Input.mousePosition;
        if (m_Raycaster != null) {
            List<RaycastResult> results = new List<RaycastResult> ();
            m_Raycaster.Raycast (m_PointerEventData, results);


            if (Input.GetButton ("Fire1")) {
                if (results.Count == 0) {
                    Frog.instance.SetPower (0);
                    Frog.instance.SetAim (false);
                }
                if(powerButton.IsInteractable()){
                    foreach (RaycastResult result in results) {
                        Frog.instance.SetAim (true);
                        //Debug.LogFormat ("centre: {0}, pos: {1}",result.gameObject.transform.position,Camera.main.ScreenToWorldPoint(result.screenPosition));
                        // Debug.Log(result.screenPosition);
                        Vector3 buttonPos = Camera.main.ScreenToWorldPoint (result.gameObject.GetComponent<RectTransform> ().position);
                        //Debug.LogFormat("rect: {0}, worldpos: {1}",result.gameObject.GetComponent<RectTransform>().position,buttonPos);
                        Vector3 mousePos = Camera.main.ScreenToWorldPoint (result.screenPosition);

                        Vector3 rightSide = Camera.main.ScreenToWorldPoint (new Vector3 (Camera.main.pixelWidth, 0, 0));
                        float radius = rightSide.x - buttonPos.x;

                        float adjacent = mousePos.x - buttonPos.x;
                        float opposite = mousePos.y - buttonPos.y;
                
                        float angle = Mathf.Rad2Deg * Mathf.Atan (adjacent / opposite);

                        Vector2 point1 = mousePos;
                        Vector2 point2 = buttonPos;

                        //Debug.LogFormat ("h: {0}, Dist: {1}, sin: {2}, angle: {3}", hypotenuse, Vector2.Distance(point1,point2),Mathf.Sin (Mathf.Deg2Rad*angle), angle);

                        float modifier = 0;

                        if (adjacent >= 0 && opposite >= 0) {
                            modifier = 0;
                        } else if (adjacent > 0 && opposite < 0) {
                            modifier = 180f;
                        } else if (adjacent < 0 && opposite < 0) {
                            modifier = 180f;
                        } else if (adjacent < 0 && opposite > 0) {
                            modifier = 360f;
                        }

                        Frog.instance.SetAimerAngle (angle, modifier);
                        //Debug.Log (Vector2.Distance (point1, point2));
                        //Debug.LogFormat ("radius:{0},dist:{1}", radius, Vector2.Distance (point1, point2));

                        Frog.instance.SetPower (Vector2.Distance (point1, point2) / radius);
                   
                    }
                } 
            }

            if (Input.GetButtonUp ("Fire1")) {
                foreach (RaycastResult result in results) {
                    Frog.instance.SetAim (false);
                    Frog.instance.Jump ();
                }
            }
        }
    }
}
