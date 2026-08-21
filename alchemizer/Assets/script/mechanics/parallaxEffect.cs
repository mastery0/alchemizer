using UnityEngine;

public class parallaxEffect : MonoBehaviour
{
    private float length, startpos;
    public GameObject cam;
    public float parallexEffectMult;
    void Start()
    {
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }
    void Update()
    {
        float temp = (cam.transform.position.x * (1 - parallexEffectMult));
        float dist = (cam.transform.position.x * parallexEffectMult);
        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);
        if (temp > startpos + length) startpos += length*2;
        else if (temp < startpos - length) startpos -= length*2;
    }
}
