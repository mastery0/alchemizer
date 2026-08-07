using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class spore : enemy
{
    public GameObject toxicCloudPrefab;
    public float spd;
    public float stopTime;
    private Coroutine cloudRoutine;
    private bool animAllowAttack;
    private new void Awake()
    {
        base.Awake();
        direction = 1;
        spd = speed;
    }
    public override void takeDamage(float damage)
    {
        if (cloudRoutine != null) StopCoroutine(cloudRoutine);
        cloudRoutine = StartCoroutine(spawnCloud());
        base.takeDamage(damage);
    }

    public IEnumerator spawnCloud()
    {
        speed = 0;
        yield return new WaitForSeconds(stopTime);
        attackAnim();
        while(!animAllowAttack) yield return null;
        Instantiate(toxicCloudPrefab, transform.position, Quaternion.identity);
        speed = spd;
        cloudRoutine = null;
        animAllowAttack = false;
    }
    public void animToAttack()
    {
        animAllowAttack = true;
    }
}
