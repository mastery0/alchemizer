using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
public enum dmgType
{
    dash,
    jump,
    bull,
    contact
}
public class slimeBoss : boss
{
    protected dmgType type;
    protected bool hasDashHit;
    protected bool hasBullHit;
    protected bool hasBullCrushed;
    protected bool hasStartAttacking;
    [Header("info")]
    public LayerMask playerMask;
    protected bool isAttacking;
    [Header("coreVisuals")]
    public SpriteRenderer coreSprite;
    public Color defColor;
    public Color teleColor= Color.white;
    public float coreGlowInterval;
    [Header("attack info")]
    public float idleTime;
    public float chanceOfNothing;
    public float meleeRange;
    public float jumpRange;
    public Transform[] bullPoint;

    [Header("jumpAttack")]
    public float jumpMult;
    public float jumpDuration;
    public float jumpHeight;

    [Header("dashAttack")]
    public float dashMult;
    public float dashDuration;
    public float dashSpeed;

    [Header("bullAttack")]
    public float bullMult;
    public Transform[] point;
    public float bullSpeed;
    public float maxDuration;


    protected override void Awake()
    {
        base.Awake();
        coreSprite.color = defColor;
    }
    protected override void Update()
    {
        base.Update();
        if (hp <= 0) die();
        if (!isAttacking) type = dmgType.contact;
        if(!hasStartAttacking)StartCoroutine(attackLoop());
    }
    protected float calcDamage(dmgType type)
    {
        switch (type) 
        {
            default:return damage;
            case dmgType.dash:return damage * dashMult;
            case dmgType.jump:return damage * jumpMult;
            case dmgType.bull:return damage * bullMult;
        }
    }
    IEnumerator attackLoop()
    {
        hasStartAttacking = true;
        while (!defeated)
        {
            yield return new WaitUntil(() => engaged);

            yield return new WaitForSeconds(idleTime);

            float d = Vector2.Distance(transform.position, player.transform.position);

            if (d < meleeRange)
                yield return StartCoroutine(dashAttack());
            else if (d < jumpRange)
                yield return StartCoroutine(jumpAttack());
            else
                yield return StartCoroutine(bullAttack());
        }
    }
    IEnumerator jumpAttack()
    {
        isAttacking = true;
        type = dmgType.jump;
        erb.linearVelocity = Vector2.zero;
        //coreGlow
        Vector2 startPos=transform.position;
        Vector2 targetPos =player.transform.position;
        float elapsed = 0f;
        while (elapsed < jumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / jumpDuration;
            Vector2 flatPos = Vector2.Lerp(startPos, targetPos, t);
            float arc = jumpHeight * 4f * t * (1f - t);
            transform.position = new Vector2(flatPos.x, startPos.y + arc);
            yield return null;
        }
        transform.position = new Vector2(targetPos.x, startPos.y);
        isAttacking =false;
    }

    IEnumerator dashAttack()
    {
        isAttacking=true;
        type = dmgType.dash;
        erb.linearVelocity=Vector2.zero;
        hasDashHit=false;
        //coreGlow
        float dirx=Mathf.Sign(player.transform.position.x-transform.position.x);
        float elapsed = 0f;
        while (elapsed < dashDuration) 
        {
            elapsed += Time.deltaTime;
            erb.linearVelocity = new Vector2(dashSpeed * dirx,erb.linearVelocityY);
            if (hasDashHit) break;
            yield return null;
        }
        erb.linearVelocity= Vector2.zero;
        isAttacking=false;
    }

    IEnumerator bullAttack()
    {
        isAttacking = true;
        type = dmgType.bull;
        erb.linearVelocity = Vector2.zero;
        //coreGlow
        float closerPoint = point[0].position.x;
        foreach (Transform p in point)
        {
            if (Mathf.Abs(transform.position.x - p.position.x) <
                Mathf.Abs(transform.position.x - closerPoint))
            {
                closerPoint = p.position.x;
            }
        }
        float dirx = Mathf.Sign(closerPoint - transform.position.x);
        float elapsed = 0f;
        while (elapsed < maxDuration)
        {
            elapsed += Time.deltaTime;
            erb.linearVelocityX = dirx * bullSpeed;
            if (Mathf.Abs(transform.position.x - closerPoint) < 0.5f)
            {
                float original = closerPoint;
                foreach (Transform x in point)
                {
                    if (x.position.x < closerPoint && closerPoint != original) closerPoint = x.position.x;
                }
            }
            yield return null;
        }
        isAttacking = false;
    }
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerScript.takeDamage(calcDamage(type));
            if(type==dmgType.dash)hasDashHit=true;
        }
    }

}