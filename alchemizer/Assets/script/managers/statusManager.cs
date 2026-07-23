using UnityEngine;
using System.Collections.Generic;

public class statusManager : MonoBehaviour
{

    public List<statusEffect> activeEffects = new List<statusEffect>();
    public List<string> immuneEffects = new List<string>();
    private void Update()
    {
        tickEffect();
    }
    public void applyEffect(statusEffect effect)
    {

        if (activeEffects.Contains(effect))
        {
            Debug.Log("contiene");
            foreach (statusEffect e in activeEffects)
            {
                if (effect.effectName == e.effectName)
                {
                    e.remainingDuration = effect.duration;
                    Debug.Log("durata resettata");
                }
            }
        } 
        else if (!immuneEffects.Contains(effect.effectName))
        {
            effect.onApply();
            activeEffects.Add(effect);
        }
    }
    public bool hasEffect(string effectName)
    {
        foreach (statusEffect effect in activeEffects)
        {
            if (effect.effectName == effectName)
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
            if (activeEffects[i] == null)
            {
                activeEffects.RemoveAt(i);
                continue;
            }
            if (activeEffects[i].remainingDuration <= 0)
            {
                activeEffects[i].onFinish();
                activeEffects.RemoveAt(i);
                continue;
            }
            activeEffects[i].onTick();
        }
    }
}
