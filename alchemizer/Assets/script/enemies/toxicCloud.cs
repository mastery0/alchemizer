using UnityEngine;

public class toxicCloud : MonoBehaviour
{
    public statusEffect poisonEfc;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject target = collision.gameObject;
        if (!target.CompareTag("Player") && !target.CompareTag("Enemy")) return;
        if (!collision.gameObject.GetComponent<statusManager>().hasEffect("Poison"))
        {
            poisonEfc = target.AddComponent<poison>();
            collision.gameObject.GetComponent<statusManager>().applyEffect(poisonEfc);
        }
        else
        {
            poison existing = target.GetComponent<poison>();
            collision.gameObject.GetComponent<statusManager>().applyEffect( existing);
        }
    }
}
