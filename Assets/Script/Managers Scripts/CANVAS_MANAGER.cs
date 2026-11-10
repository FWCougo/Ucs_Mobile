using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CANVAS_MANAGER : MonoBehaviour
{
    [Header("PAINEL DEFESA SELECIONADA")]
    [SerializeField] private GameObject selected_PAINEL;
    [SerializeField] private GameObject actionSelected_PAINEL;
    [SerializeField] private bool selectedPAINEL_active;
    [SerializeField] private MEC_DEF_OBJ selected_MecDefOBJ;

    [SerializeField] private int selectedOBJ_cost;

    [SerializeField] private TMP_Text action_TXT;
    [SerializeField] private TMP_Text selectedOBJ_name_TXT;
    [SerializeField] private TMP_Text selectedOBJ_cost_TXT;
    [SerializeField] private Image selectedOBJ_Image;
    [SerializeField] private Button selectedObg_confirmAction_BTN;

    [Header("FASES")]
    [SerializeField] private Button[] levelBTNs;

    public static CANVAS_MANAGER Instance;

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
    }

    private void Start()
    {
        SelectedPainel_STATE(false);
        ActivateLevelButtons();
    }

    #region LevelBtns
    [ContextMenu("ATIVAR BOTAO")]
    private void ActivateLevelButtons()
    {
        int _fasesLiberadas = GAME_MANAGER.Instance.Levels;

        for (int i = 0; i < levelBTNs.Length; i++)
        {            
            levelBTNs[i].interactable = false;   
        }

        for (int i = 0; i < _fasesLiberadas; i++)
        {
            levelBTNs[i].interactable = true;
        }

    }
    #endregion

    #region SelectedPainel

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
        selectedOBJ_cost = selected_MecDefOBJ.MecDef_Serial.cost;
        selectedOBJ_cost_TXT.text = "Cost : "+selectedOBJ_cost.ToString();
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
        SelectedPainel_ActionPainel();
    }
    public void SelectedPainel_Move()
    {
        action_TXT.text = $"Move?";
        SelectedPainel_ActionPainel();
    }
    public void SelectedPainel_Upgrade()
    {
        action_TXT.text = $"Upgrade?";
        SelectedPainel_ActionPainel();
    }

    #endregion
}
