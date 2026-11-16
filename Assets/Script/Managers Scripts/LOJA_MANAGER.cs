using TMPro;
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
    [SerializeField] private int currentLV;
    [SerializeField] private MEC_DEF_SO selected_MecDef_SO;
    [SerializeField] private Image selected_MecDef_IMG;
    [SerializeField] private Image selected_nextLV_MecDef_IMG;

    [Header("UPGRADE SCREEN")]
    [SerializeField] private GameObject confirmarUpgrade_GO;
    [SerializeField] private int custoUpgrade;
    [SerializeField] private TMP_Text cost_txt;
    [SerializeField] private Button yes_BTN;

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
        currentLV = _lv;

        selected_MecDef_SO = _mecDefSO;
        

        selected_MecDef_IMG.sprite = _mecDefSO.mecDefs[_lv].sprite;

        if(_lv < _mecDefSO.mecDefs.Length-1)
        {
            selected_nextLV_MecDef_IMG.sprite = _mecDefSO.mecDefs[_lv + 1].sprite;
            custoUpgrade = selected_MecDef_SO.mecDefs[currentLV + 1].costUpgrade;
        }
        else
        {
            selected_nextLV_MecDef_IMG.sprite = null;
            custoUpgrade = -999;
        }
    }

    public void FazerUpgrade()
    {
        AtivarTelaUpgrade(true);

        if (currentLV < selected_MecDef_SO.mecDefs.Length - 1)
        {
            cost_txt.text = $"COST : {custoUpgrade}";
            yes_BTN.interactable = true;
        }
        else
        {
            cost_txt.text = $"MAXED OUT";
            yes_BTN.interactable = false;
        }
    }

    public void AtivarTelaUpgrade(bool _value)
    {
        confirmarUpgrade_GO.SetActive(_value);
    }

    public void ConfirmarUpgrade()
    {

        if (GAME_MANAGER.Instance.ChecarSeTemParafusos(custoUpgrade))
        {
            switch (selected_MecDef_SO.mecDefs[0].tpDefesa)
            {
                case TipoDefesa.obstaculo:
                    GAME_MANAGER.Instance.ObstaculoLV++;
                    break;
                case TipoDefesa.trapDeVelocidade:
                    GAME_MANAGER.Instance.SlowLV++;
                    break;
                case TipoDefesa.disparoLinear:
                    GAME_MANAGER.Instance.TorretaLV++;
                    break;
                case TipoDefesa.trapDeDano:
                    GAME_MANAGER.Instance.TrapLV++;
                    break;
            }

            GAME_MANAGER.Instance.RemoveParafuso(custoUpgrade);

            cost_txt.text = "UPGRADED!!!";

            Invoke("SetarCustoTXT", 3);
        }
        else
        {
            cost_txt.text = "YOU NEED MORE SCREWS!";

            Invoke("SetarCustoTXT", 3);
        }
    }

    void SetarCustoTXT()
    {
        cost_txt.text = $"COST : {custoUpgrade}";
    }

}
