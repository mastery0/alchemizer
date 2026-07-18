using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class potionButton : MonoBehaviour
{
    public void OnClick()
    {
        foreach (potion potion in healManager.instance.potionDB)
        {
            if (GetComponentInChildren<TMP_Text>().text != potion.potionName) continue;
            Debug.Log("clicked");
            potionUI.instance.potionNameText.text = potion.potionName;
            potionUI.instance.potionDescription.text = potion.description;
            potionUI.instance.potionImage = potion.potionIMG;
            //gameObject.GetComponent<Image>().sprite = potion.potionIMG.sprite;
            potionUI.instance.potionAmount.text = "Amount: "+potion.potionAmount.ToString();
        }
    }
}
