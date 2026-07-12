using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class poison : statusEffect
{
    public float damagePerSecond;
    public float healDecrease;
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
        if (gameObject.CompareTag("Player"))
        {
            gameObject.GetComponent<player>().takeDamage(damagePerSecond * Time.deltaTime);
        }
        else if (gameObject.CompareTag("Enemy"))
        {
            gameObject.GetComponent<enemy>().takeDamage(damagePerSecond * Time.deltaTime);
        }
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

