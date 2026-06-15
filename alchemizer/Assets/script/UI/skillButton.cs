using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class skillButton : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public skillSO skill;
    public TMP_Text skillName;
    public GameObject overMenu;
    private overMenuScript menuScript;
    private void Start()
    {
        skillName.text = skill.skillName;
        menuScript=overMenu.GetComponent<overMenuScript>();
    }
    public void OnClick()
    {
        skill.Unlock();
    }
    public void OnPointerEnter(PointerEventData data)
    {
        overMenu.SetActive(true);
    }
    public void OnPointerExit(PointerEventData data)
    {
        menuScript.despawn();
    }
}
