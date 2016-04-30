using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class SetToDate : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
    {
        Text text = GetComponent<Text>();
        text.text = System.DateTime.Now.ToShortDateString();
	}
}
