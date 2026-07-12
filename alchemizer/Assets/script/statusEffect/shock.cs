using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class shock : statusEffect
{
    public float damagePerSecond;
    public float atkSpeedRed;//reduction in attack speed
    public override void onApply()
    {
        remainingDuration = duration;
        if (gameObject.CompareTag("Player"))
        {
            gameObject.GetComponent<player>().attackCooldown += atkSpeedRed;
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
            gameObject.GetComponent<player>().attackCooldown -= atkSpeedRed;
        }
        Destroy(this);
    }
}

