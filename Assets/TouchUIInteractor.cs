using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class TouchUIInteractor : MonoBehaviour
{
    [SerializeField] TMP_Text debugTxt;

	private void Start()
	{
        Debug.Log("TouchUIInteractor start");
	}
	void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Debug.Log("TouchUIInteractor: Touch!");
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.GetTouch(0).position;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (RaycastResult result in results)
            {
                //debugTxt.text = result.gameObject.name + "\r\n" + debugTxt.text; 
                var button = result.gameObject.GetComponent<UnityEngine.UI.Button>();
                if (button != null)
                {
                    Debug.Log("TouchUIInteractor: Button Pressed:" + result.gameObject.name);
                    button.onClick.Invoke();
                    break;
                }
            }
        }
    }
}