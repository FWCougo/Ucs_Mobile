using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CANVAS_MANAGER : MonoBehaviour
{
    public static CANVAS_MANAGER Instance;

    private SPAWN_INIMIGO spawn_INIMIGO;

    [Header("PAINEL DEFESA SELECIONADA")]
    [SerializeField] private GameObject selected_PAINEL;
    [SerializeField] private GameObject actionSelected_PAINEL;
    [SerializeField] private bool selectedPAINEL_active;
    [SerializeField] private MEC_DEF_OBJ selected_MecDefOBJ;

    [SerializeField] private int selectedOBJ_cost;

    [SerializeField] private TMP_Text action_TXT;
    [SerializeField] private TMP_Text selectedOBJ_name_TXT;
    [SerializeField] private TMP_Text selectedOBJ_cost_TXT;
    [SerializeField] private int custo;
    [SerializeField] private Image selectedOBJ_Image;
    [SerializeField] private Button selectedOBJ_confirmAction_BTN;

    [Header("FASES")]
    [SerializeField] private Button[] levelBTNs;

    [Header("PAINEL VITORIA")]
    [SerializeField] private TMP_Text dinheiroGasto_TXT;
    [SerializeField] private TMP_Text hpDaCasa_TXT;


    private void Awake()
    {
        if(Instance == null) 
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        spawn_INIMIGO = FindAnyObjectByType<SPAWN_INIMIGO>();
    }

    private void Start()
    {
        SelectedPainel_STATE(false);
        ActivateLevelButtons();
    }

    #region PainelVitoria
    public void SetarDinheiroGasto_TXT(int _dinheiroGasto)
    {
        dinheiroGasto_TXT.text = $"Money Spent : {_dinheiroGasto}";
    }
    public void SetarHPCasa_TXT(int _houseHP)
    {
        hpDaCasa_TXT.text = $"House HP : {_houseHP}%";
    }
    #endregion

    #region LevelBtns
    public void ActivateLevelButtons()
    {
        int _fasesLiberadas = GAME_MANAGER.Instance.UnlockedLevels;

        for (int i = 0; i < levelBTNs.Length; i++)
        {            
            if(i<_fasesLiberadas)
                levelBTNs[i].interactable = true;   
            else
                levelBTNs[i].interactable = false;
        }
    }
    #endregion

    #region Painel Selecionado
    public void SelectedPainel_STATE(bool value)
    {
        selectedPAINEL_active = value;
        selected_PAINEL.SetActive(value); 
        actionSelected_PAINEL.SetActive(false);
    }
    public void SelectedPainel_STATE()
    {
        selectedPAINEL_active = !selectedPAINEL_active;
        selected_PAINEL.SetActive(selectedPAINEL_active);
        actionSelected_PAINEL.SetActive(false);
    }
    public void SelectedPainel_STATE(MEC_DEF_OBJ _mecDefObj)
    {  
        if(selected_MecDefOBJ != _mecDefObj)
            selectedPAINEL_active = true;
        else
            selectedPAINEL_active = !selectedPAINEL_active;

        selected_MecDefOBJ = _mecDefObj;

        selected_PAINEL.SetActive(selectedPAINEL_active);
        actionSelected_PAINEL.SetActive(false);

        SelectedPainel_ChangeInfo();
    }
    private void SelectedPainel_ChangeInfo()
    {
        selectedOBJ_Image.sprite = selected_MecDefOBJ.MecDef_Serial.sprite;
        selectedOBJ_name_TXT.text = selected_MecDefOBJ.MecDef_Serial.name;
    }
    public void SelectedPainel_ActionPainel()
    {
        selected_PAINEL.SetActive(false);
        actionSelected_PAINEL.SetActive(true);
    }
    public void SelectedPainel_Sell()
    {
        action_TXT.text = $"Sell?"; 
        selectedOBJ_cost = selected_MecDefOBJ.MecDef_Serial.cost;
        selectedOBJ_cost_TXT.text = "You'll get " + selectedOBJ_cost.ToString();
        selectedOBJ_confirmAction_BTN.onClick.RemoveAllListeners();
        selectedOBJ_confirmAction_BTN.onClick.AddListener(Vender);
        SelectedPainel_ActionPainel();
    }
    public void SelectedPainel_Move()
    {
        action_TXT.text = $"Move?";
        selectedOBJ_cost = (int)(selected_MecDefOBJ.MecDef_Serial.cost*0.8f);
        selectedOBJ_cost_TXT.text = "Cost " + selectedOBJ_cost.ToString();
        selectedOBJ_confirmAction_BTN.onClick.RemoveAllListeners();
        selectedOBJ_confirmAction_BTN.onClick.AddListener(AtivarMovimentacao);
        SelectedPainel_ActionPainel();
    }
    public void SelectedPainel_Upgrade()
    {
        action_TXT.text = $"Upgrade?";
        selectedOBJ_cost = (int)(selected_MecDefOBJ.MecDef_Serial.cost);
        selectedOBJ_cost_TXT.text = "Cost " + selectedOBJ_cost.ToString();
        selectedOBJ_confirmAction_BTN.onClick.RemoveAllListeners();
        selectedOBJ_confirmAction_BTN.onClick.AddListener(Upgrade);
        SelectedPainel_ActionPainel();
    }

    public void Vender()
    {
        selected_MecDefOBJ.Vender(selectedOBJ_cost);
        SelectedPainel_STATE(false);
    }
    public void AtivarMovimentacao()
    {
        selected_MecDefOBJ.AtivarMovimentacao(true);
        MEC_DEF_MANAGER.Instance.AtivarPainelDosMecanismos(false);
        SelectedPainel_STATE(false);
    }
    public void Upgrade()
    {

        SelectedPainel_STATE(false);
    }

    public void AtivaPainelPosicionar()
    {
        SelectedPainel_ActionPainel();
        selectedOBJ_confirmAction_BTN.onClick.RemoveAllListeners();
        selectedOBJ_confirmAction_BTN.onClick.AddListener(AtivarMovimentacao);
    }
    public void Posicionar()
    {
        selected_MecDefOBJ.Posicionar();
    }

    #endregion

    #region REF MENU_MANAGER
    public void TrocarMenu(string _sceneName)
    {
        MENU_MANAGER.Instance.OpenMenu(_sceneName);
    }
    #endregion

    #region REF LOJA MANAGER
    public void FazerUpgrade()
    {
        LOJA_MANAGER.Instance.FazerUpgrade();
    }

    public void ConfirmarUpgrade()
    {
        LOJA_MANAGER.Instance.ConfirmarUpgrade();
    }

    public void AtivarTelaUpgrade(bool _value)
    {
        LOJA_MANAGER.Instance.AtivarTelaUpgrade(_value);

    }
    #endregion

    #region REF GAME_MANAGER
    public void StartGame(int _i)
    {
        GAME_MANAGER.Instance.StartGame(_i);
    }
    public void PauseGame()
    {
        GAME_MANAGER.Instance.Pause();
    }
    public void RestartGame()
    {
        GAME_MANAGER.Instance.RestartGame();
    }
    #endregion

    #region SPAWNER
    public void ComecarRound()
    {
        spawn_INIMIGO.ComecarRound();
    }
    #endregion
}
