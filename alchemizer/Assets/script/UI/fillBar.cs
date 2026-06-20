using UnityEngine;
using UnityEngine.UI;
public class fillBar : MonoBehaviour
{
    public Image bar;
    public float fillSpeed;
    protected float target;
    public void setAmount(float stat,float maxStat)
    {
        stat=Mathf.Clamp(stat,0,maxStat);
        maxStat = Mathf.Clamp(maxStat, 0, maxStat);
        target=stat/maxStat;
    }
    public void Update()
    {
        if (!Mathf.Approximately(bar.fillAmount, target))
            bar.fillAmount = Mathf.MoveTowards(bar.fillAmount,target, fillSpeed * Time.unscaledDeltaTime);
    }
}
