using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class enemy : MonoBehaviour
{

    public Transform[] patrolPoints;
    protected Vector3[] patrolPositions;
    protected bool following;
    public int direction;
    protected int currentPoint=0;

    protected GameObject player;
    protected Rigidbody2D erb;
    protected Rigidbody2D prb;
    protected player playerScript;



    [Header("Stats")]
    public float maxHp;
    protected float hp;
    public float speed;
    public float damage;
    public float range;
    protected virtual void Awake()
    {
        erb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        prb= player.GetComponent<Rigidbody2D>();
        playerScript = player.GetComponent<player>();
        if (player == null)
        {
            Debug.LogError("cannot find player obj"); 
            return;
        }
        hp = maxHp;
        patrolPositions = new Vector3[patrolPoints.Length];
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            patrolPositions[i]=patrolPoints[i].position;
        }
    }
    protected virtual void takeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0) die();
    }
    protected virtual void die()
    {
        Destroy(gameObject);
    }

    protected virtual void groundPatrol()
    {
        Vector2 target = patrolPositions[currentPoint];
        Vector2 dir=(target-(Vector2)transform.position).normalized;
        erb.linearVelocity = new Vector2(dir.x * speed, erb.linearVelocity.y);
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
        Vector2 dir = (player.transform.position - transform.position).normalized;
        erb.linearVelocity = new Vector2(dir.x * speed, erb.linearVelocity.y);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerScript.takeDamage(damage);
            Debug.Log(playerScript.hp);
        }
    }
}
