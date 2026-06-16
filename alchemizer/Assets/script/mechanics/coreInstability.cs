using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class coreInstability : MonoBehaviour
{
    //pressure goes up as the player continue hitting enemy and taking dmg and slowly goes down when he stop,reset on heal
    public player player;
    public static coreInstability instance;


    public int currentPressure = 0;
    public int pressureDelta = 10;
    public int maxPressure = 100;


    public float atkMod=1.15f;
    public float defMod=0.90f;


    public bool glassCannon = false;
    bool applied = false;
    public int lastTier = 0;
    private bool done1, done2;
    private bool isDecreasing;


    private void Start()
    {
        instance = this;
    }
    private void Update()
    {
        int newTier=0;
        currentPressure=Mathf.Clamp(currentPressure, 0, maxPressure);
        player.timeSinceHit += Time.deltaTime;
        player.timeSinceAttack += Time.deltaTime;
        if ((player.timeSinceAttack > 3f && player.timeSinceHit > 3f)&&!isDecreasing) StartCoroutine(decreasePressure());
        //if heal pressure=0;
        if (currentPressure >= 30)
        {
            newTier = 1;
        }
        if (currentPressure >= 60)
        {
            newTier = 2;
        }
        if (newTier != lastTier)
        {
            // Remove old multiplayers
            Debug.Log(newTier);
            if (lastTier >= 1) { player.attackDamage /= atkMod; player.defense /= defMod; }
            if (lastTier >= 2) { player.attackDamage /= atkMod; player.defense /= defMod; }

            // Apply new multiplayers
            if (newTier >= 1) { player.attackDamage *= atkMod; player.defense *= defMod; }
            if (newTier >= 2) { player.attackDamage *= atkMod; player.defense *= defMod; }

            lastTier = newTier;
        }
        if (glassCannon&&!applied)
        {
            applied= true;
            atkMod = 1.30f;
            defMod = 0.75f;
        }
    }
    private IEnumerator decreasePressure()
    {
        isDecreasing = true;
        currentPressure -= pressureDelta;
        yield return new WaitForSeconds(0.5f);
        isDecreasing=false;
    }
}

