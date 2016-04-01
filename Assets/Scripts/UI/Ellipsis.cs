using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class Ellipsis : MonoBehaviour {
    Text myText = null;
    [Range(0.5f, 20f)]
    public float dotEverySecond = 2f;

    private float timer = 0f;

	// Use this for initialization
	void Start () {
        myText = GetComponent<Text>();
        if (myText == null)
        {
            gameObject.SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        timer += Time.deltaTime;
        myText.text = string.Empty.PadRight(Mathf.Clamp(Mathf.FloorToInt(timer / dotEverySecond), 0, 3), '.');
        timer %= (dotEverySecond * 4f);
	}
}
