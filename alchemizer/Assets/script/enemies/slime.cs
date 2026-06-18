using UnityEngine;

public class slime : enemy
{
    protected override void Awake()
    {
        base.Awake();
        direction = 1;
    }
}
