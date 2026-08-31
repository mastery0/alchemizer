using UnityEngine;

public class switchPanel : MonoBehaviour
{
    public GameObject panel;
    public void onClick()
    {
        panel.SetActive(!panel.activeSelf);
    }
}
