using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProxyCameraMap : MonoBehaviour 
{
    public RectTransform dispatchMapRect = null;
    public Camera proxyCamera = null;
    public RectTransform proxyCameraOverlay = null;

    public ToggleGroup toggles = null;
    private int _hostOnlyLayerMask = -1;
    private int HostOnlyLayerMask
    {
        get
        {
            if (_hostOnlyLayerMask == -1)
                _hostOnlyLayerMask = -1 ^ LayerMask.NameToLayer("HostOnly");
            return _hostOnlyLayerMask;
        }
    }

    private int fingerToUse = -1;
    public bool GetNormalizedMapPosition(out Vector2 pos, bool clamp = true)
    {
        pos = new Vector2();
        Vector2 inputPos = Vector2.one * -1f;
        if (Input.touchSupported)
        {
            bool inputFound = false;
            for (int i = 0; i < Input.touches.Length; i++)
            {
                if (fingerToUse == -1 
                    || fingerToUse == Input.touches[i].fingerId)
                {
                    if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(dispatchMapRect, Input.touches[i].position, Camera.main, out inputPos))
                    {
                        return false;
                    }
                    fingerToUse = Input.touches[i].fingerId;
                    inputFound = true;
                }
            }
            if (!inputFound)
            {
                fingerToUse = -1;
                return false;
            }
        }
        else
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(dispatchMapRect, Input.mousePosition, Camera.main, out inputPos))
            {
                return false;
            }
        }

        if (Mathf.Approximately(dispatchMapRect.rect.width, 0f) || Mathf.Approximately(dispatchMapRect.rect.height, 0f))
        {
            return false;
        }

        pos.Set(inputPos.x / dispatchMapRect.rect.width, inputPos.y / dispatchMapRect.rect.height);
        pos += Vector2.one * 0.5f;
        if (clamp)
        {
            pos.Set(Mathf.Clamp01(pos.x), Mathf.Clamp01(pos.y));
        }
        else
        {
            if (pos.x < 0f || pos.x > 1f
                || pos.y < 0f || pos.y > 1f)
            {
                return false;
            }
        }
        return true;
    }
    public Vector2 GetProxyMapPosition(Vector2 normalizedMapPos)
    {
        Vector3 returnPoint = proxyCamera.ViewportToWorldPoint(normalizedMapPos);
        return new Vector2(returnPoint.x, returnPoint.z);
    }

    void Update()
    {
        if (toggles.AnyTogglesOn() && Input.GetMouseButtonDown(0))
        {
            var actives = toggles.ActiveToggles().GetEnumerator();
            if (actives.MoveNext())
            {
                OperatorToggle opToggle = actives.Current.GetComponent<OperatorToggle>();
                Vector2 normPos;
                if (GetNormalizedMapPosition(out normPos, false))
                {
                    Ray ray = proxyCamera.ViewportPointToRay(normPos);
                    RaycastHit hitInfo;
                    if (Physics.Raycast(ray, out hitInfo, 1000f, HostOnlyLayerMask))
                    {
                        switch (opToggle.action)
                        {
                            case OperatorToggle.OperatorAction.APB:
                                TrunkNetworkingOperator.Get().RequestAPB(hitInfo.point);
                                Debug.DrawLine(ray.origin, hitInfo.point, Color.green, 10f);
                                break;
                            case OperatorToggle.OperatorAction.Siren:
                                TrunkNetworkingOperator.Get().TriggerPoliceInHostageScene(new Vector2(hitInfo.point.x, hitInfo.point.z));
                                Debug.DrawLine(ray.origin, hitInfo.point, Color.blue, 10f);
                                break;
                            case OperatorToggle.OperatorAction.Helicopter:
                                TrunkNetworkingOperator.Get().TriggerHelicopterInHostageScene(hitInfo.point.z, true);
                                Debug.DrawLine(ray.origin, hitInfo.point, Color.red, 10f);
                                break;
                        }

                        opToggle.OnUse();
                    }
                    else
                    {
                        Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.yellow, 10f);
                        toggles.SetAllTogglesOff();
                    }
                }
                else
                {
                    toggles.SetAllTogglesOff();
                }
            }
        }
    }
}
