using Unity.VisualScripting;
using UnityEngine;

public class heatPotion : potion
{
    private void Start()
    {
        Init();
    }
    public override void Init()
    {
        potionID = "heatPotion";
        potionName = "Ember Potion";
        description = "Restores 25% of max health without removing pressure";
        healAmount = 0.25f;
        potionAmount = 5;
        healManager.instance.potionDB.Add(this);
    }
    public override void OnUse()
    {
        Debug.Log("Ember Potion used");
        healManager.instance.remainingUse--;
        player.instance.heal(player.instance.maxHp * healAmount);
    }
}
