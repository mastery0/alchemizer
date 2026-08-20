using UnityEngine;
using System.Collections.Generic;


[DefaultExecutionOrder(-10)]
public class healManager : MonoBehaviour
{
    public static healManager instance;
    public List<potion> potionDB=new();
    public int remainingUse;
    public potion equipped;
    private readonly List<string> defaultUnlockedPotionIDs = new List<string>();
    private string defaultEquippedPotionID;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        foreach (potion potion in potionDB)
        {
            if (potion.isUnlocked) defaultUnlockedPotionIDs.Add(potion.potionID);
        }

        if (equipped == null && potionDB.Count > 0) equipped = potionDB[0];
        if (equipped != null) defaultEquippedPotionID = equipped.potionID;

        if (equipped != null && equipped.isUnlocked)
        {
            equipped.isEquipped = true;
            remainingUse = equipped.potionAmount;
        }
    }

    public bool unlockPotion(string potionID)
    {
        if (string.IsNullOrEmpty(potionID)) return false;

        foreach (potion potion in potionDB)
        {
            if (potion.potionID != potionID) continue;

            potion.isUnlocked = true;
            Debug.Log("Potion unlocked: " + potion.potionName);
            return true;
        }

        Debug.LogWarning("Potion reward could not be found: " + potionID);
        return false;
    }

    public string[] getUnlockedPotionIDs()
    {
        List<string> unlockedPotionIDs = new List<string>();
        foreach (potion potion in potionDB)
        {
            if (potion.isUnlocked) unlockedPotionIDs.Add(potion.potionID);
        }
        return unlockedPotionIDs.ToArray();
    }

    public string getEquippedPotionID()
    {
        return equipped != null ? equipped.potionID : "";
    }

    public void applySavedPotionState(string[] unlockedPotionIDs, string equippedPotionID)
    {
        if (unlockedPotionIDs == null) return;

        foreach (potion potion in potionDB)
        {
            potion.isUnlocked = System.Array.IndexOf(unlockedPotionIDs, potion.potionID) >= 0;
            potion.isEquipped = false;
        }

        if (!string.IsNullOrEmpty(equippedPotionID))
        {
            foreach (potion potion in potionDB)
            {
                if (potion.potionID != equippedPotionID || !potion.isUnlocked) continue;
                equipped = potion;
                break;
            }
        }

        if (equipped == null || !equipped.isUnlocked)
        {
            foreach (potion potion in potionDB)
            {
                if (!potion.isUnlocked) continue;
                equipped = potion;
                break;
            }
        }

        if (equipped != null)
        {
            equipped.isEquipped = true;
            remainingUse = equipped.potionAmount;
        }
    }

    public void resetPotionState()
    {
        foreach (potion potion in potionDB)
        {
            potion.isUnlocked = defaultUnlockedPotionIDs.Contains(potion.potionID);
            potion.isEquipped = false;
        }

        equipped = null;
        foreach (potion potion in potionDB)
        {
            if (potion.potionID != defaultEquippedPotionID || !potion.isUnlocked) continue;
            equipped = potion;
            break;
        }

        if (equipped != null)
        {
            equipped.isEquipped = true;
            remainingUse = equipped.potionAmount;
        }
    }
    public void searchEquipped() {
        foreach (potion potion in potionDB)
        {
            if (potion.isEquipped && potion.isUnlocked) { equipped = potion; break; }
        }
    }
    
}
