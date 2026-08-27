using System.Collections;
using UnityEngine;

public enum dmgTypeSlime
{
    dash,
    jump,
    bull,
    contact
}

public class slimeBoss : boss
{
    protected dmgTypeSlime type;

    protected bool hasDashHit;
    protected bool hasBullHit;
    protected bool isAttacking;
    protected bool hasBeenHit;

    [Header("Core Visuals")]
    public SpriteRenderer coreSprite;
    public Color defColor;
    public Color teleColor = Color.white;

    public float dashGlowInterval;
    public float jumpGlowInterval;
    public float bullGlowInterval;

    public float dashTelegraph;
    public float jumpTelegraph;
    public float bullTelegraph;

    [Header("Attack Info")]
    public float idleTime;
    public int chanceOfNothing;
    public float meleeRange;
    public float jumpRange;

    [Header("Jump Attack")]
    public float jumpMult;
    public float jumpDuration;
    public float jumpHeight;
    public GameObject groundCheck;
    public LayerMask ground;

    [Header("Dash Attack")]
    public float dashMult;
    public float dashDuration;
    public float dashSpeed;

    [Header("Bull Attack")]
    public float bullMult;
    public Transform[] point;
    public float bullSpeed;
    public float maxDuration;

    private bool hasDeathStarted=false;
    protected override void Awake()
    {
        base.Awake();

        coreSprite.color = defColor;

        if (animator == null)
            animator = GetComponent<Animator>();

        direction = 1;
    }

    private void OnEnable()
    {
        StartCoroutine(attackLoop());
    }

    protected override void Update()
    {
        base.Update();
        if (hp <= 0 && !defeated&&!hasDeathStarted)
        {
            hasDeathStarted = true;
            deathAnim();
            StartCoroutine(canDie());
        }
        if(hasDeathStarted)erb.linearVelocity=new Vector2(0,erb.linearVelocity.y);
        if (!isAttacking)
            type = dmgTypeSlime.contact;
    }

    protected float calcDamage(dmgTypeSlime type)
    {
        switch (type)
        {
            default:
                return damage;

            case dmgTypeSlime.dash:
                return damage * dashMult;

            case dmgTypeSlime.jump:
                return damage * jumpMult;

            case dmgTypeSlime.bull:
                return damage * bullMult;
        }
    }

    protected IEnumerator attackLoop()
    {
        while (!defeated)
        {
            if (engaged)
            {
                yield return StartCoroutine(idle());

                if (Random.Range(1f, 100f) > chanceOfNothing)
                {
                    float d = Vector2.Distance(transform.position, player.transform.position);

                    hasBeenHit = false;

                    if (d < meleeRange)
                    {
                        float r = Random.value;

                        if (r < 0.6f)
                        {
                            yield return StartCoroutine(dashAttack());
                        }
                        else if (r < 0.8f)
                        {
                            yield return StartCoroutine(jumpAttack());
                        }
                        else
                        {
                            yield return StartCoroutine(bullAttack());
                        }
                    }
                    else if (d < jumpRange)
                    {
                        float r = Random.value;

                        if (r < 0.7f)
                        {
                            yield return StartCoroutine(jumpAttack());
                        }
                        else
                        {
                            yield return StartCoroutine(dashAttack());
                        }
                    }
                    else
                    {
                        yield return StartCoroutine(bullAttack());
                    }
                }
            }

            yield return null;
        }
    }

    protected IEnumerator jumpAttack()
    {
        isAttacking = true;
        type = dmgTypeSlime.jump;

        erb.linearVelocity = Vector2.zero;

        faceTarget();

        yield return StartCoroutine(coreGlow(jumpTelegraph, jumpGlowInterval));

        float startX = transform.position.x;
        float targetX = player.transform.position.x;
        float distanceX = targetX - startX;

        float gravity = Mathf.Abs(Physics2D.gravity.y * erb.gravityScale);

        float jumpVelocity = Mathf.Sqrt(2f * gravity * jumpHeight);
        float flightTime = (2f * jumpVelocity) / gravity;

        if (jumpDuration > 0f)
        {
            flightTime = jumpDuration;

            jumpVelocity = gravity * flightTime / 2f;
        }

        float horizontalVelocity = distanceX / flightTime;


        jumpUpAnim(false);
        jumpDownAnim(false);


        jumpStartAnim();

        AnimatorStateInfo state;

        while (true)
        {
            state = animator.GetCurrentAnimatorStateInfo(0);

            if (state.IsName("jumpAnim"))
                break;

            yield return null;
        }

        erb.linearVelocity = new Vector2(horizontalVelocity, jumpVelocity);

        jumpUpAnim(true);

        bool falling = false;

        while (true)
        {
            if (!falling && erb.linearVelocity.y <= 0f)
            {
                falling = true;

                jumpUpAnim(false);
                jumpDownAnim(true);
            }

            if (falling && Physics2D.OverlapCircle(groundCheck.transform.position, 0.1f, ground))
            {
                break;
            }

            yield return null;
        }

        erb.linearVelocity = Vector2.zero;

        jumpUpAnim(false);
        jumpDownAnim(false);

        jumpLandAnim();


        while (true)
        {
            state = animator.GetCurrentAnimatorStateInfo(0);

            if (state.IsName("touchGround"))
                break;

            yield return null;
        }

        while (true)
        {
            state = animator.GetCurrentAnimatorStateInfo(0);

            if (!state.IsName("touchGround"))
                break;

            yield return null;
        }

        isAttacking = false;
    }

    protected IEnumerator dashAttack()
    {
        isAttacking = true;
        type = dmgTypeSlime.dash;

        erb.linearVelocity = Vector2.zero;

        hasDashHit = false;

        faceTarget();

        yield return StartCoroutine(coreGlow(dashTelegraph, dashGlowInterval));

        dashAnim();

        float dirx = Mathf.Sign(player.transform.position.x - transform.position.x);

        float originalY = transform.position.y;

        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            elapsed += Time.deltaTime;

            erb.linearVelocity = new Vector2(dashSpeed * dirx, erb.linearVelocityY);

            transform.position = new Vector2(transform.position.x, originalY + 0.01f);

            if (hasDashHit)break;

            yield return null;
        }

        erb.linearVelocity = Vector2.zero;

        stopDashAnim();

        isAttacking = false;
    }

    protected IEnumerator bullAttack()
    {
        isAttacking = true;
        type = dmgTypeSlime.bull;

        erb.linearVelocity = Vector2.zero;

        hasBullHit = false;

        faceTarget();

        yield return StartCoroutine(coreGlow(bullTelegraph, bullGlowInterval));

        bullAnim();

        float closerPoint = point[0].position.x;

        foreach (Transform p in point)
        {
            if (Mathf.Abs(transform.position.x - p.position.x) < Mathf.Abs(transform.position.x - closerPoint))
            {
                closerPoint = p.position.x;
            }
        }

        float dirx = Mathf.Sign(closerPoint - transform.position.x);

        float elapsed = 0f;

        float originalY = transform.position.y;

        while (elapsed < maxDuration)
        {
            elapsed += Time.deltaTime;

            erb.linearVelocityX = dirx * bullSpeed;

            transform.position = new Vector2(transform.position.x, originalY + 0.01f);

            if (Mathf.Abs(transform.position.x - closerPoint) < 0.5f)
            {
                if (closerPoint == point[0].position.x)
                {
                    closerPoint = point[1].position.x;
                }
                else
                {
                    closerPoint = point[0].position.x;
                }

                dirx = Mathf.Sign(closerPoint - transform.position.x);
                Vector3 scale = transform.localScale;

                scale.x = Mathf.Abs(scale.x) * dirx;

                transform.localScale = scale;
            }

            yield return null;
        }

        erb.linearVelocity = Vector2.zero;

        stopBullAnim();

        isAttacking = false;
    }

    protected IEnumerator idle()
    {
        float desiredDistance = 5.3f;

        float t = 0f;

        bool reached = false;

        walkingAnim(true);

        while (t < idleTime)
        {
            t += Time.deltaTime;
            faceTarget();
            float dx = player.transform.position.x - transform.position.x;

            if (Mathf.Abs(dx) > desiredDistance && !reached)
            {
                faceTarget();

                erb.linearVelocity = new Vector2(Mathf.Sign(dx) * speed, erb.linearVelocityY);
            }
            else
            {
                reached = true;

                float strafeDir =Mathf.Sign(Mathf.Sin(Time.time * 3f));

                erb.linearVelocity = new Vector2(strafeDir * speed * 0.3f, erb.linearVelocityY);
            }

            if (hasBeenHit)
            {
                t = idleTime;
            }

            yield return null;
            yield return null;
        }

        erb.linearVelocity = Vector2.zero;

        walkingAnim(false);
    }

    public override void takeDamage(float damage)
    {
        base.takeDamage(damage);

        hasBeenHit = true;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerScript.takeDamage(
                calcDamage(type)
            );

            if (type == dmgTypeSlime.dash)
            {
                hasDashHit = true;
            }

            if (type == dmgTypeSlime.bull)
            {
                hasBullHit = true;
            }
        }
    }

    protected new void faceTarget()
    {
        float dirx = Mathf.Sign(player.transform.position.x - transform.position.x);

        Vector3 scale = transform.localScale;

        scale.x = Mathf.Abs(scale.x) * dirx;

        transform.localScale = scale;
    }

    protected void setGlow(float value)
    {
        coreSprite.material.SetFloat("_Pulse",value);
    }

    protected IEnumerator coreGlow(float duration, float frequency)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float pulse =Mathf.PingPong(elapsed * frequency * 2f, 1f);

            setGlow(pulse);

            yield return null;
        }

        setGlow(0f);
    }
    private IEnumerator canDie()
    {
        AnimatorStateInfo state;

        while (true)
        {
            state = animator.GetCurrentAnimatorStateInfo(0);
            erb.linearVelocity = new Vector2(0, erb.linearVelocity.y);
            if (state.IsName("deathAnim") && state.normalizedTime >= 1f)break;

            yield return null;
        }

        die();
    }
    //animator

    /*
    isWalking bool
    jump trigger
    inAir bool
    fall bool
    touchGround trigger
    dash bool
    bull bool
    death trigger
    */

    public void walkingAnim(bool value)
    {
        animator.SetBool("isWalking", value);
    }

    public void jumpStartAnim()
    {
        animator.ResetTrigger("touchGround");

        animator.SetBool("inAir", false);
        animator.SetBool("fall", false);

        animator.SetTrigger("jump");
    }

    public void jumpUpAnim(bool value)
    {
        animator.SetBool("inAir", value);
    }

    public void jumpDownAnim(bool value)
    {
        animator.SetBool("fall", value);
    }

    public void jumpLandAnim()
    {
        animator.SetBool("inAir", false);
        animator.SetBool("fall", false);

        animator.SetTrigger("touchGround");
    }

    public void dashAnim()
    {
        animator.SetBool("dash", true);
    }

    public void stopDashAnim()
    {
        animator.SetBool("dash", false);
    }

    public void bullAnim()
    {
        animator.SetBool("dash", true);
    }

    public void stopBullAnim()
    {
        animator.SetBool("dash", false);
    }

    public void deathAnim()
    {
        animator.SetTrigger("death");
    }
}