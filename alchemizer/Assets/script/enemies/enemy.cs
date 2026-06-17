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
