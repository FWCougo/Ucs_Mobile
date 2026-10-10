using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MEC_DEF_CANVA : MonoBehaviour
{

    [SerializeField] private MEC_DEF_OBJ mecDef;

    [Header("Canvas Local")]
    [SerializeField] private bool canvaLigado;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Canvas canva;
    [SerializeField] private GameObject painelCtz;
    [SerializeField] private GameObject botaoVenda;

    public void Vender()
    {
        mecDef.Vender();
    }

    public void PainelDeVenda(bool _value)
    {
        text.text = $"Sell {mecDef.MecDef_Serial.cost * 0.5f}";

        if (_value)
        {
            painelCtz.SetActive(true);
            botaoVenda.SetActive(false);
        }
        else
        {
            botaoVenda.SetActive(true);
            painelCtz.SetActive(false);
            AtivaCanva();
        }
    }

    public void AtivaCanva()
    {
        canvaLigado = !canvaLigado;

        if (canvaLigado)
        {
            canva.gameObject.SetActive(true);
        }
        else
        {
            canva.gameObject.SetActive(false);
        }
    }
}
