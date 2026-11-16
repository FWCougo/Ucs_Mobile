using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class LOJA_MECDEF_BTN : MonoBehaviour
{
    [SerializeField] private TMP_Text title_TXT;
    [SerializeField] private int currentLevel = 0;
    [SerializeField] private Image[] stars_IMG;
    [SerializeField] private Image main_IMG;
    [SerializeField] private MEC_DEF_SO mecDef_SO;

    private void OnEnable()
    {
        PegarLevel();
        AtivarEstrelas();
        SetarNome();
        SetarIMG();
    }
    private void PegarLevel()
    {
        switch (mecDef_SO.mecDefs[0].tpDefesa)
        {
            case TipoDefesa.obstaculo:
                currentLevel = GAME_MANAGER.Instance.ObstaculoLV-1;
                break;
            case TipoDefesa.trapDeVelocidade:
                currentLevel = GAME_MANAGER.Instance.SlowLV-1;
                break;
            case TipoDefesa.disparoLinear:
                currentLevel = GAME_MANAGER.Instance.TorretaLV-1;
                break;
            case TipoDefesa.trapDeDano:
                currentLevel = GAME_MANAGER.Instance.TrapLV-1;
                break;
        }
    }
    private void SetarIMG()
    {
        main_IMG.sprite = mecDef_SO.mecDefs[currentLevel].sprite;
    }
    private void SetarNome()
    {
        title_TXT.text = mecDef_SO.mecDefs[currentLevel].name;
    }
    private void AtivarEstrelas()
    {
        for (int i = 0; i < stars_IMG.Length; i++)
        {
            if (i <= currentLevel)
                stars_IMG[i].color = Color.yellow;
            else
                stars_IMG[i].color = new Color32(12, 12, 12, 191);
        }
    }

    public void EnviarParaLoja()
    {
        LOJA_MANAGER.Instance.ChangeSelected(mecDef_SO, currentLevel);
    }
}
