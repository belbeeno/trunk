using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class DragBehaviour : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [System.Serializable]
    public class ManipulateVector2 : UnityEvent<Vector2> { }
    public ManipulateVector2 OnDragStartEvent;
    public ManipulateVector2 OnDragEvent;
    public UnityEvent OnDragEndEvent;

    public bool flipY = true;
    private Vector2 newPos;

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (eventData.IsPointerMoving() 
            && eventData.button == PointerEventData.InputButton.Left)
        {
            if (flipY)
            {
                newPos.Set(eventData.position.x, Screen.height - eventData.position.y);
                OnDragEvent.Invoke(newPos);
            }
            else
            {
                OnDragEvent.Invoke(eventData.position);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (flipY)
            {
                newPos.Set(eventData.position.x, Screen.height - eventData.position.y);
                OnDragStartEvent.Invoke(newPos);
            }
            else
            {
                OnDragStartEvent.Invoke(eventData.position);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnDragEndEvent.Invoke();
        }
    }
}
