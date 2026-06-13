using UnityEngine;

public class slime : enemy
{
    protected override void Awake()
    {
        base.Awake();
        direction = 1;
    }
    void FixedUpdate()
    {
        Behaviour();
    }
    protected void Behaviour()
    {
        if (Mathf.Abs(player.transform.position.x - transform.position.x) <= range)
        {
            groundFollow();
        }
        else
        {
            groundPatrol();
        }
    }
}
