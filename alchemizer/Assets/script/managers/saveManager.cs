using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
public class saveManager : MonoBehaviour
{
    public skillSO[] allSkills;
    public static saveManager instance;
    private static SaveData pendingLoadData;
    private List<string> seenDialogue=new List<string>();
    [System.Serializable]
    public class SaveData
    {
        public float maxHp;
        public Vector2 respawnAltar;
        public int respawnscene;

        public int airEss;
        public int waterEss;
        public int fireEss;
        public int lightEss;
        public int darkEss;

        public int[] unlockedSkillIDs;
        public string[] seenDialogueIDs;
    }
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (pendingLoadData != null)
        {
            StartCoroutine(ApplyPendingLoadWhenReady());
        }
    }
    public bool hasSeenDialogue(string dialogueID)
    {
        return seenDialogue.Contains(dialogueID);
    }
    public void markDialogueSeen(string dialogueID)
    {
        if (!seenDialogue.Contains(dialogueID))
        {
            seenDialogue.Add(dialogueID);
        }
    }
    [ContextMenu("save")]
    public void save()
    {
        SaveData data = new SaveData();
        data.maxHp=player.instance.maxHp;
        data.respawnAltar=player.instance.respawnAltar;
        data.respawnscene=player.instance.respawnScene;

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
        data.seenDialogueIDs = seenDialogue.ToArray();

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
            pendingLoadData = data;
            Time.timeScale = 1f;
            SceneManager.LoadScene(data.respawnscene);
        }
    }

    public void applyPendingLoad()
    {
        if (pendingLoadData == null) return;
        if (!CanApplyData()) return;

        ApplyData(pendingLoadData);
        pendingLoadData = null;
    }

    private IEnumerator ApplyPendingLoadWhenReady()
    {
        while (pendingLoadData != null && !CanApplyData())
        {
            yield return null;
        }

        applyPendingLoad();
    }

    private bool CanApplyData()
    {
        return player.instance != null && essenceManager.instance != null && coreInstability.instance != null;
    }

    private void ApplyData(SaveData data)
    {
        player.instance.maxHp = data.maxHp;
        player.instance.hp = data.maxHp;
        player.instance.respawnScene = data.respawnscene;
        player.instance.respawnAltar = data.respawnAltar;
        player.instance.transform.position = data.respawnAltar;
        player.instance.isAlive = true;

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
        seenDialogue.Clear();
        foreach (string dialogueID in data.seenDialogueIDs)
        {
            seenDialogue.Add(dialogueID);
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

        seenDialogue.Clear();

        foreach (skillSO skill in allSkills)
        {
            skill.isUnlocked= false;
        }
    }
}
