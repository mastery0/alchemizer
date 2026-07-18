using Unity.VisualScripting;
using UnityEngine;

public class heatPotion : potion
{
    private void Awake()
    {
        Init();
    }
    public override void Init()
    {
        do
        {
            potionID = "heatPotion";
            potionName = "Ember Potion";
            description = "Restores 25% of max health without removing pressure";
            healAmount = 0.25f;
            potionAmount = 5;
        } while (potionName != "Ember Potion");
        healManager.instance.potionDB.Add(this);
    }
    public override void OnUse()
    {
        Debug.Log("Ember Potion used");
        healManager.instance.remainingUse--;
        player.instance.heal(player.instance.maxHp * healAmount);
    }
}
