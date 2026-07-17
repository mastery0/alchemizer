using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ragePotion : potion
{
    private void Start()
    {
        Init();
    }
    public override void Init()
    {
        potionID = "ragePotion";
        potionName = "Rage Potion";
        description = "Burns 5% of current health and increases attack by 20% for 10 seconds";
        healAmount = -0.05f;
        potionAmount = 2;
        healManager.instance.potionDB.Add(this);
    }
    public override void OnUse()
    {
        Debug.Log("Rage Potion used");
        healManager.instance.remainingUse--;
        player.instance.heal(player.instance.hp * healAmount);
        coreInstability.instance.currentPressure += coreInstability.instance.pressurePlusDelta;
        player.instance.buffATK(0.2f, 10);
    }
}
