using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class bleed : statusEffect
{
    public float damagePerSecond;
    public float speedDmgMult;// This is a multiplier for the damage taken based on the speed of the character
    public override void onApply()
    {
        remainingDuration = duration;
    }
    public override void onTick()
    {
        if (gameObject.CompareTag("Player"))
        {
            gameObject.GetComponent<player>().takeDamage((damagePerSecond+gameObject.GetComponent<Rigidbody2D>().linearVelocity.magnitude * speedDmgMult) * Time.deltaTime);
        }
        else if (gameObject.CompareTag("Enemy"))
        {
            gameObject.GetComponent<enemy>().takeDamage((damagePerSecond + gameObject.GetComponent<Rigidbody2D>().linearVelocity.magnitude * speedDmgMult) * Time.deltaTime);
        }
        remainingDuration -= Time.deltaTime;
    }
    public override void onFinish()
    {
        Destroy(this);
    }
}

