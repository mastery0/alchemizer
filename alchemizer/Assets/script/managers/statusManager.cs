using UnityEngine;
using System.Collections.Generic;

public class statusManager : MonoBehaviour
{

    public List<statusEffect> activeEffects = new List<statusEffect>();
    private void Update()
    {
        tickEffect();
    }
    public void applyEffect(statusEffect effect)
    {
        effect.onApply();
        activeEffects.Add(effect);
    }
    public bool hasEffect(string effectName)
    {
        foreach (statusEffect effect in activeEffects)
        {
            if (effect.name == effectName)
            {
                return true;
            }
        }
        return false;
    }
    public void tickEffect()
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            activeEffects[i].onTick();
            if (activeEffects[i].remainingDuration <= 0)
            {
                activeEffects[i].onFinish();
                activeEffects.RemoveAt(i);
            }
        }
    }
}
