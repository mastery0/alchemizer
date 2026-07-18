using UnityEngine;
using System.Collections.Generic;
using UnityEditor;


public class healManager : MonoBehaviour
{
    public static healManager instance;
    public List<potion> potionDB=new();
    public bool canSwap=false;
    public int remainingUse;
    public potion equipped;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        remainingUse = equipped.potionAmount;
        foreach (potion potion in potionDB)
        {
            Debug.Log(potion.name);
        }
    }
    public void searchEquipped() {
        foreach (potion potion in potionDB)
        {
            if (potion.isEquipped) { equipped = potion; break; }
        }
    }
    
}
