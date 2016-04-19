using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OperatorPanAndZoom : MonoBehaviour 
{
    public Camera proxyCamera = null;

    public Scrollbar hScrollBar = null;
    public Scrollbar vScrollBar = null;
    public Slider zoomSlider = null;

    public float MaxOrthographicSize = 1200f;
    public float MinOrthographicSize = 300f;

    public float MinPos
    {
        get { return proxyCamera.orthographicSize; }
    }
    public float MaxPos
    {
        get { return 2f * MaxOrthographicSize - proxyCamera.orthographicSize; }
    }

	// Use this for initialization
	void Start () 
    {
        if (proxyCamera == null)
        {
            Debug.LogError("OperatorPanAndZoom missing a proxy camera!", gameObject);
        }
	    if (hScrollBar == null || vScrollBar == null || zoomSlider == null)
        {
            Debug.LogError("OperatorPanAndZoom missing some UI elements!", gameObject);
        }

        zoomSlider.onValueChanged.AddListener(OnZoomSliderValueChanged);
        hScrollBar.onValueChanged.AddListener(OnHorizontalScroll);
        vScrollBar.onValueChanged.AddListener(OnVerticalScroll);
	}

    public void Init()
    {
        MaxOrthographicSize = proxyCamera.orthographicSize;
        zoomSlider.value = 0f;
        hScrollBar.value = 0.5f;
        vScrollBar.value = 0.5f;
        hScrollBar.size = 1f;
        vScrollBar.size = 1f;
    }

    public void OnZoomSliderValueChanged(float value)
    {
        proxyCamera.orthographicSize = Mathf.Lerp(MaxOrthographicSize, MinOrthographicSize, value);
        hScrollBar.size = proxyCamera.orthographicSize / MaxOrthographicSize;
        vScrollBar.size = proxyCamera.orthographicSize / MaxOrthographicSize;
        RefreshXPos();
        RefreshYPos();
    }

    public void OnHorizontalScroll(float value)
    {
        hScrollBar.value = value;
        RefreshXPos();
    }
    protected void RefreshXPos()
    {
        Vector3 pos = proxyCamera.transform.position;
        pos.x = Mathf.Lerp(MinPos, MaxPos, hScrollBar.value);
        proxyCamera.transform.position = pos;
    }

    public void OnVerticalScroll(float value)
    {
        vScrollBar.value = value;
        RefreshYPos();
    }
    protected void RefreshYPos()
    {
        Vector3 pos = proxyCamera.transform.position;
        pos.z = Mathf.Lerp(MinPos, MaxPos, vScrollBar.value);
        proxyCamera.transform.position = pos;
    }
}
