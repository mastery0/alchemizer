using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class areaSwitch : MonoBehaviour, IPointerDownHandler
{
    public string targetArea;
    private bool inRange;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inRange = true;
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            inRange = false;
        }
    }
    public void OnPointerDown (PointerEventData eventData)
    {
         if(inRange)areaManager.instance.switchToArea(targetArea);
    }
}
