using UnityEngine;

public class quit : MonoBehaviour
{
    public void OnClick()
    {
        Application.Quit();
        if (UnityEditor.EditorApplication.isPlaying) Debug.Log("quit");
    }
}
