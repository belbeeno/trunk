using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class Bouncer : MonoBehaviour {

    public GameObject _trunkLid = null;
    float _duration = 0f;
    float _timer = 0f;
    float _final = 4f; 
    bool _isBouncing = false;

    [Range(10, 200)]
    public float _maxAmp = 25f;

    float _start;

	// Update is called once per frame
	void Update () {
        if (!_isBouncing)
        {
            return;
        }

        _timer += Time.deltaTime;
        float t = _timer / _duration;
        float y = 0f;
        
        if (t < 0.5)
        {
            y = Ease.ElasticEaseIn(t, 0, _final, 1);
        }
        else
        {
            y = Ease.ElasticEaseIn(1-t, 0, _final, 1);
        }

        y = Mathf.Clamp((_start - y * _maxAmp), _start - _maxAmp, _start + _maxAmp);
        _trunkLid.transform.localRotation = Quaternion.Euler(0, y, 0);

        if (_timer > _duration)
        {
            _trunkLid.transform.localRotation = Quaternion.Euler(0, _start, 0);
            _isBouncing = false;
            //Debug.Log("Stopped Bouncing");
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
        
        //Debug.Log("Starting bounce");
    }
}
