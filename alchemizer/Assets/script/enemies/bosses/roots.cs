using UnityEngine;
using System.Collections;
public class roots : MonoBehaviour
{
    [Header("emrge")]
    public float emergeDuration=0.25f;
    public float emergeHeight=0.9f;
    public Collider2D col;
    public float speed = 5f;
    public float lifetime = 5f;

    private float dirx;
    private float damage;
    private bool hasHit;
    private bool isActive;

    private void Awake()
    {
        if(col == null) col = GetComponent<Collider2D>();
        col.enabled = false;
    }
    public void setup(float dirx, float damage)
    {
        this.dirx = dirx;
        this.damage = damage;
        StartCoroutine(emerge());
    }
    IEnumerator emerge()
    {
        Vector2 target = new Vector2(transform.position.x,transform.position.y-1.2f);
        Vector2 startPos=target-new Vector2(0f,emergeHeight);
        transform.position=startPos;
        float elapsed=0f;
        while(elapsed < speed)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed/emergeDuration);
            transform.position=Vector2.Lerp(startPos,target,t);
            yield return null;
        }
        transform.position=target;
        col.enabled = true;
        isActive = true;
        StartCoroutine(selfDestroy());
    }
    IEnumerator selfDestroy()
    {
        yield return new WaitForSeconds(lifetime);
        if(!hasHit)Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!isActive||hasHit) return;
        if(collision.CompareTag("Player"))
        {
            hasHit = true;
            collision.GetComponent<player>().takeDamage(damage);
            Destroy(gameObject,0.1f);
        }
    }
}
