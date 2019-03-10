using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UIController : MonoBehaviour {
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;
	// Use this for initialization
	void Start () {
        m_Raycaster = GetComponent<GraphicRaycaster>();
        m_EventSystem = GetComponent<EventSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButton ("Fire1")) {
            m_PointerEventData = new PointerEventData (m_EventSystem);
            m_PointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult> ();
            if (m_Raycaster != null) {
                m_Raycaster.Raycast (m_PointerEventData, results);
                foreach (RaycastResult result in results) {
                    //Debug.LogFormat ("centre: {0}, pos: {1}",result.gameObject.transform.position,Camera.main.ScreenToWorldPoint(result.screenPosition));
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(result.screenPosition);
                    float adjacent = mousePos.x - result.gameObject.transform.position.x;
                    float opposite = mousePos.y - result.gameObject.transform.position.y;
                    
                    float angle = Mathf.Rad2Deg * Mathf.Atan ( adjacent / opposite);

                    Vector2 point1 = mousePos;
                    Vector2 point2 = result.gameObject.transform.position;

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
                    Debug.Log (Vector2.Distance (point1, point2));
                    Frog.instance.SetPower (Vector2.Distance(point1,point2));
                }

            }
        }

        if (Input.GetButtonUp ("Fire1")) {
            m_PointerEventData = new PointerEventData (m_EventSystem);
            m_PointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult> ();
            if (m_Raycaster != null) {
                m_Raycaster.Raycast (m_PointerEventData, results);
                foreach (RaycastResult result in results) {
                    Frog.instance.Jump ();
                }

            }
        }
    }
}
