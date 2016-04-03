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
                bool useUpToggle = false;
                OperatorToggle opToggle = actives.Current.GetComponent<OperatorToggle>();
                switch (opToggle.action)
                {
                    case OperatorToggle.OperatorAction.APB:
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hitInfo;
                        if (Physics.Raycast(ray, out hitInfo, 1000f, HostOnlyLayerMask))
                        {
                            useUpToggle = true;
                            TrunkNetworkingOperator.Get().RequestAPB(hitInfo.point);

                            Debug.DrawLine(ray.origin, hitInfo.point, Color.green, 10f);
                        }
                        else if (Physics.Raycast(ray, out hitInfo, 1000f))
                        {
                            Debug.DrawLine(ray.origin, hitInfo.point, Color.red, 10f);
                        }
                        break;
                }

                if (useUpToggle)
                {
                    opToggle.OnUse();
                }
            }
        }
    }
}
