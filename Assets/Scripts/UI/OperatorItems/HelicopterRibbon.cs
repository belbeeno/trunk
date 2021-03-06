﻿using UnityEngine;
using System.Collections;

public class HelicopterRibbon : OperatorItemBase 
{
    public enum HeliState : int
    {
        Invalid = -1,

        Preview = 0,
        Action,
    }

    public PID.InitParams pidParams;
    PID pidY = null;

    protected override void Start()
    {
        pidY = new PID(pidParams, GetVisibleProgressY, GetTargetProgressY, SetVisibleProgressY);
        base.Start();
    }

    protected override Vector2 AnchoredPosition
    {
        get
        {
            return rect.anchoredPosition;
        }
        set
        {
            Vector2 pos = rect.anchoredPosition;
            pos.y = value.y;
            rect.anchoredPosition = pos;
        }
    }

    protected float timer = -1f;
    protected override bool CanSetStateToInvalid() 
    {
        return ((currentState == (int)HeliState.Action) && (timer <= 0f));
    }

    protected override int PreviewState
    {
        get { return (int)HeliState.Preview; }
    }
    public float GetVisibleProgressY()
    {
        return AnchoredPosition.y;
    }
    public float GetTargetProgressY() { return target.y; }
    public void SetVisibleProgressY(float val)
    {
        Vector2 pos = AnchoredPosition;
        pos.y = val;
        AnchoredPosition = pos;
    }

    public override void OperatorAction(OperatorToggle.OperatorAction action)
    {
        currentState = (int)HeliState.Action;
        timer = GameSettings.HELICOPTER_PING_DURATION;
    }
    public override void InitPositions(float oMax)
    {
        pidParams.oMax = oMax;
        pidY.AssignInit(pidParams);
    }
    protected override void ResetPositions()
    {
        pidY.Reset();
    }
    protected override void UpdatePositions()
    {
        pidY.Compute();
        if (currentState == (int)HeliState.Action)
        {
            timer -= Time.deltaTime;
        }
    }
}
