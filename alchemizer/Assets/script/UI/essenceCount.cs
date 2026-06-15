using UnityEngine;
using TMPro;
public class essenceCount : MonoBehaviour
{
    public TMP_Text airEssenceText;
    public TMP_Text fireEssenceText;
    public TMP_Text waterEssenceText;
    public TMP_Text lightEssenceText;
    public TMP_Text darkEssenceText;
    public void Update()
    {
        airEssenceText.text = "air:"+essenceManager.instance.essenceInv[essenceManager.essenceTypes.air].ToString();
        fireEssenceText.text = "fire:"+essenceManager.instance.essenceInv[essenceManager.essenceTypes.fire].ToString();
        waterEssenceText.text = "water:"+essenceManager.instance.essenceInv[essenceManager.essenceTypes.water].ToString();
        lightEssenceText.text = "light:"+essenceManager.instance.essenceInv[essenceManager.essenceTypes.light].ToString();
        darkEssenceText.text = "dark:"+essenceManager.instance.essenceInv[essenceManager.essenceTypes.dark].ToString();
    }
}
