using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[DefaultExecutionOrder(-1000)]
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
    }
    public void searchEquipped() {
        foreach (potion potion in potionDB)
        {
            if (potion.isEquipped) { equipped = potion; break; }
        }
    }
    
}
