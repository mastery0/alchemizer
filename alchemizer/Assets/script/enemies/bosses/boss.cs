using System.Collections;
using UnityEngine;

public abstract class boss : enemy
{
    [Header("setup")]
    public string bossName;
    public GameObject[] arenaGate;
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
    }
    protected override void Update()
    {
        if(Vector2.Distance(transform.position,player.transform.position)<Vector2.Distance(transform.position,arenaGate[0].transform.position))engage();
    }
    protected virtual void engage()
    {
        if (engaged) return;
        if(lockOnEngage)foreach (GameObject go in arenaGate)go.SetActive(true);
        //if(bossHealthUI.instance!=null)bossHealthUI.instance.show(this);
    }
    public override void takeDamage(float damage)
    {
        if (isInvincible || defeated) return;
        base.takeDamage(damage);
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
        foreach(GameObject go in arenaGate)go.SetActive(false);
        //if(bossHealthUI.instance!=null)bossHealthUI.instance.hide(this);
        if(checkPointPrefab!=null)Instantiate(checkPointPrefab, checkPointPos,Quaternion.identity);
        Destroy(gameObject);
    }
}
