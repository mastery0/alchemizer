using System.Collections.Generic;
using UnityEngine;
using static essenceManager;

[CreateAssetMenu(menuName = "Skills/Skill")]
public class skillSO : ScriptableObject
{
    [System.Serializable]
    public struct essence
    {
        public essenceManager.essenceTypes type;
        public int amount;
    }
    [Header("skill info")]
    public int skillID;
    public string skillName;
    public string skillDescription;
    public skillSO[] requiredSkill;
    public bool skipRequirement;

    [Header("statsUP")]
    public float hpMult=1f;
    public float atkMult = 1f;
    public float dashMult = 1f;
    public bool dashInv = false;
    public float essenceMult = 1f;
    public float rangMult = 1f;
    public essence[] essences = null;

    [HideInInspector]
    public bool isUnlocked = false;

    private void OnEnable()
    {
        isUnlocked = false;
    }
    public void Unlock()
    {
        if (canUnlock()&&!isUnlocked)
        {
            foreach (essence s in essences)
            {
                essenceManager.instance.essenceInv[s.type] -= s.amount;
            }
            player.instance.maxHp *= hpMult;
            if(hpMult!=1)player.instance.hp=player.instance.maxHp;
            player.instance.attackDamage *= atkMult;
            player.instance.dashForce *= dashMult;
            player.instance.attackRange *= rangMult;
            if (dashInv) player.instance.dashInvincibility = true;
            Debug.Log("unlocked:" + skillName);
            isUnlocked = true;
            //essenceMult to do
        }
        else if(isUnlocked)
        {
            Debug.Log("Already Unlocked");
        }

    }
    public bool canUnlock()
    {
        bool unlocked = true;
        if (!skipRequirement)
        {
            foreach (skillSO parent in requiredSkill)
            {
                if (!parent.isUnlocked)
                {
                    Debug.Log("required:" + parent.skillName);
                    unlocked = false;
                }
            }
        }
        foreach( essence s in essences)
        {
            if (essenceManager.instance.essenceInv[s.type] < s.amount)
            {
                Debug.Log("required:" + s.type.ToString() +" essences,you have"+ essenceManager.instance.essenceInv[s.type].ToString()+"essences");
                unlocked = false;
            }
        }
        return unlocked;
    }
}
