using UnityEngine;

public class LOJA_MANAGER : MonoBehaviour
{
    public static MENU_MANAGER Instance;

    [SerializeField] private GameObject[] menuList;
    [SerializeField] private int currentMenuID = -1;
    [SerializeField] private int previousMenuID = -1;

    public void OpenMenu(string menuName)
    {
        CloseMenus();

        for (int i = 0; i < menuList.Length; i++)
        {
            if (menuList[i].name == menuName)
            {
                menuList[i].SetActive(true);
                currentMenuID = i;
                break;
            }
        }

        if (previousMenuID != -1 && previousMenuID != currentMenuID)
        {
            menuList[previousMenuID].SetActive(false);
        }

        previousMenuID = currentMenuID;
    }

    public void CloseMenus()
    {
        for (int i = 0; i < menuList.Length; i++)
        {
            menuList[i].SetActive(false);
        }
    }
}
