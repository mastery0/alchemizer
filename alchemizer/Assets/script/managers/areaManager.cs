using UnityEngine;
[System.Serializable]
public class area
{
    public string areaName;
    public GameObject areaPrefab;
    public GameObject enemies;
}
public class areaManager : MonoBehaviour
{
    public static areaManager instance;
    public area[] areaList;
    public string currentArea;
    private GameObject loadedEnemies;
    private void Awake()
    {
        instance=this;
        loadedEnemies= new GameObject("loadedEnemies");
    }
    public void switchToArea(string areaName)
    {
        foreach (area area in areaList)
        {
            if (area.areaName == areaName)
            {
                if (currentArea != null)
                {
                    foreach (area areaToDisable in areaList)
                    {
                        if (areaToDisable.areaName == currentArea)
                        {
                            areaToDisable.areaPrefab.SetActive(false);
                            areaToDisable.enemies.SetActive(false);
                        }
                    }
                }
                area.areaPrefab.SetActive(true);
                Destroy(loadedEnemies);
                loadedEnemies=Instantiate(area.enemies, area.areaPrefab.transform);
                loadedEnemies.SetActive(true);
                currentArea = areaName;
                return;
            }
        }
    }
}
