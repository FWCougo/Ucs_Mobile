using UnityEngine;

public class MEC_DEF_MANAGER : MonoBehaviour
{
    public static MEC_DEF_MANAGER Instance;
    [SerializeField] private Transform MecDef_Conteiner;
    [SerializeField] private CanvasGroup mecDefs_canvasGroup;
    [SerializeField] private MEC_DEF_UI mecDefUI;



    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        FadeMecDef_CG(1);
    }

    public void AtivarPainelDosMecanismos(bool _value)
    {
        PodeUsarMecDef_Painel(_value);

        if (_value)
        {
            FadeMecDef_CG(1);
        }
        else
        {            
            FadeMecDef_CG(0.5f);
        }
    }

    public void PodeUsarMecDef_Painel(bool _value)
    {
        mecDefs_canvasGroup.blocksRaycasts = _value;
    }

    public void FadeMecDef_CG(float _fadeValue)
    {
        mecDefs_canvasGroup.alpha = _fadeValue;
    }

    public Transform MecDef_Pai()
    {
        return MecDef_Conteiner;
    }
}
