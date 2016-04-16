using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class ResizeOnDrag : MonoBehaviour
{
    public RectTransform rect = null;
    protected RectTransform parentRect = null;

    public Vector2 minSize = new Vector2(100f, 100f);

    void Start()
    {
        if (rect == null) rect = GetComponent<RectTransform>();
        parentRect = rect.parent.GetComponent<RectTransform>();
    }

    private Vector2 cachedNewPos;

    public void ResizeScreen(Vector2 mousePos)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, mousePos, Camera.main, out cachedNewPos))
        {
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, cachedNewPos.x - rect.anchoredPosition.x + 10f);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, cachedNewPos.y + rect.anchoredPosition.y + 10f);

            cachedNewPos.Set(Mathf.Max(rect.sizeDelta.x, minSize.x), Mathf.Max(rect.sizeDelta.y, minSize.y));
            rect.sizeDelta = cachedNewPos;
        }
    }
}
