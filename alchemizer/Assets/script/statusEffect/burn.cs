using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class burn : statusEffect
{
    public float damagePerSecond;
    public override void onApply()
    {
        remainingDuration = duration;
    }
    public override void onTick()
    {
        if(gameObject.CompareTag("Player"))
        {
            gameObject.GetComponent<player>().takeDamage(damagePerSecond * Time.deltaTime);
        }
        else if(gameObject.CompareTag("Enemy"))
        {
            gameObject.GetComponent<enemy>().takeDamage(damagePerSecond * Time.deltaTime);
        }
        remainingDuration -= Time.deltaTime;
    }
    public override void onFinish()
    {
        Destroy(this);
    }
}
