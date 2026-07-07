using System.Collections.Generic;
using UnityEngine;

public class inventory : MonoBehaviour
{
    public static inventory instance;
    public int maxSlots = 20;
    public List<itemStack> items = new List<itemStack>();
    public void Awake()
    {
        instance = this;
    }
    public bool addItem(itemData item,int amount)
    {
        if (item==null||amount<=0) return false;
        int remaining=amount;

        foreach (itemStack stack in items)
        {
            if (stack.item != item) continue;
            int spaceLeft = item.maxStack - stack.amount;
            remaining-=Mathf.Min(spaceLeft, remaining);
            if(remaining<=0) break;
        }

        int freeSlots = maxSlots - items.Count;
        int neededStacks = Mathf.CeilToInt((float)remaining / item.maxStack);
        if(neededStacks > freeSlots) return false;

        remaining = amount;

        foreach (itemStack stack in items)
        {
            if (stack.item != item) continue;
            int spaceLeft = item.maxStack - stack.amount;
            int toAdd = Mathf.Min(spaceLeft, remaining);
            stack.amount += toAdd;
            remaining -= toAdd;
        }


        while(remaining > 0)
        {
            int stackAmount = Mathf.Min(remaining, item.maxStack);
            items.Add(new itemStack { item = item, amount = stackAmount });
            remaining -= stackAmount;
        }
        return true;
    }
}

