using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Banking : MonoBehaviour {
    GameObject _rotationLayer;

    [Range(0, 1)]
    public float _t = 0;

    float _maxAngle;
    public Vector3 _direction;
    float _duration; 

    public bool _isBanking = false;

    float _timer = 0f;


    // Use this for initialization
    void Start () {
        _rotationLayer = GameObject.Find("Rotation Layer");
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isBanking)
        {
            return; 
        }
        _timer += Time.deltaTime;
        _t = _timer / _duration; 

        var t1 = (2 * _t - 1);
        var t2 = t1 * t1 * t1 * t1;
        var t3 = Mathf.Max(-t2 + 1, 0);

        var angles = t3*_maxAngle * _direction; 

        _rotationLayer.transform.localRotation = Quaternion.Euler(angles);

        if (_timer >= _duration)
        {
            stopBanking(); 
        }
         
    }


    public void startBanking(Vector3 direction, float duration, float maxAngle)
    {  
        if (_isBanking)
        {
            Debug.Log("Currently already banking");
            return; 
        }
        _direction = direction;
        _maxAngle = maxAngle;
        _duration = duration; 
        _isBanking = true;
        _timer = 0f;
    }

    public void stopBanking()
    {
        _direction = new Vector3(0,0,0);
        _maxAngle = 0f;
        _duration = 0f;
        _isBanking = false;
        _timer = 0f;
    }
}
