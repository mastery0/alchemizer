using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class player : MonoBehaviour
{
    public static player instance;
    public Rigidbody2D prb;
    public LayerMask ground;
    public LayerMask enemyLayer;
    public GameObject inv;
    public fillBar hpBar;
    public GameObject deathPanel;
    public GameObject bp;


    public Vector2 respawnAltar;
    public int respawnScene;


    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public int jumpAmount = 1;
    public float fastFallForce = 3f;
    public float dashForce = 5f;
    public float dashTime = 0.2f;
    public float dashCooldown = 1f;
    public float glideFallSpeed = 2f;
    [Header("Combat")]
    public float hp;
    public float maxHp = 100f;
    public float healMult = 1f;
    public float defense = 0f;
    public float attackDamage = 10f;
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    public float iFrames = 0.1f;

    //unlocks
    [System.NonSerialized] public bool hasDash = true;
    [System.NonSerialized] public bool coreInstability = true;
    [System.NonSerialized] public bool dashInvincibility = false;
    [System.NonSerialized] public bool airDash = true;
    [System.NonSerialized] public int dashCount = 1;
    [System.NonSerialized] public bool enemiesHeal = false;
    public bool hasGlider = true;

    private float moveX;
    private bool jumpHeld;
    private bool isDashing;
    private int currentDash;
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

        StartCoroutine(FadeDeathPanel(img, 0f,true));
    }
    void FixedUpdate()
    {
        if (!isDashing) prb.linearVelocity = new Vector2(moveX * moveSpeed, prb.linearVelocityY);
        grounded = Physics2D.OverlapCircle(transform.position, 0.9f, ground);
        if (grounded) currentDash = dashCount;
        glide();
        
        hp = Mathf.Clamp(hp, 0, maxHp);
    }
    // Input System
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!isAlive) return;
        Vector2 moveInput = context.ReadValue<Vector2>();
        bool isJumpHeld = moveInput.y > 0;
        moveX = moveInput.x;
        if (isJumpHeld && !jumpHeld && grounded)
        {
            jump();
        }
        jumpHeld = isJumpHeld;
        if (moveInput.y < 0)
        {
            fastFall();
        }
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
        attack();
    }
    public void OnOpenInv(InputAction.CallbackContext context)
    {
        if (!isAlive) return;
        inv.SetActive(!inv.activeSelf);
    }

    //movement
    private IEnumerator Dash()
    {
        isDashing = true;
        dashCD = true;
        currentDash--;
        prb.linearVelocity=new Vector2(moveX * dashForce, 0);
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
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
        prb.linearVelocity=new Vector2( prb.linearVelocityX, jumpForce);
    }
    public void fastFall()
    {
        prb.linearVelocity = new Vector2(prb.linearVelocityX, -fastFallForce);
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
        if (prb.linearVelocity.x > 0) facingDirection = Vector2.right;
        if (prb.linearVelocity.x < 0) facingDirection = Vector2.left;
        return facingDirection;
    }
    public void takeDamage(float damage)
    {
        if (!isAlive) return;
        if (isInvicible) return;
        hp -= damage-damage*defense;
        hpBar.setAmount(hp,maxHp);
        timeSinceHit = 0f;
        core.currentPressure += 10;
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
        canAttack = false;

        Vector2 dir = direction();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, attackRange, enemyLayer);
        Vector2 endPoint;
        if (hit.collider == null) endPoint = (Vector2)transform.position + dir * attackRange;
        else endPoint = hit.point;
        StartCoroutine(showRay(endPoint));

        if (hit.collider != null)
        {
            hit.collider.GetComponent<enemy>().takeDamage(attackDamage);
            timeSinceAttack = 0f;
            core.currentPressure += 10;
            hitStopManager.instance.stopTime(0.08f);
            Debug.Log("hitted");
        }
        Debug.Log("attacking");
        StartCoroutine(attackCD());
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
}
