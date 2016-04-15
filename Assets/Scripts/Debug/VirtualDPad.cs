using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class VirtualDPad : MonoBehaviour
{
    private int touchLeft = -1;
    private Vector2 startLeftPos = new Vector2();

    public float leftThumbDeadzone = 20f;
    public Vector2 cursorSensitivity = new Vector2(0.5f, 0.5f);

    public Transform cameraToLetFly = null;

    public float moveSpeed = 10f;

    public bool IsLooking()
    {
        return touchLeft != -1;
    }

    private int touchRight = -1;
    public bool IsButtonDown()
    {
        return touchRight != -1;
    }

    private bool TryGetTouch(int fingerId, ref Touch retVal)
    {
        for (int i = 0; i < Input.touches.Length; i++)
        {
            if (fingerId == Input.touches[i].fingerId)
            {
                retVal = Input.touches[i];
                return true;
            }
        }
        return false;
    }

    void UpdateFingerIds()
    {
        int currentWatchedInputs = (IsLooking() ? 1 : 0);
        currentWatchedInputs += (IsButtonDown() ? 1 : 0);
        Touch[] touches = Input.touches;

        if (Input.touchCount > currentWatchedInputs
            && currentWatchedInputs < 2)
        {
            for (int i = 0; i < touches.Length; i++)
            {
                if (!IsLooking()
                    && touches[i].position.x < Screen.width / 2f)
                {
                    touchLeft = touches[i].fingerId;
                    startLeftPos = touches[i].position;
                }
                else if (!IsButtonDown()
                    && touches[i].position.x > Screen.width / 2f)
                {
                    touchRight = touches[i].fingerId;
                }

                if (IsLooking() && IsButtonDown()) break;
            }
        }

        bool hasLeft = false;
        bool hasRight = false;

        for (int i = 0; i < touches.Length; i++)
        {
            if (hasLeft && hasRight) break;

            hasLeft |= touches[i].fingerId == touchLeft;
            hasRight |= touches[i].fingerId == touchRight;
        }

        if (!hasLeft)
        {
            touchLeft = -1;
        }
        if (!hasRight)
        {
            touchRight = -1;
        }
    }

    float rotationX, rotationY;
    void Update()
    {
        if (!Debug.isDebugBuild) return;

        if (cameraToLetFly != null && Input.touchCount >= 5)
        {
            cameraToLetFly.transform.parent = transform;
            cameraToLetFly = null;
        }

        if (cameraToLetFly != null) return;

        UpdateFingerIds();

        if (IsLooking())
        {
            Touch myTouch = new Touch();
            if (TryGetTouch(touchLeft, ref myTouch))
            {
                DebugConsole.SetText("myTouch", myTouch.position.ToString());
                Vector2 moveDir = myTouch.position - startLeftPos;
                if (moveDir.sqrMagnitude < leftThumbDeadzone * leftThumbDeadzone)
                {
                    moveDir = Vector2.zero;
                }
                DebugConsole.SetText("MoveDirection", moveDir.ToString());

                rotationX += moveDir.x * cursorSensitivity.x * Time.deltaTime;
                rotationY += moveDir.y * cursorSensitivity.x * Time.deltaTime;
                rotationY = Mathf.Clamp(rotationY, -90, 90);
            }
        }

        transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
        if (IsButtonDown())
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }
}
