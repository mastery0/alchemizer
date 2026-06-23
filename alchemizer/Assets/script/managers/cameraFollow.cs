using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 5f;
    public Vector3 offsetX =new Vector3(-2, 0, 0);
    private Vector3 targetPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        targetPos = new Vector3(target.position.x, target.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos+offsetX, followSpeed * Time.deltaTime);
    }
}
