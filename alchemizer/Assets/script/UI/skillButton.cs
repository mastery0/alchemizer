using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class skillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public skillSO skill;
    public TMP_Text skillName;
    private void Start()
    {
        skillName.text = skill.skillName;
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
        ConjureMenu();
    }
    public void OnPointerExit(PointerEventData data)
    {
        skillTreeManager.overMenu.gameObject.SetActive(false);
    }
    void ConjureMenu()
    {
        skillTreeManager.overMenu.transform.position = transform.position + new Vector3(200, -10, 0);
        skillTreeManager.overMenu.skillName.text = skill.name;
        skillTreeManager.overMenu.skillDescription.text = skill.skillDescription;
        skillTreeManager.overMenu.Spawn();
    }
}
