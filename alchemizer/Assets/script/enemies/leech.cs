using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class leech : enemy
{
    public int stealPerTick = 1;
    public float stealInterval = 1f;
    private bool isAttached = false;
    private Vector2 offset;
    private Coroutine drain;
    protected override void Update()
    {
        if (isAttached)
        {
            erb.position=(Vector2)player.transform.position+offset;
        }
        base.Update();
    }
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isAttached) if(collision.gameObject.CompareTag("Player"))attach(collision.gameObject);
    }
    private void attach(GameObject target)
    {
        isAttached = true;
        erb.linearVelocity = Vector2.zero;
        erb.bodyType = RigidbodyType2D.Kinematic;
        offset=(Vector2)transform.position-(Vector2)player.transform.position;
        if(drain!=null)StopCoroutine(drain);
        drain=StartCoroutine(drainRoutine());
    }
    private IEnumerator drainRoutine()
    {
        while (isAttached)
        {
            yield return new WaitForSeconds(stealInterval);
            if(!isAttached ) yield break;
            stealEssence();
            applyPoison();
        }
    }
    private void stealEssence()
    {
        int steal = Random.Range(0, 4);
        if (essenceManager.instance.essenceInv[(essenceManager.essenceTypes)steal] > stealPerTick) essenceManager.instance.essenceInv[(essenceManager.essenceTypes)steal] -= stealPerTick;
    }
    private void applyPoison()
    {
        GameObject target = player;
        statusEffect poisonEfc;
        if (!target.CompareTag("Player")) return;
        if (!player.GetComponent<statusManager>().hasEffect("Poison"))
        {
            poisonEfc = target.AddComponent<poison>();
            player.GetComponent<statusManager>().applyEffect(poisonEfc);
        }
        else
        {
            poison existing = target.GetComponent<poison>();
            player.GetComponent<statusManager>().applyEffect(existing);
        }
    }
    public override void die()
    {
        detach();
        base.die();
    }
    private void detach()
    {
        isAttached=false;
        if(drain != null) StopCoroutine(drain);
    }
    private void OnDestroy()
    {
        detach();
    }
}
