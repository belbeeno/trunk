using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Banking : MonoBehaviour {
    [SerializeField]
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
        _t = Mathf.Clamp01(_timer / _duration);

        var y = 0f; 
        if (_t < 0.5)
        {
            y = Ease.QuartEaseOut(_t, 0, _maxAngle, _duration / 2);
        } else
        {
            y = Ease.QuartEaseOut(1 - _t, 0, _maxAngle, _duration / 2);
        }

        var angles = y * _direction; 

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
