using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class TextboxResizer : UIBehaviour
{
    public Text textBox = null;
    public InputField inputField = null;
    Vector2 textBoxStartingSize = new Vector2();
    public RectTransform myRect = null;
    public ScrollRect scroller = null;

    // Caret won't be masked because of: https://issuetracker.unity3d.com/issues/scrollview-mask-scroll-view-doesnt-mask-the-input-field-selection-and-the-caret
    protected Transform caret = null;

    protected override void Start()
    {
        if (myRect == null) myRect = GetComponent<RectTransform>();
        if (textBox == null || inputField == null)
        {
            enabled = false;
            return;
        }
        textBoxStartingSize = textBox.rectTransform.sizeDelta;
    }

    protected IEnumerator AssignScrollerNextFrame(float val)
    {
        yield return new WaitForEndOfFrame();
        scroller.verticalScrollbar.value = val;
    }

    protected override void OnRectTransformDimensionsChange()
    {
        if (enabled)
        {
            if (caret == null)
            {
                caret = inputField.transform.FindChild("InputText Input Caret");
                if (!caret) return;
                caret.SetParent(textBox.transform, false);
            }
            Vector2 newSize = new Vector2(textBoxStartingSize.x, Mathf.Max(textBoxStartingSize.y, myRect.sizeDelta.y));
            textBox.rectTransform.sizeDelta = newSize;
            if (gameObject.activeSelf)
            {
                if (inputField.caretPosition >= inputField.text.Length - 1)
                {
                    StopAllCoroutines();
                    StartCoroutine(AssignScrollerNextFrame(scroller.verticalScrollbar.direction == Scrollbar.Direction.BottomToTop ? 0f : 1f));
                }
                else if (inputField.caretPosition <= 0)
                {
                    StopAllCoroutines();
                    StartCoroutine(AssignScrollerNextFrame(scroller.verticalScrollbar.direction == Scrollbar.Direction.BottomToTop ? 1f : 0f));
                }
            }
        }

        base.OnRectTransformDimensionsChange();
    }
}
