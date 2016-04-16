using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ChangeCursorOnMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Texture2D cursorToUse = null;
    public Vector2 hotSpot = new Vector2();
    public CursorMode mode = CursorMode.Auto;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(cursorToUse, hotSpot, mode);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.SetCursor(null, Vector2.zero, mode);
    }
}
