using JetBrains.Annotations;
using System;
using System.Collections;
using UnityEngine;

public class charger : enemy
{
    [Header("Dash Settings")]
    public float dashRange;
    public float CD;
    private bool canDash = true;
    private bool isDashing = false;
    private bool inRange;
    public float dashTime;
    public float dashForce;
    public bleed bleedEfc;
    protected override void Awake()
    {
        base.Awake();
        direction = 1;
    }
    protected override void Update()
    {
        if (!playerScript.isAlive) return;
        hasSight();
        inRange = (Mathf.Abs(player.transform.position.x - transform.position.x) <= dashRange && Mathf.Abs(player.transform.position.y - transform.position.y) <= 1f);
        if (inRange && canDash)
        {
            StartCoroutine(dash());
            return;
        }
        if(!isDashing)
        base.Update();
    }
    [ContextMenu("dash")]
    protected IEnumerator dash()
    {
        canDash = false;
        isDashing = true;

        Vector2 dir = (player.transform.position - transform.position).normalized;
        yield return new WaitForSeconds(0.5f);

        erb.linearVelocity = new Vector2(dir.x * dashForce, 0);
        yield return new WaitForSeconds(dashTime);

        erb.linearVelocity = new Vector2(0f, erb.linearVelocity.y);
        isDashing = false;

        yield return new WaitForSeconds(CD);
        canDash = true;
    }
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerScript.takeDamage(damage);
            Debug.Log(playerScript.hp);
            if (isDashing)
            {
                
                GameObject target = collision.gameObject;
                if (!target.CompareTag("Player")) return;
                if (!collision.gameObject.GetComponent<statusManager>().hasEffect("Bleed"))
                {
                    bleedEfc = target.AddComponent<bleed>();
                    collision.gameObject.GetComponent<statusManager>().applyEffect(bleedEfc);
                }
                else
                {
                    bleed existing = target.GetComponent<bleed>();
                    collision.gameObject.GetComponent<statusManager>().applyEffect(existing);
                }
            }
        }
    }
}
