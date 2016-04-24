using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScalingInputField : InputField
{
    private RectTransform rectTransform;
    private ScrollRect scroller = null;
    protected override void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        scroller = transform.parent.GetComponent<ScrollRect>();
        onValueChanged.AddListener(new UnityEngine.Events.UnityAction<string>(ResizeInput));
    }

    // Resize input field as new lines get added
    private void ResizeInput(string iText)
    {
        string fullText = text;

        Vector2 extents = textComponent.rectTransform.rect.size;
        var settings = textComponent.GetGenerationSettings(extents);
        settings.generateOutOfBounds = false;
        var prefHeight = new TextGenerator().GetPreferredHeight(fullText, settings) + 16;

        if (prefHeight > textComponent.rectTransform.rect.height - 16 || prefHeight < textComponent.rectTransform.rect.height + 16)
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, prefHeight);
        }

        if (scroller != null)
        {
            if (caretPosition >= text.Length - 1)
            {
                scroller.verticalScrollbar.value = (scroller.verticalScrollbar.direction == Scrollbar.Direction.BottomToTop ? 0f : 1f);
            }
            else if (caretPosition <= 0)
            {
                scroller.verticalScrollbar.value = (scroller.verticalScrollbar.direction == Scrollbar.Direction.BottomToTop ? 1f : 0f);
            }
        }
    }

}
