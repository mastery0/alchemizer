using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class skillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public skillSO skill;
    //private Image skillIMG;
    //public Sprite skillBG;
    //public TMP_Text skillName;
    private void Awake()
    {
        //skillIMG = GetComponent<Image>();
    }
    private void Start()
    {
        //skillName.text = skill.skillName;
        //skillIMG.sprite=skillBG;
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
        //costPrefab.SetActive(false);
        skillTreeManager.overMenu.gameObject.SetActive(false);
    }
    void ConjureMenu()
    {
        skillTreeManager.overMenu.transform.position = transform.position + new Vector3(200, -10, 0);
        skillTreeManager.overMenu.skillName.text = skill.skillName;
        skillTreeManager.overMenu.skillDescription.text = skill.skillDescription;
        skillTreeManager.overMenu.skillImage.sprite = skill.skillMenuImg;
        skillTreeManager.overMenu.skill = skill;
        if (skill.costSprites.y == -1 && skill.costSprites.x == -1)
        {
            skillTreeManager.overMenu.costIMG1.gameObject.SetActive(false);
            skillTreeManager.overMenu.costTXT1.gameObject.SetActive(false);
            skillTreeManager.overMenu.costIMG2.gameObject.SetActive(false);
            skillTreeManager.overMenu.costTXT2.gameObject.SetActive(false);
        }
        else
        if (skill.costSprites.y == -1)
        {
            skillTreeManager.overMenu.costIMG1.gameObject.SetActive(true);
            skillTreeManager.overMenu.costTXT1.gameObject.SetActive(true);
            skillTreeManager.overMenu.costIMG2.gameObject.SetActive(false);
            skillTreeManager.overMenu.costTXT2.gameObject.SetActive(false);
            skillTreeManager.overMenu.costIMG1.sprite = skillTreeManager.overMenu.essencesSprites[skill.costSprites.x];
            skillTreeManager.overMenu.costTXT1.text = skill.cost1.ToString();
        }
        else
        {
            skillTreeManager.overMenu.costIMG1.gameObject.SetActive(true);
            skillTreeManager.overMenu.costTXT1.gameObject.SetActive(true);
            skillTreeManager.overMenu.costIMG2.gameObject.SetActive(true);
            skillTreeManager.overMenu.costTXT2.gameObject.SetActive(true);
            skillTreeManager.overMenu.costIMG1.sprite = skillTreeManager.overMenu.essencesSprites[skill.costSprites.x];
            skillTreeManager.overMenu.costTXT1.text = skill.cost1.ToString();
            skillTreeManager.overMenu.costIMG2.sprite = skillTreeManager.overMenu.essencesSprites[skill.costSprites.y];
            skillTreeManager.overMenu.costTXT2.text = skill.cost2.ToString();
        }
        skillTreeManager.overMenu.Spawn();
    }
}
