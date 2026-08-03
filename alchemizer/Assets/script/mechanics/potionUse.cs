using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class potionUse : MonoBehaviour
{
    public Image potionImage;
    public TMP_Text uses;
    private void Update()
    {
        foreach(var potion in healManager.instance.potionDB)
        {
            if(potion == healManager.instance.equipped)
            {
                potionImage.sprite = potion.potionIMG;
                uses.text = healManager.instance.remainingUse.ToString();
            }
        }
    }
}
