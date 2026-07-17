using UnityEngine;
using UnityEngine.UI;
public class potion : MonoBehaviour
{
    public string potionID;
    public string potionName;
    public string description;
    public Image potionIMG;
    public float healAmount;
    public int potionAmount;
    public bool isUnlocked;
    public bool isEquipped;
    public virtual void OnUse() { }
    public virtual void Init() { }
}
