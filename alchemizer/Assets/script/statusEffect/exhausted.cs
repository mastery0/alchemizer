using UnityEngine;

public class exhausted : statusEffect
{
    // This status effect increases the rate of pressure loss and decreases the rate of pressure gain for the player.
    public int decreasing;
    public int increasing;
    public override void onApply()
    {
        remainingDuration = duration;
        if (gameObject.CompareTag("Player"))
        {
            gameObject.GetComponent<coreInstability>().pressureMinusDelta += decreasing;
            gameObject.GetComponent<coreInstability>().pressurePlusDelta -= increasing;
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
            gameObject.GetComponent<coreInstability>().pressureMinusDelta -= decreasing;
            gameObject.GetComponent<coreInstability>().pressurePlusDelta += increasing;
        }

        Destroy(this);
    }
}
