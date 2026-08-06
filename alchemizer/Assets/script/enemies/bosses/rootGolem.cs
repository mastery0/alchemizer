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
            if (engaged)
            {
                yield return StartCoroutine(idle());
                if (Random.Range(0, 100) < chanceOfNothing) continue;
                if (Vector2.Distance(transform.position, player.transform.position) > meleeRange - 1.2 ||
                (player.transform.position.x - transform.position.x) * prb.linearVelocity.x > 0)
                    StartCoroutine(meleeAttack());
                else
                if (Random.Range(0, 100) < 60) StartCoroutine(meleeAttack());
                else if (!hasRootHit) StartCoroutine(rootAttack());
                else StartCoroutine(meleeAttack());
            }
            yield return null;
        }
    }
    IEnumerator meleeAttack() 
    { 
        Debug.Log("melee");
        isAttacking = true;
        type = dmgTypeGolem.melee;
        hasMeleeHit = false;
        erb.linearVelocity=Vector2.zero;
        yield return StartCoroutine(crackGlow(meleeTelegraph, meleeGlowInterval));
        faceTarget();
        StartCoroutine(miniJump(7));
        float elapsed = 0f;
        while(elapsed < meleeDuration)
        {
            elapsed += Time.deltaTime;
            //OnDrawGizmos();
            if (!defeated && !hasMeleeHit && Vector2.Distance(transform.position, player.transform.position) <= meleeRange)
            {
                playerScript.takeDamage(calcDamage(type));
                Debug.Log("hitMelee");
                hasMeleeHit = true;
            }
            yield return null;
        }
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
        rootCoolDown();
        type = dmgTypeGolem.contact;
        erb.linearVelocity=Vector2.zero;
        yield return StartCoroutine(crackGlow(rootTelegraph, rootGlowInterval));
        faceTarget();
        if(!defeated) spawnRoots();
        yield return new WaitForSeconds(rootCD);
        isAttacking = false;
    }

    IEnumerator rootCoolDown() 
    {
        hasRootHit = true;
        yield return new WaitForSeconds(rootCD);
        hasRootHit = false;
    }
    IEnumerator idle() 
    {
        float desiredDestince = 3.5f;
        float t = 0f;
        bool reached= false;
        while (t < idleTime)
        {
            t+= Time.deltaTime;
            float dx=player.transform.position.x - transform.position.x;
            if (Mathf.Abs(dx) > desiredDestince&&!reached)
            {
                float dirx = Mathf.Sign(dx);
                erb.linearVelocity = new Vector2(dirx * speed, erb.linearVelocity.y);
            }
            else
            {
                erb.linearVelocity = new Vector2(0f, erb.linearVelocity.y);
                reached = true;
            }
            if(hasBeenHit)
            {
                t = idleTime;
            }
            yield return null;
        }
        erb.linearVelocity = Vector2.zero;
    }
    void spawnRoots() 
    { 
        float dirx = Mathf.Sign(player.transform.position.x - transform.position.x);
        for(int i=0;i < rootCount; i++)
        {
            Vector3 spawnPos = rootSpawnPoint.position + new Vector3(i * rootSpace * dirx, 0f, 0f);
            GameObject root = Instantiate(rootPrefab, spawnPos, Quaternion.identity);
            root.GetComponent<roots>().setup(dirx,calcDamage(dmgTypeGolem.root));
        }
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
        base.takeDamage(damage);
    }
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<player>().takeDamage(calcDamage(type));
            if(type==dmgTypeGolem.melee) hasBeenHit=true;
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

        erb.linearVelocity = new Vector2(horizontalSpeed, horizontalSpeed * 0.5f);
        Debug.Log(erb.linearVelocity.y);
        while (erb.linearVelocity.y > 0)
        {
            yield return null;
            Debug.Log(erb.linearVelocity.y);
        }
            while (transform.position.y > originalY + 0.01f)
            yield return null;

        erb.linearVelocity = Vector2.zero;
    }
    void setCrackColor(Color c)
    {
        foreach(var s in crackSprites)
        {
            s.color = c;
        }
    }
}
