using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class YellDetector : MonoBehaviour 
{
    protected static YellDetector _instance = null;
    public static YellDetector Get()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType <YellDetector>();
        }
        return _instance;
    }

    protected AudioClip clip = null;

    public bool IsListening()
    {
        return (!string.IsNullOrEmpty(ActiveMic) && Microphone.IsRecording(ActiveMic));
    }
    public string ActiveMic
    {
        get;
        protected set;
    }
    public bool listenOnStart = true;
    [Range(0f, 1f)]
    public float yellThreshold = 0.5f;

    [SerializeField]
    float yellSampleRate = 0.1f;

    [HideInInspector]
    public float average = 0f;
    [HideInInspector]
    public float max = 0f;

    [System.Serializable]
    public class YellEvent : UnityEvent<float> { }
    public YellEvent OnYellDetected;

    public void StartListening()
    {
        if (!IsListening() 
            && !string.IsNullOrEmpty(ActiveMic))
        {
            int minFreq;
            int maxFreq;
            Microphone.GetDeviceCaps(ActiveMic, out minFreq, out maxFreq);
            if (minFreq == 0 && maxFreq == 0)
            {
                // According to the documentation, if both are 0 we can support any sampling rate.
                minFreq = 44100;
            }
            clip = Microphone.Start(ActiveMic, true, 10, minFreq);
            StartCoroutine(ProcessAudio());
        }
    }

    public void StopListening()
    {
        if (IsListening())
        {
            StopAllCoroutines();
            Microphone.End(ActiveMic);
            clip = null;
        }
    }

    void OnEnable()
    {
        _instance = this;

        if (Microphone.devices.Length > 0)
        {
            if (string.IsNullOrEmpty(ActiveMic))
            {
                ActiveMic = Microphone.devices[0];
            }
            if (listenOnStart)
            {
                StartListening();
            }
        }
    }

    void OnDisable()
    {
        if (_instance == this)
        {
            _instance = null;
        }
        OnYellDetected.RemoveAllListeners();
        StopListening();
    }

    protected IEnumerator ProcessAudio()
    {
        int pos = 0;
        int lastSample = 0;
        int diff;
        WaitForSeconds pause = new WaitForSeconds(yellSampleRate);
        while (this.IsListening())
        {
            pos = Microphone.GetPosition(ActiveMic);
            diff = pos - lastSample;
            if (diff > 0)
            {
                float[] samples = new float[diff*clip.channels];
                clip.GetData(samples, lastSample);
                max = 0f;
                average = 0f;
                for (int i = 0; i < samples.Length; i++)
                {
                    average += Mathf.Abs(samples[i]);
                    max = Mathf.Max(Mathf.Abs(samples[i]), max);
                }
                average /= samples.Length;

                if (max >= yellThreshold)
                {
                    OnYellDetected.Invoke(average);
                }
            }
            lastSample = pos;

            yield return pause;
        }
    }

	// Update is called once per frame
	void Update () 
    {
        DebugConsole.SetText("YellDetector", "Avg:(" + average.ToString("F3") + ") Max:(" + max.ToString("F3") + ")");
	}
}
