using UnityEngine;

public class skillTreeManager : MonoBehaviour
{
    public static overMenuScript overMenu;
    public overMenuScript clone;
    private void Start()
    {
        overMenu = GetComponentInChildren<overMenuScript>(true);
        clone = overMenu;
    }
}
