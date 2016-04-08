using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class POI : MonoBehaviour
{
    public LayerMask targetMask;

    public Text textfield = null;
    public Image background = null;

    public RectTransform rect = null;
    private RectTransform parentRect = null;
	
    void Start()
    {
        if (rect == null)
        {
            rect = GetComponent<RectTransform>();
        }
        parentRect = rect.parent.GetComponent<RectTransform>();
        textfield.text = "";
        background.enabled = false;
    }

    public float maxMoveDelta = 10f;

	// Update is called once per frame
	void Update () 
    {
        Vector2 outPos;
        bool isOnScreen = RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, Input.mousePosition, Camera.main, out outPos);
        if (isOnScreen)
        {
            rect.anchoredPosition = Vector2.MoveTowards(rect.anchoredPosition, outPos, maxMoveDelta);
        }
        else
        {
            return;
        }

        Ray camToCity = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit info;
        bool success = false;
        if (Physics.Raycast(camToCity, out info, 1000f, targetMask.value))
        {
            if (info.collider != null && info.collider.tag.Equals("POI"))
            {
                string target = info.collider.gameObject.name;
                textfield.text = target.Substring(0, Mathf.Min(textfield.text.Length + 1, target.Length));
                success = true;
            }
        }

        if (!success)
        {
            textfield.text = (textfield.text.Length <= 1 ? string.Empty : textfield.text.Substring(0, textfield.text.Length - 1));
        }

        background.enabled = (textfield.text.Length > 0);

	}
}
