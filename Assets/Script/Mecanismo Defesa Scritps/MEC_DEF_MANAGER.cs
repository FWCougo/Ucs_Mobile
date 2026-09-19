using UnityEngine;

public class MEC_DEF_MANAGER : MonoBehaviour
{
    public static MEC_DEF_MANAGER Instance;
    [SerializeField] private Transform MecDef_Conteiner;
    [SerializeField] private CanvasGroup mecDefs_canvasGroup;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        FadeMecDef_CG(1);
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
