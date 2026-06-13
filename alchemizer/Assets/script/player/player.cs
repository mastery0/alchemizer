using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class player : MonoBehaviour
{
    public static player instance;
    public Rigidbody2D prb;
    public LayerMask ground;
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float fastFallForce = 3f;
    public float dashForce = 5f;
    public float dashTime = 0.2f;
    public float dashCooldown = 1f;
    [Header("Combat")]
    public float hp;
    public float maxHp = 100f;
    public float attackDamage = 10f;
    public float attackRange = 1f;
    public float attackCooldown = 0.5f;


    private float moveX;
    private bool isDashing;
    private bool grounded;
    private bool canDash = true;
    private bool isInvicible=false;
    // Update is called once per frame

    private void Start()
    {
        instance = this;
        prb = GetComponent<Rigidbody2D>();
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
    public void takeDamage(float damage)
    {
        if (isInvicible) return;
        hp -= damage;
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
    public void die()
    {
        Debug.Log("Player Died");
    }
}
