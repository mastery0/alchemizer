using UnityEngine;

public class weakness : statusEffect
{
    public float damageReduction;
    public override void onApply()
    {
        remainingDuration = duration;
        if (gameObject.CompareTag("Player"))
        {
            gameObject.GetComponent<player>().attackDamage -= damageReduction;
        }
        else if (gameObject.CompareTag("Enemy"))
        {
            gameObject.GetComponent<enemy>().damage -= damageReduction;
        }
    }
    public override void onTick()
    {

        remainingDuration -= Time.deltaTime;
    }
    public override void onFinish()
    {
        if (gameObject.CompareTag("Player"))
        {
            gameObject.GetComponent<player>().attackDamage += damageReduction;
        }
        else if (gameObject.CompareTag("Enemy"))
        {
            gameObject.GetComponent<enemy>().damage += damageReduction;
        }
        Destroy(this);
    }
}
