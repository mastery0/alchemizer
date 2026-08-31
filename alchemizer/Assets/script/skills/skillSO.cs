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
        public Sprite skillBG;
        public Sprite skillMenuImg;


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

    [Header("cost settings")]

    public Vector2Int costSprites=new Vector2Int(-1,-1); //if y=-1 only 1 essence
    public int cost1;
    public int cost2;
    [HideInInspector]
    public bool isUnlocked = false;

    private void OnEnable()
    {
        //0 air 1 fire 2 water 3 light 4 dark
        int n = 0;
        foreach (essence s in essences)
        {
            n++;
            switch (s.type)
            {
                case essenceTypes.air:
                    if (costSprites.x == -1) { costSprites.x = 0; cost1 = s.amount; }
                    else { costSprites.y = 0; cost2 = s.amount; }
                    break;
                case essenceTypes.fire:
                    if (costSprites.x == -1) { costSprites.x = 1; cost1 = s.amount; }
                    else { costSprites.y = 1; cost2 = s.amount; }
                    break;
                case essenceTypes.water:
                    if (costSprites.x == -1) { costSprites.x = 2; cost1 = s.amount; }
                    else { costSprites.y = 2; cost2 = s.amount; }
                    break;
                case essenceTypes.light:
                    if (costSprites.x == -1) { costSprites.x = 3; cost1 = s.amount; }
                    else { costSprites.y = 3; cost2 = s.amount; }
                    break;
                case essenceTypes.dark:
                    if (costSprites.x == -1) { costSprites.x = 4; cost1 = s.amount; }
                    else
                    { costSprites.y = 4; cost2 = s.amount; }
                    break;
            }
        }
        if (n == 1) costSprites.y = -1;
        isUnlocked = false;
        if (skillID == 0) {costSprites.x = -1;isUnlocked = true;}
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
