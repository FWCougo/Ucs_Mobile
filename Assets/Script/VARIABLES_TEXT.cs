using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class VARIABLES_TEXT : MonoBehaviour
{
    private TMP_Text text;
    [SerializeField] string textoAdicional;
    [SerializeField] private TipoDeTexto tipo;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        Invoke("SetarTexto",0.01f);
    }

    private void SetarTexto()
    {
        string _txt = "";

        switch (tipo)
        {
            case TipoDeTexto.Moedas:
                _txt = GAME_MANAGER.Instance.Coins.ToString();
                break;

            case TipoDeTexto.Parafuso:
                _txt = GAME_MANAGER.Instance.Parafusos.ToString();
                break;
        }

        text.text = $"{textoAdicional}{_txt}";
    }
}

public enum TipoDeTexto { Moedas, Parafuso};
