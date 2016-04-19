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

    public ProxyCameraMap proxyMap = null;
	
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
        if (proxyMap == null) return;
        Vector2 normPos;
        bool success = false;
        
        if (proxyMap.GetNormalizedMapPosition(out normPos, false))
        {
            Vector2 localPos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, Input.mousePosition, Camera.main, out localPos))
            {
                rect.anchoredPosition = Vector2.MoveTowards(rect.anchoredPosition, localPos, maxMoveDelta * Time.deltaTime);
                success = true;
            }
        }

        if (success)
        {
            success = false;
            Ray camToCity = proxyMap.proxyCamera.ViewportPointToRay(normPos);
            RaycastHit info;
            if (Physics.Raycast(camToCity, out info, 1000f, targetMask.value))
            {
                if (info.collider != null && info.collider.tag.Equals("POI"))
                {
                    string target = info.collider.gameObject.name;
                    textfield.text = target.Substring(0, Mathf.Min(textfield.text.Length + 1, target.Length));
                    success = true;
                }
            }
        }

        if (!success)
        {
            textfield.text = (textfield.text.Length <= 1 ? string.Empty : textfield.text.Substring(0, textfield.text.Length - 1));
        }

        background.enabled = (textfield.text.Length > 0);
	}
}
