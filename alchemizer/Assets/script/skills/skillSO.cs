using System.Collections.Generic;
using Unity.VisualScripting;
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
        public essence[] essences = null;


    [Header("statsUP")]
        public float hpMult=1f;
        public float atkMult = 1f;
        public float dashMult = 1f;
        public float essenceMult = 1f;
        public float rangMult = 1f;
        public float atkCDmult = 1f;
        public float speedMult = 1f;
        public float dashCDmult = 1f;
        public int dashCount = 0;
        public float iFrames = 0; 
        public float healMult = 0f;
    [Header("unlocks")]
        public bool dashInv = false;
        public bool coreinstability=false;
        public bool glassCannon=false;
        public bool airDash = false;
        public bool glider=false;
        public bool enemyHeals=false;
    [HideInInspector]
    public bool isUnlocked = false;

    private void OnEnable()
    {
        isUnlocked = false;
    }
    public void Unlock()
    {
        if (canUnlock())
        {
            payEssences();
            applyEffects();
        }
    }
    public bool canUnlock()
    {
        bool unlocked = true;
        if (isUnlocked)
        {
            Debug.Log("already unlocked");
            unlocked = false;
        }
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
                Debug.Log("required: " + s.type.ToString() +" essences,you have "+ essenceManager.instance.essenceInv[s.type].ToString()+" essences");
                unlocked = false;
            }
        }
        if (skillID == 6) //this skill require dash if that skill id is changed change this accordingly
        {
            if (!player.instance.hasDash)
            {
                Debug.Log("dash Required");
                unlocked=false;
            }
        }
        return unlocked;
    }
    public void payEssences()
    {
        foreach (essence s in essences)
        {
            essenceManager.instance.essenceInv[s.type] -= s.amount;
        }
    }
    public void applyEffects()
    {
        if (isUnlocked) return;
        player.instance.maxHp *= hpMult;
        if (hpMult != 1) player.instance.hp = player.instance.maxHp;
        player.instance.attackDamage *= atkMult;
        player.instance.dashForce *= dashMult;
        player.instance.attackRange *= rangMult;
        player.instance.attackCooldown /= atkCDmult;
        player.instance.moveSpeed *= speedMult;
        player.instance.dashCooldown /= dashCDmult;
        player.instance.dashCount += dashCount;
        player.instance.iFrames += iFrames;
        player.instance.healMult += healMult;
        if (dashInv) player.instance.dashInvincibility = true;
        if (coreinstability) player.instance.coreInstability = true;
        if (glassCannon) coreInstability.instance.glassCannon = true;
        if (airDash) player.instance.airDash = true;
        if (glider) player.instance.hasGlider = true;
        if (enemyHeals) player.instance.enemiesHeal = true;
        Debug.Log("unlocked:" + skillName);
        isUnlocked = true;
        //essenceMult to do
    }
}
