using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class bossBar : fillBar
{
    public new static bossBar instance;

    [Header("boss bar")]
    public GameObject barRoot;
    public float hideDelay = 1.5f;

    void Awake()
    {
        instance = this;
        if (barRoot != null) barRoot.SetActive(false);
    }

    public void show()
    {
        if (barRoot != null) barRoot.SetActive(true);
    }

    public void hide()
    {
        if (barRoot != null) barRoot.SetActive(false);
    }

    /*public void hideDelayed()
    {
        StartCoroutine(hideAfterDelay());
    }

    IEnumerator hideAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);
        hide();
    }*/
}