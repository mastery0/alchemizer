using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class skillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public skillSO skill;
    public TMP_Text skillName;
    public GameObject overMenu;
    private overMenuScript menuScript;
    private void Start()
    {
        skillName.text = skill.skillName;
        menuScript = overMenu.GetComponent<overMenuScript>();
    }
    void LateUpdate()
    {
        Vector3 parentScale = transform.parent.lossyScale;

        transform.localScale = new Vector3(
            1f / parentScale.x,
            1f / parentScale.y,
            1f / parentScale.z
        );
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
