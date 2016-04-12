using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections;

public abstract class TrunkNetworkingBase : MonoBehaviour
{
    public UnityEvent OnSessionEstablished;
    public UnityEvent OnResetImminent;
    public UnityEvent OnGameWin;

    public abstract void Begin();
    public virtual IEnumerator SetUpSession(int citySeed, int pathSeed)
    {
        // We have nothing to set up / syncronize, so just do nothing for now.
        var wfs = new WaitForSeconds(1f);
        for (int i = 0; i < 10; i++)
        {
            yield return wfs;
            Log("Setting up game: " + i * 10f + "% complete");
        }
        Log("Setting up game: Done!");
        OnSessionEstablished.Invoke();
    }

    public UnityEngine.UI.Text statusText = null;
    public void Log(string msg, bool asError = false)
    {
        statusText.text += "\n" + (asError ? "<color=\"red\">ERROR:</color> " : string.Empty) + msg;
        DebugConsole.SetText("NetworkStatus", msg);
    }

    public void Restart(string msg = null)
    {
        OnResetImminent.Invoke();

        StopAllCoroutines();
        StartCoroutine(RestartingIn(msg));
    }
    private IEnumerator RestartingIn(string msg)
    {
        if (!string.IsNullOrEmpty(msg))
        {
            Log(msg + "  Resetting in...", true);
        }
        else
        {
            Log("Resetting in...");
        }

        for (int sec = 5; sec > 0; sec--)
        {
            Log(sec + "...");
            yield return new WaitForSeconds(1f);
        }

        SceneManager.LoadScene("Trunk", LoadSceneMode.Single);
    }
}
