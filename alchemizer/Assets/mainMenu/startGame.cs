using UnityEngine;

public class startGame : MonoBehaviour
{
    public void OnClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("gameScene");
    }
}
