using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
public class checkPoint : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler
{
    public static checkPoint instance;


    public Vector2 checkPointPos;
    public GameObject menuPanel;
    public UnityEngine.Camera cam;
    public RectTransform Canvasrect;


    public GameObject potionMenu;
    public TMP_Text equipTxt;
    public Button equipBtn;

    [HideInInspector]public string selectedPotion;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        checkPointPos = transform.position;
    }
    private void Update()
    {
        if (Vector3.Distance(player.instance.transform.position, gameObject.transform.position) > 2)
        {
            Canvasrect.gameObject.SetActive(false);
        }
        if (Canvasrect.gameObject.activeSelf) positionMenu();
        if (selectedPotion != null)
        {
            foreach (var potion in healManager.instance.potionDB)
            {
                if (potion.isEquipped)
                {
                    if (potion.potionID == selectedPotion)
                    {
                        equipBtn.interactable = false;
                        equipTxt.text = "equipped";
                    }
                    else
                    {
                        equipBtn.interactable = true;
                        equipTxt.text = "equip";
                    }
                }
            }
        }
    }
    public void OnPointerEnter(PointerEventData data)
    {
        if (Vector3.Distance(player.instance.transform.position, gameObject.transform.position) > 2)
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            return;
        }
        gameObject.GetComponent<SpriteRenderer>().color = Color.green;
    }

    public void OnPointerExit(PointerEventData data)
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }
    public void OnPointerDown(PointerEventData data)
    {
        if(Vector3.Distance(player.instance.transform.position, gameObject.transform.position) > 2)
        {
            return;
        }
        Canvasrect.gameObject.SetActive(true);
        positionMenu();
    }
    public void positionMenu()
    {
        Vector2 pos=cam.WorldToScreenPoint(checkPointPos);
        menuPanel.GetComponent<RectTransform>().position=pos+new Vector2(0,250f);
    }

    public void onSaveClick()
    {
        player.instance.respawnAltar = checkPointPos;
        player.instance.respawnScene = gameObject.scene.buildIndex;
        saveManager.instance.save();
        Debug.Log("Checkpoint saved at: " + checkPointPos);
    }
    public void onPotionClick()
    {
        potionMenu.gameObject.SetActive(true);
    }
    public void onExitClick()
    {
        potionMenu.SetActive(false);
    }
    public void onEquipClick()
    {
        foreach(potion potion in healManager.instance.potionDB)if(potion.potionID==selectedPotion) healManager.instance.equipped = potion;
    }
}
