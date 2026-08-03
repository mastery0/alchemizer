using UnityEngine;
using System.Collections.Generic;
using UnityEditor;


public class healManager : MonoBehaviour
{
    public static healManager instance;
    public List<potion> potionDB=new();
    public int remainingUse;
    public potion equipped;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        potionDB[0].isEquipped= true;
        remainingUse = equipped.potionAmount;
    }
    public void searchEquipped() {
        foreach (potion potion in potionDB)
        {
            if (potion.isEquipped) { equipped = potion; break; }
        }
    }
    
}
