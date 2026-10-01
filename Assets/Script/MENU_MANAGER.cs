using UnityEngine;

public class MENU_MANAGER : MonoBehaviour
{
    public static MENU_MANAGER Instance;

    [SerializeField] private GameObject[] menuList;
    [SerializeField] private int currentMenuID = -1;
    [SerializeField] private int previousMenuID = -1;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    private void Start()
    {
        //GetMenus();
        CloseMenus();
        OpenMenu("MENU_MENU");
    }

    [ContextMenu("GET MENUS")]
    public void GetMenus()
    {
        menuList = GameObject.FindGameObjectsWithTag("menu");
    }

    public void OpenMenu(string menuName)
    { 
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
