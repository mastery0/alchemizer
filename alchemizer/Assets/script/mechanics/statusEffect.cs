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

    public void damage(float damage)
    {
        if (gameObject.CompareTag("Player"))
        {
            if (!player.instance.isAlive) return;
            player.instance.hp -= damage;
            player.instance.hpBar.setAmount(player.instance.hp, player.instance.maxHp);
            player.instance.timeSinceHit = 0f;
            if (player.instance.hp <= 0)
            {
                player.instance.die();
            }
        }
        if (gameObject.CompareTag("Enemy"))
        {
            enemy e=GetComponent<enemy>();
            e.hp -= damage;
            if (e.hp <= 0) e.die();
        }
    }
}
