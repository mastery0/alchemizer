using UnityEngine;

public class skillTreeManager : MonoBehaviour
{
    public static overMenuScript overMenu;
    private void Start()
    {
        overMenu = GetComponentInChildren<overMenuScript>(true);
    }
}
