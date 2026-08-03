using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(100)]
public class potionButton : MonoBehaviour
{
    public bool usedForSwapping = false;
    public checkPoint checkPoint;
    public potion assignedPotion;

    public void Setup(potion potion)
    {
        assignedPotion = potion;
        GetComponent<Image>().sprite = potion.potionIMG;
        checkPoint = GetComponentInParent<checkPoint>();
    }

    public void OnClick()
    {
        if (assignedPotion == null) return;

        potionUI.instance.potionNameText.text = assignedPotion.potionName;
        potionUI.instance.potionDescription.text = assignedPotion.description;
        potionUI.instance.potionImage.sprite = assignedPotion.potionIMG;
        potionUI.instance.potionImage.color = Color.green;
        potionUI.instance.potionAmount.text = "Amount: " + assignedPotion.potionAmount.ToString();

        if (usedForSwapping && checkPoint != null)
        {
            checkPoint.selectedPotion = assignedPotion.potionID;
        }
    }
}