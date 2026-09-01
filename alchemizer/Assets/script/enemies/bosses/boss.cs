using System.Collections;
using UnityEngine;

public abstract class boss : enemy
{
    [Header("setup")]
    public string bossName;
    public GameObject arenaGate;
    public Transform engagePoint;
    public bool lockOnEngage = true;
    [Header("phases")]
    public bool hasPhases = false;
    public float[] phasesThresholds;
    public int currentPhase;
    protected bool isInvincible = false;
    protected bool isTrans = false;
    [Header("checkPoint")]
    public bool hasACheckPoint;
    public GameObject checkPointPrefab;
    public Vector2 checkPointPos;

    protected bool engaged;
    protected bool defeated;

    protected override void Awake()
    {
        base.Awake();
        currentPhase = 1;
        if (saveManager.instance != null)
        {
            saveManager.instance.loadApplied += applySavedDefeatState;
        }
    }

    private void OnDestroy()
    {
        if (saveManager.instance != null)
        {
            saveManager.instance.loadApplied -= applySavedDefeatState;
        }
    }

    private void Start()
    {
        applySavedDefeatState();
    }

    private void applySavedDefeatState()
    {
        if (saveManager.instance != null && saveManager.instance.isBossDefeated(getBossID()))
        {
            if (checkPointPrefab != null) Instantiate(checkPointPrefab, checkPointPos, Quaternion.identity);
            arenaGate.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    private string getBossID()
    {
        return string.IsNullOrEmpty(bossName) ? enemyID : bossName;
    }
    protected override void Update()
    {
        if(Vector2.Distance(transform.position,player.transform.position)<Vector2.Distance(transform.position,engagePoint.position))engage();
    }
    protected virtual void engage()
    {
        if (engaged) return;
        if(lockOnEngage)arenaGate.SetActive(true);
        engaged=true;
        bossBar.instance.show();
        bossBar.instance.setAmount(hp, maxHp);
    }
    public override void takeDamage(float damage)
    {
        if (isInvincible || defeated) return;
        hp -= damage;
        if (hp <= 0&&enemyID!= "bossSlime") die();
        else
        {
            if (isFlashing) StopCoroutine(hitFlash());
            StartCoroutine(hitFlash());
        }
        bossBar.instance.setAmount(hp, maxHp);
        checkPhaseTransition();
    }
    protected virtual void checkPhaseTransition()
    {
        if (phasesThresholds == null || currentPhase > phasesThresholds.Length) return;
        float hpPercent=hp/maxHp;
        if (hpPercent <= phasesThresholds[currentPhase - 1])
        {
            currentPhase++;
            if (isTrans) StopCoroutine(phaseTrans());
            StartCoroutine(phaseTrans());
        }
    } 
    protected virtual IEnumerator phaseTrans()
    {
        isTrans = true;
        isInvincible= false;
        yield return new WaitForSeconds(1f);
        isInvincible = false;
        isTrans = false;
    }

    public override void die()
    {
        if(defeated) return;
        defeated = true;
        arenaGate.SetActive(false);
        bossBar.instance.hide();
        if(checkPointPrefab!=null)Instantiate(checkPointPrefab, checkPointPos,Quaternion.identity);
        saveManager.instance.markBossDefeated(getBossID());
        Destroy(gameObject);
    }
}
