using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class enemy : MonoBehaviour
{

    public Transform[] patrolPoints;
    protected Vector3[] patrolPositions;
    protected bool following;
    public int direction;
    public GameObject[] essencePrefab;
    protected int currentPoint=0;
    public LayerMask sightMask;
    protected GameObject player;
    protected Rigidbody2D erb;
    protected Rigidbody2D prb;
    protected player playerScript;

    [Header("Ground Safety")]
    [SerializeField] protected LayerMask groundMask;
    [SerializeField] protected float groundCheckDistance = 0.35f;
    [SerializeField] protected float edgeCheckAhead = 0.45f;
    [SerializeField] protected float footCheckOffset = 0.05f;

    protected Collider2D enemyCollider;

    [Header("Stats")]
    public float maxHp;
    protected float hp;
    protected bool sight;
    protected Vector2 dir;
    public float speed;
    public float damage;
    public float range;

    [Header("drops")]
    public essenceManager.essenceTypes[] essenceDrop;
    public Vector2Int minMaxEssence;
    public GameObject heals;
    protected virtual void Awake()
    {
        erb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        prb= player.GetComponent<Rigidbody2D>();
        playerScript = player.GetComponent<player>();
        enemyCollider = GetComponent<Collider2D>();
        hp = maxHp;
        patrolPositions = new Vector3[patrolPoints.Length];
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            patrolPositions[i]=patrolPoints[i].position;
        }
    }
    public virtual void takeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0) die();
    }
    protected virtual void die()
    {
        dropEssence();
        Destroy(gameObject);
    }
    protected virtual void Update()
    {
        hasSight();
        if (sight) groundFollow();
        else groundPatrol();
    }
    protected virtual void dropEssence()
    {
        foreach (var essence in essenceDrop)
        {
            int amount = Random.Range(minMaxEssence.x, minMaxEssence.y);
            if (amount == 0) continue;
            Vector2 pos = new Vector2(Random.Range(transform.position.x - 1.2f, transform.position.x + 1.2f), transform.position.y);
            var instance = Instantiate(essencePrefab[(int)essence], pos, Quaternion.identity);
            instance.GetComponent<essenceScript>().amount = amount;
        }
        if (playerScript.enemiesHeal)
            if (Random.Range(0, 10) == 9)
            {
                {
                    Instantiate(heals, new Vector2(Random.Range(transform.position.x - 1.2f, transform.position.x + 1.2f), transform.position.y), Quaternion.identity);
                }
            }
    }


    protected virtual void groundPatrol()
    {
        Vector2 target = patrolPositions[currentPoint];
        Vector2 patrolDir=(target-(Vector2)transform.position).normalized;
        SetSafeHorizontalVelocity(patrolDir.x * speed);
        if (Vector2.Distance(transform.position, target) < 0.4f)
        {
            currentPoint += direction;
            if (currentPoint >= patrolPoints.Length)
            {
                currentPoint=patrolPoints.Length-2;
                direction = -1; 
            }
            if (currentPoint < 0)
            {
                currentPoint = 1;
                direction = 1;
            }
        }
    }
    protected virtual void groundFollow()
    {
        SetSafeHorizontalVelocity(dir.x * speed);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerScript.takeDamage(damage);
            Debug.Log(playerScript.hp);
        }
    }
    public void hasSight()
    {
        dir = (player.transform.position - transform.position).normalized;
        RaycastHit2D saw = Physics2D.Raycast(transform.position, dir, range, sightMask);
        Debug.DrawRay(transform.position, dir * range, Color.red, 1f);
        sight = (saw.collider != null && saw.collider.CompareTag("Player"));
    }
    protected void SetSafeHorizontalVelocity(float xVelocity)
    {
        if (Mathf.Approximately(xVelocity, 0f))
        {
            erb.linearVelocity = new Vector2(0f, erb.linearVelocity.y);
            return;
        }

        if (!HasGroundAhead(Mathf.Sign(xVelocity)))
        {
            erb.linearVelocity = new Vector2(0f, erb.linearVelocity.y);
            return;
        }

        erb.linearVelocity = new Vector2(xVelocity, erb.linearVelocity.y);
    }

    protected bool HasGroundAhead(float moveDirection)
    {
        if (enemyCollider == null) return true;

        Bounds bounds = enemyCollider.bounds;
        Vector2 origin = new Vector2(
            bounds.center.x + moveDirection * (bounds.extents.x + edgeCheckAhead),
            bounds.min.y + footCheckOffset
        );

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundMask);
        Debug.DrawRay(origin, Vector2.down * groundCheckDistance, hit.collider ? Color.green : Color.yellow);

        return hit.collider != null;
    }

    protected void StopAtEdge()
    {
        float xVelocity = erb.linearVelocity.x;

        if (Mathf.Approximately(xVelocity, 0f)) return;

        if (!HasGroundAhead(Mathf.Sign(xVelocity)))
        {
            erb.linearVelocity = new Vector2(0f, erb.linearVelocity.y);
        }
    }
}
