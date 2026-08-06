using UnityEngine;
using System.Collections;
using UnityEditor.XR;
using JetBrains.Annotations;

public enum  dmgTypeGolem
{
    melee,
    root,
    contact
}
public class rootGolem : boss
{
    protected dmgTypeGolem type;
    protected bool hasRootHit;
    protected bool hasMeleeHit;
    protected bool hasBeenHit;
    protected bool isAttacking;
    protected string lastAttack="";
    [Header("crackVisuals")]
    public SpriteRenderer[] crackSprites;
    public Color defColor;
    public Color glowColor = Color.magenta;
    public float meleeGlowInterval;
    public float rootGlowInterval;
    public float meleeTelegraph;
    public float rootTelegraph;

    [Header("attack info")]
    public float idleTime;
    public int chanceOfNothing;
    public float meleeRange;

    [Header("meleeAttack")]
    public float meleeMult;
    public float meleeDuration;
    public float meleeRecovery;

    [Header("rootAttack")]
    public GameObject rootPrefab;
    public Transform rootSpawnPoint;
    public int rootCount;
    public float rootSpace;
    public float rootMult;
    public float rootCD;

    protected override void Awake()
    {
        base.Awake();
        foreach(var s in crackSprites)
        {
            s.color = defColor;
        }
    }
    private void OnEnable()
    {
        StartCoroutine(attackLoop());
    }
    protected override void Update()
    {
        base.Update();
        if(hp<=0) die();
        if (!isAttacking) type = dmgTypeGolem.contact;
    }
    protected float calcDamage(dmgTypeGolem type)
    {
        
        switch (type)
        {
            case dmgTypeGolem.melee: return meleeMult * damage;
            case dmgTypeGolem.root: return rootMult * damage;
            default: return damage;
        }
    }
    IEnumerator attackLoop()
    {
        while (!defeated)
        {
            if (!engaged) { yield return null; continue; }

            yield return StartCoroutine(idle());
            if (defeated) yield break;
            if (Random.Range(0, 100) < chanceOfNothing) continue;

            float distance = Vector2.Distance(transform.position, player.transform.position);
            hasBeenHit = false;
            if (distance < meleeRange - 1.2)
                if (lastAttack == "melee" && !hasRootHit)
                    if (Random.value < 0.7)
                        yield return StartCoroutine(rootAttack());
                    else yield return StartCoroutine(meleeAttack());
                else yield return StartCoroutine(meleeAttack());
            else
                if (!hasRootHit) yield return StartCoroutine(rootAttack());
            else yield return StartCoroutine(meleeAttack());

            yield return null;
        }
    }
    IEnumerator meleeAttack() 
    {
        isAttacking = true;
        type = dmgTypeGolem.melee;
        hasMeleeHit = false;
        erb.linearVelocity=Vector2.zero;

        yield return StartCoroutine(crackGlow(meleeTelegraph, meleeGlowInterval));

        faceTarget();

        float distance=Mathf.Abs(player.transform.position.x-transform.position.x);
        float jumpPower = Mathf.Clamp(distance * 1.5f, 4f, 8f);
        float direction = Mathf.Sign(player.transform.position.x - transform.position.x);
        if (direction != 0) { yield return StartCoroutine(miniJump(jumpPower * direction)); Debug.Log("jump skipped"); }
        float elapsed = 0f;
        while(elapsed < meleeDuration)
        {
            elapsed += Time.deltaTime;
            //OnDrawGizmos();
            if (!defeated && !hasMeleeHit && Vector2.Distance(transform.position, player.transform.position) <= meleeRange)
            {
                playerScript.takeDamage(calcDamage(type));
                hasMeleeHit = true;
            }
            yield return null;
        }
        erb.linearVelocity=Vector2.zero;
        lastAttack = "melee";
        isAttacking = false;
    }
    /*private void OnDrawGizmos()
    {
        if (isAttacking && type == dmgTypeGolem.melee)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, meleeRange);
        }
    }*/
    IEnumerator rootAttack() 
    { 
        isAttacking = true;
        type = dmgTypeGolem.contact;
        erb.linearVelocity=Vector2.zero;
        yield return StartCoroutine(crackGlow(rootTelegraph, rootGlowInterval));
        faceTarget();
        if(!defeated) yield return StartCoroutine(spawnRoots());
        lastAttack = "root";
        isAttacking = false;
        StartCoroutine(rootCoolDown());
        yield return null;
    }
    IEnumerator spawnRoots() 
    {
        float dirx = Mathf.Sign(player.transform.position.x - transform.position.x);
        for (int i = 0; i < rootCount; i++)
        {
            Vector3 spawnPos = rootSpawnPoint.position + new Vector3(i * rootSpace * dirx, 0f, 0f);
            GameObject root = Instantiate(rootPrefab, spawnPos, Quaternion.identity);
            root.GetComponent<roots>().setup(dirx, calcDamage(dmgTypeGolem.root));
            yield return new WaitForSeconds(0.15f);
        }
        yield return null;
    }

    IEnumerator rootCoolDown() 
    {
        hasRootHit = true;
        yield return new WaitForSeconds(rootCD);
        hasRootHit = false;
    }
    IEnumerator idle() 
    {
        float desiredDistance = Random.Range(3.5f-1.5f,3.5f+1.5f);
        float elapsed = 0f;
        bool locked = false;
        while (elapsed < idleTime)
        {
            elapsed += Time.deltaTime;
            float dx=player.transform.position.x - transform.position.x;
            float distance=Mathf.Abs(dx);
            if (hasBeenHit)
            {
                elapsed = idleTime;
                break;
            }
            if (distance > desiredDistance&&!locked)
            {
                float dirx = Mathf.Sign(dx);
                erb.linearVelocity = new Vector2(dirx * speed, erb.linearVelocity.y);
            }
            else if(distance < desiredDistance&&!locked)
            {
                float dirx = -Mathf.Sign(dx);
                erb.linearVelocity = new Vector2(dirx*speed*0.8f, erb.linearVelocity.y);
            }
            else
            {
                /*locked = true;
                float strafeDir = 1;
                if (Random.value < 0.002f)
                {
                    strafeDir *= -1;             
                }
                else
                {
                    erb.linearVelocity = new Vector2(strafeDir * speed * 0.5f, erb.linearVelocity.y);
                }*/
                elapsed=idleTime;
            }
           yield return null;
        }
        erb.linearVelocity = Vector2.zero;
        yield return null;
    }
    void faceTarget() 
    {
        float dirx=Mathf.Sign(player.transform.position.x-transform.position.x);
        Vector3 scale = transform.localScale;
        scale.x=Mathf.Abs(scale.x)*dirx;
        transform.localScale=scale;
    }
    public override void takeDamage(float damage)
    {
        if (isInvincible||defeated) return;
        hasBeenHit = true;
        base.takeDamage(damage);
    }
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<player>().takeDamage(calcDamage(type));
        }
    }
    IEnumerator crackGlow(float duration,float frequency)
    {
        float elapsed = 0f;
        while (elapsed<duration)
        {
            elapsed += Time.deltaTime;
            float pulse = Mathf.PingPong(elapsed * frequency * 2f, 1f);
            setCrackColor(Color.Lerp(defColor, glowColor, pulse));
            yield return null;
        }
        setCrackColor(defColor);
    }
    IEnumerator miniJump(float horizontalSpeed)
    {
        float originalY = transform.position.y;

        float verticalPower = Mathf.Clamp(Mathf.Abs(horizontalSpeed) * 0.7f, 3f, 6f);
        erb.linearVelocity = new Vector2(horizontalSpeed, verticalPower);
        while (erb.linearVelocity.y > 0) yield return null;
            while (transform.position.y > originalY + 0.01f) yield return null;

        erb.linearVelocity = Vector2.zero;
        yield return null;
    }
    void setCrackColor(Color c)
    {
        foreach(var s in crackSprites)
        {
            s.color = c;
        }
    }
}
