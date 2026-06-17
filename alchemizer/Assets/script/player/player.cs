using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class player : MonoBehaviour
{
    public static player instance;
    public Rigidbody2D prb;
    public LayerMask ground;
    public LayerMask enemyLayer;
    public GameObject inv;
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
    public float defense = 0f;
    public float attackDamage = 10f;
    public float attackRange = 1f;
    public float attackCooldown = 1f;


    //unlocks
    [System.NonSerialized] public bool hasDash = true;
    [System.NonSerialized] public bool coreInstability = false;
    [System.NonSerialized] public bool dashInvincibility = false;
    [System.NonSerialized] public bool airDash = true;
    [System.NonSerialized] public int dashCount = 1;
    public bool hasGlider = true;

    private float moveX;
    private bool jumpHeld;
    private bool isDashing;
    private int currentDash;
    private bool dashCD;
    private coreInstability core;
    private bool grounded;
    public float timeSinceAttack;
    public float timeSinceHit;
    private bool canAttack = true;
    private bool isInvicible=false;
    private Vector2 facingDirection;
    // Update is called once per frame

    private void Start()
    {
        instance = this;
        prb = GetComponent<Rigidbody2D>();
        core = prb.GetComponent<coreInstability>();
        hp=maxHp;
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
        if (context.started && dashCheck())
        {
            StartCoroutine(Dash());
        }
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        attack();
    }
    public void OnOpenInv(InputAction.CallbackContext context)
    {
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
        if (isInvicible) return;
        hp -= damage-damage*defense;
        timeSinceHit = 0f;
        core.currentPressure += 10;
        if (hp <= 0)
        {
            die();
        }
        
        StartCoroutine(invincibility());
    }
    private IEnumerator invincibility()
    {
        isInvicible = true;
        yield return new WaitForSeconds(1);
        isInvicible=false;
    }
    private IEnumerator attackCD()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
    public void die()
    {
        Debug.Log("Player Died");
    }
    public void attack()
    {
        if (!canAttack) return;
        canAttack = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction(), attackRange, enemyLayer);
        Debug.DrawRay(transform.position,direction()*attackRange,Color.blue,1);
        if (hit.collider != null)
        {
            hit.collider.GetComponent<enemy>().takeDamage(attackDamage);
            timeSinceAttack = 0f;
            core.currentPressure += 10;
            Debug.Log("hitted");
        }
        Debug.Log("attacking");
        StartCoroutine(attackCD());
    }
}
