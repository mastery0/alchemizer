using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class checkPoint : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler
{
    public Vector2 checkPointPos;
    private void Start()
    {
        checkPointPos = transform.position;
    }
    public void OnPointerEnter(PointerEventData data)
    {
        if (Vector3.Distance(player.instance.transform.position, gameObject.transform.position) > 10)
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            return;
        }
        gameObject.GetComponent<SpriteRenderer>().color = Color.green;
    }

    public void OnPointerExit(PointerEventData data)
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }
    public void OnPointerDown(PointerEventData data)
    {
        player.instance.respawnAltar=checkPointPos;
        player.instance.respawnScene=gameObject.scene.buildIndex;
        saveManager.instance.save();
        Debug.Log("Checkpoint saved at: " + checkPointPos);
    }
}
