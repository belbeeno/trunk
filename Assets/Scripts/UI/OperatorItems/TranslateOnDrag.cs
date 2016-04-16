using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class TranslateOnDrag : MonoBehaviour
{
    public RectTransform rect = null;
    protected RectTransform parentRect = null;

    void Start()
    {
        if (rect == null) rect = GetComponent<RectTransform>();
        parentRect = rect.parent.GetComponent<RectTransform>();
    }

    private Vector2 cachedNewPos;
    private bool isMoving = false;
    private Vector2 startOffset;

    public void StartMovingScreen(Vector2 mousePos)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, mousePos, Camera.main, out cachedNewPos))
        {
            isMoving = true;
            startOffset.Set(cachedNewPos.x - rect.anchoredPosition.x, cachedNewPos.y + rect.anchoredPosition.y);
        }
    }

    public void MoveScreen(Vector2 mousePos)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, mousePos, Camera.main, out cachedNewPos))
        {
            cachedNewPos.x -= startOffset.x;
            cachedNewPos.y = -cachedNewPos.y + startOffset.y;
            rect.anchoredPosition = cachedNewPos;
        }
    }

    public void StopMovingScreen()
    {
        isMoving = false;
        cachedNewPos = rect.anchoredPosition;
        if (rect.offsetMax.x > parentRect.rect.xMax)
        {
            cachedNewPos.x -= rect.offsetMax.x - parentRect.rect.xMax;
        }
        else if (rect.offsetMin.x < 0f)
        {
            cachedNewPos.x = 0f;
        }

        if (rect.offsetMax.y > 0f)
        {
            cachedNewPos.y = 0f;
        }
        else if (rect.offsetMin.y < -parentRect.rect.yMax)
        {
            cachedNewPos.y -= rect.offsetMin.y + parentRect.rect.yMax;
        }
        rect.anchoredPosition = cachedNewPos;
    }
}
