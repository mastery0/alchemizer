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
    [Header("Combat")]
    public float hp;
    public float maxHp = 100f;
    public float defense = 0f;
    public float attackDamage = 10f;
    public float attackRange = 1f;
    public float attackCooldown = 1f;


    //unlocks
    private bool canDash = true;
    [HideInInspector]public bool coreInstability = false;
    [HideInInspector]public bool dashInvincibility = false;


    private float moveX;
    private bool isDashing;
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
        
        hp = Mathf.Clamp(hp, 0, maxHp);
    }
    // Input System
    public void OnMove(InputAction.CallbackContext context)
    {
        moveX = context.ReadValue<Vector2>().x;
        if (context.ReadValue<Vector2>().y > 0 && grounded)
        {
            jump();
        }
        if (context.ReadValue<Vector2>().y < 0)
        {
            fastFall();
        }
    }
    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started && canDash)
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
        Debug.Log("Dashing");
        isDashing = true;
        canDash = false;
        prb.linearVelocity=new Vector2(moveX * dashForce, 0);
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    public void jump()
    {
        prb.linearVelocity=new Vector2( prb.linearVelocityX, jumpForce);
    }
    public void fastFall()
    {
        prb.linearVelocity = new Vector2(prb.linearVelocityX, -fastFallForce);
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
