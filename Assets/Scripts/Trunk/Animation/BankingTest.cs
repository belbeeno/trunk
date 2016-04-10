using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class BankingTest : MonoBehaviour {

    Banking _bank =null;
    [Range(1,10)]
    public float duration = 3f;
    [Range(-30, 30)]
    public float maxAngle = 5f;

    Bouncer _bounce = null;

	// Use this for initialization
	void Start () {
       _bank = GetComponent<Banking>();
       _bounce = GetComponent<Bouncer>();

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.A))
        {
            _bank.startBanking(Vector3.left, duration, maxAngle);
        } else if (Input.GetKey(KeyCode.D))
        {
            _bank.startBanking(Vector3.right, duration, maxAngle);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            _bank.startBanking(Vector3.down, duration, maxAngle);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            _bank.startBanking(Vector3.up, duration, maxAngle);
        }
        else if (Input.GetKey(KeyCode.B))
        {
            _bounce.Bounce(duration);
        }

    }
}
