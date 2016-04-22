using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class ChangeDesktop : MonoBehaviour 
{
    public Image myImage = null;

    public Sprite[] desktops = new Sprite[0];
    int currentDesktop = -1;

	// Use this for initialization
	void Start () 
    {
        if (myImage == null) myImage = GetComponent<Image>();
        RandomDesktop();
	}
	
    public void RandomDesktop()
    {
        if (desktops.Length > 0)
        {
            if (currentDesktop >= 0)
            {
                currentDesktop = (currentDesktop + Random.Range(1, Mathf.Max(desktops.Length - 1, 1))) % desktops.Length;
            }
            else
            {
                currentDesktop = Random.Range(0, desktops.Length);
            }
            myImage.sprite = desktops[currentDesktop];
        }
    }
}
