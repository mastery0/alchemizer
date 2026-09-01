using UnityEngine;
public enum menu
{
    skillTree,
    pauseMenu,
    questMenu,
    potionMenu
}
public class actionBar : MonoBehaviour
{
    public static actionBar instance;
    public bool isOpened = false;
    public menu lastOpenedMenu;

    public GameObject skillTree;
    public GameObject inventory;
    public GameObject pauseMenu;
    public GameObject questMenu;
    public GameObject potionMenu;

    public AudioClip clickSFX;
    public void Awake()
    {
        instance = this;
    }
    private void OnEnable()
    {
        isOpened = true;
        Time.timeScale = 0;
        skillTree.SetActive(true);
        inventory.SetActive(false);
        pauseMenu.SetActive(false);
        questMenu.SetActive(false);
        potionMenu.SetActive(false);
        switch (lastOpenedMenu)
        {
            case  menu.skillTree :
                skillTree.SetActive(true);
                inventory.SetActive(false);
                pauseMenu.SetActive(false);
                questMenu.SetActive(false);
                potionMenu.SetActive(false);
                break;
            case menu.pauseMenu :
                skillTree.SetActive(false);
                inventory.SetActive(false);
                pauseMenu.SetActive(true);
                questMenu.SetActive(false);
                potionMenu.SetActive(false);
                break;
            case menu.questMenu:
                skillTree.SetActive(false);
                inventory.SetActive(false);
                pauseMenu.SetActive(false);
                questMenu.SetActive(true);
                potionMenu.SetActive(false);
                break;
            case menu.potionMenu:
                skillTree.SetActive(false);
                inventory.SetActive(false);
                pauseMenu.SetActive(false);
                questMenu.SetActive(false);
                potionMenu.SetActive(true);
                break;
            default:
                skillTree.SetActive(true);
                inventory.SetActive(false);
                pauseMenu.SetActive(false);
                questMenu.SetActive(false);
                potionMenu.SetActive(false);
                break;
        }
    }
    private void OnDisable()
    {
        isOpened = false;
        Time.timeScale = 1;
        skillTree.SetActive(false);
        inventory.SetActive(false);
        pauseMenu.SetActive(false);
        questMenu.SetActive(false);
        potionMenu.SetActive(false);
    }
    public void OpenSkillTree()
    {
        audioManager.instance.playSFX(clickSFX);
        skillTree.SetActive(true);
        inventory.SetActive(false);
        pauseMenu.SetActive(false);
        questMenu.SetActive(false);
        potionMenu.SetActive(false);
        lastOpenedMenu = menu.skillTree;
    }
    public void OpenInventory()
    {
        audioManager.instance.playSFX(clickSFX);
        skillTree.SetActive(false);
        inventory.SetActive(true);
        pauseMenu.SetActive(false);
        questMenu.SetActive(false);
        potionMenu.SetActive(false);
    }
    public void OpenPauseMenu()
    {
        audioManager.instance.playSFX(clickSFX);
        skillTree.SetActive(false);
        inventory.SetActive(false);
        pauseMenu.SetActive(true);
        questMenu.SetActive(false);
        potionMenu.SetActive(false);
        lastOpenedMenu = menu.pauseMenu;
    }
    public void OpenQuestMenu()
    {
        audioManager.instance.playSFX(clickSFX);
        skillTree.SetActive(false);
        inventory.SetActive(false);
        pauseMenu.SetActive(false);
        questMenu.SetActive(true);
        potionMenu.SetActive(false);
        lastOpenedMenu = menu.questMenu;
    }
    public void OpenPotionMenu()
    {
        audioManager.instance.playSFX(clickSFX);
        skillTree.SetActive(false);
        inventory.SetActive(false);
        pauseMenu.SetActive(false);
        questMenu.SetActive(false);
        potionMenu.SetActive(true);
        lastOpenedMenu = menu.potionMenu;
    }
    public void closeActionBar()
    {
        audioManager.instance.playSFX(clickSFX);
        gameObject.SetActive(false);
    }
}
