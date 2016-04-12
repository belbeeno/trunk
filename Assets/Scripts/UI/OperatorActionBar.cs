using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(ToggleGroup))]
public class OperatorActionBar : MonoBehaviour 
{
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
   
    void Start()
    {
        if (toggles == null)
        {
            toggles = GetComponent<ToggleGroup>();
        }
    }

    void Update()
    {
        if (toggles.AnyTogglesOn() && Input.GetMouseButtonUp(0))
        {
            var actives = toggles.ActiveToggles().GetEnumerator();
            if (actives.MoveNext())
            {
                OperatorToggle opToggle = actives.Current.GetComponent<OperatorToggle>();
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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
                            //TrunkNetworkingOperator.Get().BroadcastSirens(hitInfo.point);
                            Debug.DrawLine(ray.origin, hitInfo.point, Color.blue, 10f);
                            break;
                    }

                    opToggle.OnUse();
                }
            }
        }
    }
}
