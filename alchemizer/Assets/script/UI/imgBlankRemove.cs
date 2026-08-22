using UnityEngine;
using UnityEngine.UI;
public class imgBlankRemove : MonoBehaviour
{
   private Image img;
    void Start()
    {
        img= GetComponent<Image>();
    }

    void Update()
    {
        if (img.sprite == null) img.color = new Color() { a = 0 };
        else img.color = Color.white;
    }
}
