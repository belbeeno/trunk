using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class DroppedItemIcon : MonoBehaviour
{
    [SerializeField]
    RectTransform rect = null;
    public RectTransform Rect
    {
        get
        {
            if (rect == null) rect = GetComponent<RectTransform>();
            return rect;
        }
    }

    [SerializeField]
    Image myImage = null;
    public Image MyImage
    {
        get
        {
            if (myImage == null) myImage = GetComponent<Image>();
            return myImage;
        }
    }

    [SerializeField]
    CanvasGroup group = null;
    public CanvasGroup Group
    {
        get
        {
            if (group == null)
            {
                group = GetComponent<CanvasGroup>();
            }
            return group;
        }
    }

    [System.Serializable]
    public struct HintTypeToSpriteMapping
    {
        public NetMessage.APBResponse.Hint.HintType hintType;
        public Sprite icon;
    }
    public HintTypeToSpriteMapping[] mapping = new HintTypeToSpriteMapping[0];

    public bool Init(Vector2 pos, NetMessage.APBResponse.Hint.HintType iconType)
    {
        StopAllCoroutines();

        Rect.localScale = Vector3.zero;

        bool spriteFound = false;
        for (int i = 0; i < mapping.Length; i++)
        {
            if (mapping[i].hintType == iconType)
            {
                MyImage.sprite = mapping[i].icon;
                spriteFound = true;
                break;
            }
        }

        if (spriteFound)
        {
            gameObject.name = NetMessage.APBResponse.Hint.TypeToName(iconType);
            Rect.anchoredPosition = pos;
            Group.alpha = 1f;
            StartCoroutine(AnimateIn());
        }
        else
        {
            Group.alpha = 0f;
            return false;
        }

        return true;
    }

    public float animDuration = 1f;
    public IEnumerator AnimateIn()
    {
        float timer = 0f;
        Vector3 easedVector = new Vector3();
        while (timer <= animDuration)
        {
            easedVector.x = Ease.CubicEaseOut(timer, 0f, 1f, animDuration);
            easedVector.y = Ease.BounceEaseOut(timer, 0f, 1f, animDuration);
            easedVector.z = Mathf.Clamp01(timer / animDuration);
            transform.localScale = easedVector;
            yield return 0;
            timer += Time.deltaTime;
        }
        transform.localScale = Vector3.one;
    }
}
