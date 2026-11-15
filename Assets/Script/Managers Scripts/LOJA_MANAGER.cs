using UnityEngine;
using UnityEngine.UI;

public class LOJA_MANAGER : MonoBehaviour
{
    public static LOJA_MANAGER Instance;

    [Header("MENUS")]
    [SerializeField] private GameObject[] menuList;
    [SerializeField] private int currentMenuID = -1;
    [SerializeField] private int previousMenuID = -1;

    [Header("SELECTED MEC DEF")]
    [SerializeField] private MEC_DEF_SO selected_MecDef_SO;
    [SerializeField] private Image selected_MecDef_IMG;
    [SerializeField] private Image selected_nextLV_MecDef_IMG;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

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

    public void ChangeSelected(MEC_DEF_SO _mecDefSO, int _lv)
    {
        selected_MecDef_IMG.sprite = _mecDefSO.mecDefs[_lv].sprite;
        if(_lv < _mecDefSO.mecDefs.Length-1)
        {
            selected_nextLV_MecDef_IMG.sprite = _mecDefSO.mecDefs[_lv + 1].sprite;
        }
        else
        {
            selected_nextLV_MecDef_IMG.sprite = null;
        }
    }

    public void ComprarUpgrade()
    {

    }
}
