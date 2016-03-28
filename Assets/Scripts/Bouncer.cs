using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Bouncer : MonoBehaviour {

    GameObject _trunkLid = null;
    public float _duration = 0f;
    public float _timer = 0f;

    [Range(1, 100)]
    public float _final = 4f; 

    bool _isBouncing;

    [Range(1, 250)]
    public float _maxAmp = 3f;

    [Range(0, 1)]
    public float t = 0f;

    public float _start;

    public float y = 0f;

    // Use this for initialization
    void Start () {
	    _trunkLid = GameObject.Find("Trunk Hinge");
    }
	
	// Update is called once per frame
	void Update () {
        if (!_isBouncing)
        {
            return;
        }

        _timer += Time.deltaTime;
        t = _timer / _duration;
        if (t < 0.5)
        {
            y = Ease.ElasticEaseIn(t, 0, _final, 1);
        }
        else
        {
            y = Ease.ElasticEaseIn(1-t, 0, _final, 1);
        }

        y = Mathf.Max(360f, (_start - y * _maxAmp)); 
        y = Mathf.Max(y, 270f);

        _trunkLid.transform.localRotation = Quaternion.Euler(0, y, 0);

        if (_timer > _duration)
        {
            _trunkLid.transform.localRotation = Quaternion.Euler(0, _start, 0);
            _isBouncing = false;
            Debug.Log("Stopped Bouncing");
        }
    }

    public void Bounce(float duration)
    {
        if (_isBouncing)
        {
            return;
        }
        _start = _trunkLid.transform.localRotation.eulerAngles.y;
        _duration = duration;
        _timer = 0;
        _isBouncing = true;
        Debug.Log("Starting bounce");
    }
}
