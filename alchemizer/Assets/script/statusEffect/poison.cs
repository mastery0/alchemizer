using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using JetBrains.Annotations;
public class poison : statusEffect
{
    public float damagePerSecond;
    public float healDecrease;

    private void Awake()
    {
        effectName = "Poison";
        duration = 10f;
        damagePerSecond = 3;
        healDecrease = 0.3f;
    }
    public override void onApply()
    {
        remainingDuration = duration;
        if (gameObject.CompareTag("Player"))
        {
            gameObject.GetComponent<player>().healMult-= healDecrease;
        }
        else if (gameObject.CompareTag("Enemy"))
        {
            gameObject.GetComponent<enemy>().healpercent -= healDecrease;
        }
    }

    public override void onTick()
    {
        damage(damagePerSecond*Time.deltaTime);
        remainingDuration -= Time.deltaTime;
    }
    public override void onFinish()
    {
        if (gameObject.CompareTag("Player"))
        {
            gameObject.GetComponent<player>().healMult += healDecrease;
        }
        else if (gameObject.CompareTag("Enemy"))
        {
            gameObject.GetComponent<enemy>().healpercent += healDecrease;
        }
        Destroy(this);
    }
}

