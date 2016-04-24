using UnityEngine;
using System.Collections;

public class PingWave : OperatorItemBase
{
    public enum PingState : int
    {
        Invalid = -1,

        Preview = 0,
        Action,
    }

    public PID.InitParams pidParams;
    PID pidX = null;
    PID pidY = null;

    protected override void Start()
    {
        pidX = new PID(pidParams, GetVisibleProgressX, GetTargetProgressX, SetVisibleProgressX);
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
            rect.anchoredPosition = value;
        }
    }

    protected override int PreviewState
    {
        get { return (int)PingState.Preview; }
    }

    public override void InitPositions(float oMax)
    {
        pidParams.oMax = oMax;
        pidX.AssignInit(pidParams);
        pidY.AssignInit(pidParams);        
    }

    public float GetVisibleProgressX()
    {
        return AnchoredPosition.x;
    }
    public float GetVisibleProgressY()
    {
        return AnchoredPosition.y;
    }

    public float GetTargetProgressX() { return target.x; }
    public float GetTargetProgressY() { return target.y; }

    public void SetVisibleProgressX(float val)
    {
        Vector2 pos = AnchoredPosition;
        pos.x = val;
        AnchoredPosition = pos;
    }
    public void SetVisibleProgressY(float val)
    {
        Vector2 pos = AnchoredPosition;
        pos.y = val;
        AnchoredPosition = pos;
    }

    public override void OperatorAction(OperatorToggle.OperatorAction action)
    {
        currentState = (int)PingState.Action;
    }

    protected override void ResetPositions()
    {
        pidX.Reset();
        pidY.Reset();
    }

    protected override void UpdatePositions()
    {
        pidX.Compute();
        pidY.Compute();
    }
}
