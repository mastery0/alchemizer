using UnityEngine;

public class skillTreeManager : MonoBehaviour
{
    public static skillTreeManager instance;
    public static overMenuScript overMenu;
    public overMenuScript clone;
    public AudioClip levelUpSFX;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        overMenu = GetComponentInChildren<overMenuScript>(true);
        clone = overMenu;
    }
}
