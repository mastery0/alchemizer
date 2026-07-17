using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class potionUI : MonoBehaviour
{
    public static potionUI instance;
    public GameObject potionPrefab;
    public Transform content;
    public TMP_Text potionNameText;
    public TMP_Text potionDescription;
    public Image potionImage;
    public TMP_Text potionAmount;
    private void Awake()
    {
        instance = this;
    }
    void OnEnable()
    {
        buildUI();
    }
    public void buildUI()
    {
        foreach(potion potion in healManager.instance.potionDB)
        {
            if (!potion.isUnlocked) continue;
            GameObject potionObj=Instantiate(potionPrefab, content);
            potionPrefab.GetComponentInChildren<TMP_Text>().text = potion.name;
            Debug.Log(potion.name);
        }
    }

}
