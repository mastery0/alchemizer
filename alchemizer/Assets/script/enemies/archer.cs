using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class archer : enemy
{
    [Header("Shoot Settings")]
    public float shooterRange;
    private bool isShooting=false;
    private bool fleeing=false;
    private bool canshoot=false;
    private bool inRange=false;
    public GameObject projectile;
    public float projSpeed;
    public float shootCD;


    protected override void Awake()
    {
        base.Awake();
        direction = 1;
    }

    protected override void Update()
    {
        hasSight();
        inRange = (Mathf.Abs(player.transform.position.x - transform.position.x) <= shooterRange && !isShooting);
        if ((player.transform.position - transform.position).x < shooterRange * 0.6f)
        {
            fleeing= true;
        }
        if (fleeing)
        {
            flee();
            return;
        }
        if (inRange && canshoot && sight)
        {
            StartCoroutine(shooter());
            return;
        }
        if(!isShooting)
        base.Update();
    }
    public void flee()
    {
        Vector2 dir = (transform.position - player.transform.position).normalized;

        SetSafeHorizontalVelocity(dir.x * speed);
    }
    private IEnumerator shooter()
    {
        isShooting = true;
        canshoot = false;
        yield return new WaitForSeconds(1f);
        Vector2 dir = (player.transform.position - transform.position).normalized;
        GameObject p=Instantiate(projectile,transform.position+new Vector3(0.2f,0,0),Quaternion.identity,transform);
        p.GetComponent<Rigidbody2D>().linearVelocity = (new Vector2(dir.x * projSpeed, dir.y*projSpeed));
        isShooting = false;
        yield return new WaitForSeconds(shootCD);
    }
}
