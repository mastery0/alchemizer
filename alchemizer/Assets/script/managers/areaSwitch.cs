using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class areaSwitch : MonoBehaviour
{
    public string targetArea;
    public Vector2 targetCoords;
    public GameObject menuPanel;
    [HideInInspector] public UnityEngine.Camera cam;

    public RectTransform Canvasrect;

    public TMP_Text areaTxt;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (Vector3.Distance(player.instance.transform.position, gameObject.transform.position) > 2)
        {
            Canvasrect.gameObject.SetActive(false);
        }
        else
        {
            Canvasrect.gameObject.SetActive(true);
        }
        if (Canvasrect.gameObject.activeSelf) positionMenu();
        areaTxt.text = "Enter "+targetArea;
    }
    public void onClick ()
    {
        areaManager.instance.switchToArea(targetArea);
        player.instance.prb.linearVelocity = Vector2.zero;
        player.instance.transform.position = targetCoords;
    }

    public void positionMenu()
    {
        Vector2 pos = cam.WorldToScreenPoint(transform.position);
        menuPanel.GetComponent<RectTransform>().position = pos + new Vector2(0, 250f);
    }
}
