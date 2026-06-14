using System.Collections.Generic;
using UnityEngine;

public class essenceManager : MonoBehaviour
{
    public static essenceManager instance;
    public enum essenceTypes
    {
        air,
        fire,
        water,
        light,
        dark
    }//if modified modify dictionary,drop() in enemy
    public Dictionary<essenceTypes, int> essenceInv = new Dictionary<essenceTypes, int> 
    {
        [essenceTypes.air]=0,
        [essenceTypes.fire]=0,
        [essenceTypes.water]=0,
        [essenceTypes.light]=0,
        [essenceTypes.dark]=0,
        [essenceTypes.light]=0,
    };
    private void Awake()
    {
        instance = this;
    }
    public void modifyAmount(essenceTypes type,int amount)
    {
        essenceInv[type] += amount;
    }

    private void Update()
    {
        foreach(var essence in essenceInv)
        {
            Debug.Log(essence);
        }
    }
}
