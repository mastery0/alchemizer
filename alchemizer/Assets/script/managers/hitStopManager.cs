using System.Collections;
using UnityEngine;

public class hitStopManager : MonoBehaviour
{
    public static hitStopManager instance;
    private void Awake()
    {
        instance = this;
    }
    public void stopTime(float time,float slowScale = 0.02f)
    {
        Time.timeScale = 1f;
        StartCoroutine(hitStop(time, slowScale));
    }
    public IEnumerator hitStop(float time,float slowScale)
    {
        Time.timeScale=slowScale;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1f;
        yield return null;
    }
}
