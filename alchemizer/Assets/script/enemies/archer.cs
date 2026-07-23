using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class archer : enemy
{
    [Header("Shoot Settings")]
    public float shooterRange;
    private bool isShooting = false;
    private bool fleeing = false;
    private bool canshoot = false;
    private bool inRange = false;
    public GameObject projectile;
    public float projSpeed;
    public float shootCD;


    protected override void Awake()
    {
        base.Awake();
        direction = 1;
        canshoot = true;
    }

    protected override void Update()
    {
        if (!playerScript.isAlive) return;
        shootSight();
        inRange = (Mathf.Abs(player.transform.position.x - transform.position.x) <= shooterRange && !isShooting);

        if (Mathf.Abs(player.transform.position.x - transform.position.x) < shooterRange * 0.6f && !isShooting&&sight)
        {
            fleeing = true;
        }
        else
        {
            fleeing = false;
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

        if (!isShooting&&!sight)
            base.groundPatrol();
    }

    public void flee()
    {
        Vector2 dir = (transform.position - player.transform.position).normalized;

        SetSafeHorizontalVelocity(dir.x * speed);
    }
    private void shootSight()
    {
        Vector2 dir = (player.transform.position - transform.position).normalized;
        RaycastHit2D saw = Physics2D.Raycast(transform.position, dir, shooterRange, sightMask); 
        sight = (saw.collider != null && saw.collider.CompareTag("Player"));
    }
    private IEnumerator shooter()
    {
        isShooting = true;
        canshoot = false;
        erb.linearVelocity = new Vector2(0, erb.linearVelocity.y);
        yield return new WaitForSeconds(1f);
        Vector2 dir = (player.transform.position - transform.position).normalized;
        GameObject p=Instantiate(projectile,transform.position+new Vector3(0.2f,0,0),Quaternion.identity);
        p.GetComponent<projScript>().setDamage(damage);
        p.GetComponent<Rigidbody2D>().linearVelocity = (new Vector2(dir.x * projSpeed, dir.y*projSpeed));
        isShooting = false;
        yield return new WaitForSeconds(shootCD);
        canshoot = true;
    }
}
