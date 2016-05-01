using UnityEngine;

//[ExecuteInEditMode]
public class TrunkMover : MonoBehaviour 
{   
    public float speed = 1f;
    public float maxTurnAngle = 25f;

    [MinMaxSlider(1f, 120f)]
    public Vector2 bounceRangeInSeconds = new Vector2(45f, 75f);
    public float bounceDuration = 1f;
    
    private Route _route;
    public bool isMoving = false;
    public Banking banking = null;
    public Bouncer bouncer = null;
    public Latch latch = null;

    float timer = 0f;
    bool wasTurning = false;

    [HideInInspector]
    public Vector3 forward = new Vector3();

    public void Start()
    {
        _route = new Route();
        if (banking == null)
        {
            banking = GetComponentInChildren<Banking>();
        }
        if (bouncer == null)
        {
            bouncer = GetComponentInChildren<Bouncer>();
        }

        timer = Random.Range(bounceRangeInSeconds.x, bounceRangeInSeconds.y);
        forward = transform.forward;
    }

	public void Update () 
    {
        forward = transform.forward;
        if (!isMoving) return;

        if (_route != null && _route.IsAtIntersection() != wasTurning)
        {
            if (_route.IsAtIntersection())
            {
                banking.startBanking(_route.GetTurningDir(), _route.GetTurnDuration(speed), maxTurnAngle);
            }
            wasTurning = _route.IsAtIntersection();
        }

        if ( _route != null 
            && _route.CanMove())
        {
            _route.Update(transform, speed, Time.deltaTime);
        }

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            if (latch.isOpen)
            {
                bouncer.Bounce(bounceDuration);
                var audioSource = GetComponent<CardboardAudioSource>();
                audioSource.Play();
            }
            timer = Random.Range(bounceRangeInSeconds.x, bounceRangeInSeconds.y);
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(Camera.main.transform.position + Camera.main.transform.forward * (GameSettings.HOSTAGE_CULLING_RADIUS / 2f), GameSettings.HOSTAGE_CULLING_RADIUS);
    }
}
