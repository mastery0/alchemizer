using TMPro;
using UnityEngine;

public class potionButton : MonoBehaviour
{
    public void OnClick()
    {
        foreach (potion potion in healManager.instance.potionDB)
        {
            if (GetComponentInChildren<TMP_Text>().text != potion.potionName) continue;
            Debug.Log("clicked");
            potionUI.instance.potionNameText.text = potion.name;
            potionUI.instance.potionDescription.text = potion.description;
            potionUI.instance.potionImage = potion.potionIMG;
            potionUI.instance.potionAmount.text = potion.potionAmount.ToString();
        }
    }
}
