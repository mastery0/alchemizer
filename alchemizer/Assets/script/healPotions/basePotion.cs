using UnityEngine;

public class basePotion : potion
{
    private void Awake()
    {
        Init();
    }
    public override void Init()
    {
        do
        {
            potionID = "basePotion";
            potionName = "Base Potion";
            description = "heal 40 of max health but set pressure to 0";
            healAmount = 0.4f;
            potionAmount = 3;
        } while (potionName != "Base Potion");
        healManager.instance.potionDB.Add(this);
    }
    public override void OnUse()
    {
        Debug.Log("base Potion used");
        healManager.instance.remainingUse--;
        player.instance.heal(player.instance.maxHp * healAmount);
        player.instance.core.currentPressure = 0;
    }
}
