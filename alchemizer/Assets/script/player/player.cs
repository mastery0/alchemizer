using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
[RequireComponent(typeof(Animator))]
public class player : MonoBehaviour
{
    public static player instance;
    public Rigidbody2D prb;
    public LayerMask ground;
    public LayerMask enemyLayer;
    public GameObject actionbarObj;
    public fillBar hpBar;
    public GameObject deathPanel;
    public GameObject bp;
    public GameObject groundCheck;

    public Vector2 respawnAltar;
    public int respawnScene;

    public Animator animator;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public int jumpAmount = 1;
    public float fastFallForce = 3f;
    public float dashForce = 5f;
    public float dashTime = 0.2f;
    public float dashCooldown = 1f;
    public float glideFallSpeed = 2f;
    private float moveXClone;
    [Header("Combat")]
    public float hp;
    public float maxHp = 100f;
    public float healMult = 1f;
    public float defense = 0f;
    public float attackDamage = 10f;
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    public float iFrames = 0.1f;

    public float raySpeed;

    //unlocks
    [System.NonSerialized] public bool hasDash = true;
    [System.NonSerialized] public bool coreInstability = true;
    [System.NonSerialized] public bool dashInvincibility = false;
    [System.NonSerialized] public bool airDash = true;
    [System.NonSerialized] public int dashCount = 1;
    [System.NonSerialized] public bool enemiesHeal = false;
    [System.NonSerialized] public bool hasDoubleJump = true;//to set false before ship
    public bool hasGlider = true;

    private float moveX;
    private bool jumpHeld;
    private bool isDashing;
    private int currentDash;
    private int currentJump;
    private bool jumpCheck;
    private float defaultGravity;
    private bool dashCD;
    [System.NonSerialized] public coreInstability core;
    private bool grounded;
    public float timeSinceAttack;
    public float timeSinceHit;
    private bool canAttack = true;
    [System.NonSerialized] public bool isAlive = true;
    private bool isInvicible=false;
    private Vector2 facingDirection;
    private LineRenderer rayEffect;
    private bool canMove = true;

    private void Awake()
    {
        // Keep the Animator reference valid even if it was not assigned in the
        // inspector. The player uses the Animator on this same GameObject.
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Start()
    {
        instance = this;
        isAlive = true;
        Time.timeScale = 1f;
        prb = GetComponent<Rigidbody2D>();
        rayEffect = GetComponent<LineRenderer>();
        core = prb.GetComponent<coreInstability>();
        hp=maxHp;
        hpBar.setAmount(hp, maxHp);
        if (saveManager.instance != null)
        {
            saveManager.instance.applyPendingLoad();
        }
        hpBar.setAmount(hp, maxHp);
        Image img = bp.GetComponent<Image>();
        Color c = img.color;
        c.a = 1f;
        img.color = c;
        bp.SetActive(true);
        defaultGravity=prb.gravityScale;
        StartCoroutine(FadeDeathPanel(img, 0f,true));
    }
    void FixedUpdate()
    {
        faceTarget();
        if (!isDashing) prb.linearVelocity = new Vector2(moveX * moveSpeed, prb.linearVelocityY);

            grounded = Physics2D.OverlapCircle(groundCheck.transform.position, 0.1f, ground);
        if (grounded) { currentDash = dashCount; currentJump = jumpAmount; }
        glide();
        
        hp = Mathf.Clamp(hp, 0, maxHp);
        //animator
        setGrounded(grounded);
        if (!Mathf.Approximately(moveX, 0f)) setWalking(true);
        else if(Mathf.Approximately(prb.linearVelocity.y, 0f)) setWalking(false);
        if (prb.linearVelocity.y < 0f&&!grounded) setFalling(true);
        else setFalling(false);
    }
    protected void faceTarget()
    {
        if (Mathf.Approximately(moveX, 0f)) return;

        float dirx = Mathf.Sign(moveX);
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * dirx;
        transform.localScale = scale;
    }
    // Input System
    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log(canMove);  
        if (!isAlive) return;
        if (!canMove && context.canceled) moveXClone = 0;
        if (!canMove) return;
        Debug.Log("func called");
        Vector2 moveInput = context.ReadValue<Vector2>();
        bool isJumpHeld = moveInput.y > 0;
        moveX = moveInput.x;
        moveXClone = moveX;
        if (isJumpHeld && !jumpHeld && grounded)
        {
            jump();
            jumpCheck=false;
        }
        jumpHeld = isJumpHeld;
        if (hasDoubleJump && jumpHeld && !grounded && currentJump > 0&&jumpCheck)
        {
            jump();
            currentJump--;
            jumpCheck = false;
        }
        if (moveInput.y < 0)
        {
            fastFall();
        }
        if(prb.gravityScale!=defaultGravity&&moveInput.y>=0)prb.gravityScale = defaultGravity;
        if (moveInput.y == 0) jumpCheck = true;
    }
    public void OnDash(InputAction.CallbackContext context)
    {
        if (!isAlive) return;
        if (context.started && dashCheck())
        {
            StartCoroutine(Dash());
        }
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!isAlive) return;
        if(dialogueManager.instance.isTalking) return;
        if (actionBar.instance != null && actionBar.instance.isOpened) return;
        attack();
    }
    public void OnOpenMenu(InputAction.CallbackContext context)
    {
        if (!isAlive) return;
        actionbarObj.SetActive(!actionbarObj.activeSelf);
        Debug.Log(actionbarObj.activeSelf);
    }
    public void OnHeal(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed) return;
        if (!isAlive) return;
        healManager.instance.searchEquipped();
        if(healManager.instance.remainingUse>0)healManager.instance.equipped.OnUse();
    }
    //movement
    private IEnumerator Dash()
    {
        prb.gravityScale = 0;
        isDashing = true;
        dashCD = true;
        currentDash--;
        prb.linearVelocity=new Vector2(moveX * dashForce, 0);
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
        prb.gravityScale = defaultGravity;
        yield return new WaitForSeconds(dashCooldown);
        dashCD = false;
        
    }
    public bool dashCheck()
    {
        if (!hasDash) {Debug.Log("1"); return false;}
        if (isDashing) { Debug.Log("2"); return false; }
        if (!grounded) if(!airDash) { Debug.Log("3"); return false; }
        if (dashCD) { Debug.Log("4"); return false; }
        if (currentDash<=0) { Debug.Log("5"); return false; }
        return true;
    }
    public void jump()
    {
        jumpAnim();
        prb.linearVelocity=new Vector2( prb.linearVelocityX, jumpForce);
    }
    public void fastFall()
    {
        prb.gravityScale = prb.gravityScale * 1.3f;
    }
    private void glide()
    {
        if (!hasGlider) return;
        if (!jumpHeld) return;
        if (grounded) return;
        if (prb.linearVelocityY >= -glideFallSpeed) return;

        prb.linearVelocity = new Vector2(prb.linearVelocityX, -glideFallSpeed);
    }
    public Vector2 direction()
    {
        if (transform.localScale.x > 0) facingDirection = Vector2.right;
        if (transform.localScale.x < 0) facingDirection = Vector2.left;
        return facingDirection;
    }
    public void takeDamage(float damage)
    {
        if (!isAlive) return;
        if (isInvicible) return;
        hp -= damage-damage*defense;
        hpBar.setAmount(hp,maxHp);
        timeSinceHit = 0f;
        core.currentPressure += core.pressurePlusDelta;
        hitStopManager.instance.stopTime(0.08f);
        if (hp <= 0)
        {
            die();
        }
        
        StartCoroutine(invincibility());
    }
    private IEnumerator invincibility()
    {
        isInvicible = true;
        yield return new WaitForSeconds(iFrames);
        isInvicible=false;
    }
    private IEnumerator attackCD()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
    public void die()
    {
        if (!isAlive) return;
        dieAnim();
        isAlive = false;
        removeEssence();
        if (hitStopManager.instance != null)
        {
            hitStopManager.instance.StopAllCoroutines();
        }

        Time.timeScale = 0f;

        Image img = deathPanel.GetComponent<Image>();
        Color c = img.color;
        c.a = 0f;
        img.color = c;
        deathPanel.SetActive(true);

        StartCoroutine(FadeDeathPanel(img,1f));
    }

    public void respawn()
    {
        if (saveManager.instance != null)
        {
            saveManager.instance.load();
        }
    }

    private IEnumerator FadeDeathPanel(Image img, float t, bool disable = false)
    {
        Color c = img.color;
        yield return new WaitForSecondsRealtime(0.01f);
        while (!Mathf.Approximately(c.a,t))
        {
            c.a = Mathf.MoveTowards(c.a, t, 0.5f * Time.unscaledDeltaTime);
            img.color = c;
            yield return null;
        }
        if(disable&&img.color.a==t)img.gameObject.SetActive(false);
    }
    public void attack()
    {
        if (!canAttack) return;
        if (!grounded) return;

        canAttack = false;

        Vector2 dir = direction();

        canMove = false;
        moveX = 0;

        attackAnim();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, attackRange, enemyLayer);

        if (hit.collider != null)
        {
            StartCoroutine(timedRayCast(hit.collider,hit.point));
        }
        StartCoroutine(attackCD());
    }
    IEnumerator timedRayCast(Collider2D target,Vector2 hitPoint)
    {
        float distance = Vector2.Distance(transform.position, hitPoint);

        float time = distance / raySpeed;

        yield return new WaitForSeconds(time);


        //DEBUG
        Vector2 startPoint=transform.position;
        float elapsed = 0;
        while (elapsed < time)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / time;

            Vector2 currentPoint = Vector2.Lerp(startPoint, hitPoint, t);

            Debug.DrawLine(startPoint, currentPoint, Color.red);
            Debug.Log("in debug loop");
            yield return null;
        }
        //END DEBUG


        if (target==null)yield break;

            enemy e = target.GetComponent<enemy>();
            if (e != null)
            {
                e.takeDamage(attackDamage);

                timeSinceAttack = 0f;
                core.currentPressure += core.pressurePlusDelta;

                hitStopManager.instance.stopTime(0.08f);
            }
    }
    public void heal(float amount1)
    {
        float overflow = 0f;
        float amount=amount1*healMult;
        if (hp + amount<maxHp)
        {
            hp += amount;
        }
        else
        {
            hp = maxHp;
            overflow = (hp + amount) - maxHp;
        }
        hpBar.setAmount(hp,maxHp);
    }
    public void removeEssence()
    {
        essenceManager.instance.modifyAmount(essenceManager.essenceTypes.air, -(int)(essenceManager.instance.essenceInv[essenceManager.essenceTypes.air]*0.2));
        essenceManager.instance.modifyAmount(essenceManager.essenceTypes.water, -(int)(essenceManager.instance.essenceInv[essenceManager.essenceTypes.water] * 0.2));
        essenceManager.instance.modifyAmount(essenceManager.essenceTypes.fire, -(int)(essenceManager.instance.essenceInv[essenceManager.essenceTypes.fire] * 0.2));
        essenceManager.instance.modifyAmount(essenceManager.essenceTypes.light, -(int)(essenceManager.instance.essenceInv[essenceManager.essenceTypes.light] * 0.2));
        essenceManager.instance.modifyAmount(essenceManager.essenceTypes.dark, -(int)(essenceManager.instance.essenceInv[essenceManager.essenceTypes.dark] * 0.2));
    }

    public IEnumerator showRay(Vector2 endPoint)
    {
        rayEffect.enabled = true;
        rayEffect.SetPosition(0,transform.position);
        rayEffect.SetPosition(1,endPoint);
        yield return new WaitForSeconds(0.2f);
        rayEffect.enabled = false;
    }

    public IEnumerator buffATK(float buff,float time)
    {
        attackDamage += attackDamage*buff;
        yield return new WaitForSeconds(time);
        attackDamage -= attackDamage*buff;
    }
    //Animator Settings
    public void setWalking(bool walking)
    {
        animator.SetBool("isWalking", walking);
    }

    public void jumpAnim()
    {
        animator.SetTrigger("jump");
    }

    public void setFalling(bool falling)
    {
        animator.SetBool("falling", falling);
    }
    public void setGrounded(bool grounded)
    {
        animator.SetBool("grounded", grounded);
    }
    public void attackAnim()
    {
        animator.SetTrigger("attack");
    }

    public void dieAnim()
    {
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        animator.SetTrigger("die");
    }

    public void finishingAttackAnim()
    {
        canMove = true;
        moveX = moveXClone;
    }
}
