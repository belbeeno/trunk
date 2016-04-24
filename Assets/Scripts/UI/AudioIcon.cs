using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class AudioIcon : MonoBehaviour 
{
    public VoiceChat.VoiceChatPlayer player = null;
    public Sprite[] spriteLevels = new Sprite[0];
    public Image image = null;
    public int volume = 4;

    float GetNormalizedVolume()
    {
        volume = Mathf.Clamp(volume, 0, spriteLevels.Length - 1);
        return (spriteLevels.Length > 0 ? (float)(volume + 1f) / spriteLevels.Length : 1f);
    }

    public void Start()
    {
        if (image == null) image = GetComponent<Image>();
        SetVolume(GetNormalizedVolume());
    }

    public void IncVolume()
    {
        volume++;
        SetVolume(GetNormalizedVolume());
    }

    public void DecVolume()
    {
        volume--;
        SetVolume(GetNormalizedVolume());
    }

    private void SetVolume(float vol)
    {
        if (player != null && player.gameObject.activeSelf)
        {
            player.source.volume = Mathf.Clamp01(vol);
        }
        image.sprite = spriteLevels[volume];
    }
}
