using UnityEngine;
using UnityEngine.Rendering;

public abstract class statusEffect: MonoBehaviour
{
    public string effectName;
    public float duration;
    public float remainingDuration;
    public abstract void onApply();
    public abstract void onTick();
    public abstract void onFinish();
}
