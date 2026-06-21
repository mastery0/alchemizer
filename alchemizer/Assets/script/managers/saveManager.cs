using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class saveManager : MonoBehaviour
{
    public skillSO[] allSkills;
    public static saveManager instance;
    [System.Serializable]
    public class SaveData
    {
        public float maxHp;
        public Vector2 respawnAltar;

        public int airEss;
        public int waterEss;
        public int fireEss;
        public int lightEss;
        public int darkEss;

        public int[] unlockedSkillIDs;
    }
    private void Awake()
    {
        instance = this;
    }
    [ContextMenu("save")]
    public void save()
    {
        SaveData data = new SaveData();
        data.maxHp=player.instance.maxHp;
        data.respawnAltar=player.instance.respawnAltar;


        data.airEss = essenceManager.instance.essenceInv[essenceManager.essenceTypes.air];
        data.waterEss = essenceManager.instance.essenceInv[essenceManager.essenceTypes.water];
        data.fireEss = essenceManager.instance.essenceInv[essenceManager.essenceTypes.fire];
        data.lightEss = essenceManager.instance.essenceInv[essenceManager.essenceTypes.light];
        data.darkEss = essenceManager.instance.essenceInv[essenceManager.essenceTypes.dark];

        List<int> unlocked = new List<int>();
        foreach (skillSO skill in allSkills)
        {
            if (skill.isUnlocked)
            {
                unlocked.Add(skill.skillID);
            }
        }
        data.unlockedSkillIDs = unlocked.ToArray();


        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/save.json", json);
    }

    public void load()
    {
        string path = Application.persistentDataPath + "/save.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            player.instance.maxHp = data.maxHp;
            player.instance.respawnAltar = data.respawnAltar;
            player.instance.transform.position = data.respawnAltar;
            essenceManager.instance.essenceInv[essenceManager.essenceTypes.air]=data.airEss;
            essenceManager.instance.essenceInv[essenceManager.essenceTypes.water]=data.waterEss;
            essenceManager.instance.essenceInv[essenceManager.essenceTypes.fire]=data.fireEss;
            essenceManager.instance.essenceInv[essenceManager.essenceTypes.light]=data.lightEss;
            essenceManager.instance.essenceInv[essenceManager.essenceTypes.dark] = data.darkEss;

            foreach (skillSO skill in allSkills)
            {
                skill.isUnlocked= false;
                foreach (int id in data.unlockedSkillIDs)
                {
                    if (id != skill.skillID) continue;
                    skill.applyEffects();
                    break;
                }
            }
        }
    }
    [ContextMenu("reset")]
    public void toDefault()
    {
        player.instance.maxHp = 100;
        player.instance.respawnAltar = new Vector2(0,0);
        essenceManager.instance.essenceInv[essenceManager.essenceTypes.air] = 0;
        essenceManager.instance.essenceInv[essenceManager.essenceTypes.water] = 0;
        essenceManager.instance.essenceInv[essenceManager.essenceTypes.fire] = 0;
        essenceManager.instance.essenceInv[essenceManager.essenceTypes.light] = 0;
        essenceManager.instance.essenceInv[essenceManager.essenceTypes.dark] = 0;

        foreach (skillSO skill in allSkills)
        {
            skill.isUnlocked= false;
        }
    }
}
