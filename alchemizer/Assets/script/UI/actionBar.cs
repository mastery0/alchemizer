using UnityEngine;

public class actionBar : MonoBehaviour
{
    public GameObject skillTree;
    public GameObject inventory;
    public GameObject pauseMenu;
    private void OnEnable()
    {
        skillTree.SetActive(false);
        inventory.SetActive(true);
        pauseMenu.SetActive(false);
    }
    private void OnDisable()
    {
        skillTree.SetActive(false);
        inventory.SetActive(false);
        pauseMenu.SetActive(false);
    }
    public void OpenSkillTree()
    {
        skillTree.SetActive(true);
        inventory.SetActive(false);
        pauseMenu.SetActive(false);
    }
    public void OpenInventory()
    {
        skillTree.SetActive(false);
        inventory.SetActive(true);
        pauseMenu.SetActive(false);
    }
    public void OpenPauseMenu()
    {
        skillTree.SetActive(false);
        inventory.SetActive(false);
        pauseMenu.SetActive(true);
    }
}
