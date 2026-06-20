using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
public class pressureBar : fillBar
{
    public Sprite[] pressures;

    public void Start()
    {
        bar.fillAmount = 0;
    }
    public new void Update()
    {
        base.Update();
        checkSprites();
    }
    public void checkSprites()
    {
        if(player.instance.core.currentPressure >= 60)
        {
            bar.sprite = pressures[2];
            return;
        }
        if (player.instance.core.currentPressure >= 30)
        {
            bar.sprite = pressures[1];
            return;
        }
        if (player.instance.core.currentPressure < 30)
        {
            bar.sprite = pressures[0];
            return;
        }
    }
}
