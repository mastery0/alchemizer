using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class bleed : statusEffect
{
    public float damagePerSecond;
    public float speedDmgMult;// This is a multiplier for the damage taken based on the speed of the character

    private void Awake()
    {
        effectName = "Bleed";
        duration = 8f;
        damagePerSecond = 2;
        speedDmgMult = 0.5f;
    }
    public override void onApply()
    {
        remainingDuration = duration;
    }
    public override void onTick()
    {
        damage((damagePerSecond + (gameObject.GetComponent<Rigidbody2D>().linearVelocity.magnitude * speedDmgMult)) * Time.deltaTime);
        remainingDuration -= Time.deltaTime;
    }
    public override void onFinish()
    {
        Destroy(this);
    }
}

