using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MEC_DEF_CANVA : MonoBehaviour
{
    [Header("Mecanismo Defesa REF")]
    [SerializeField] private MEC_DEF_OBJ mecDef;



    [Header("Canvas Local")]
    [SerializeField] private bool canvaLigado;
    [SerializeField] private Canvas canva;
    [Header("POSICIONAR")]
    [SerializeField] private GameObject painelPosicionar;
    [Header("VENDER")]
    private int custo = 0;
    [SerializeField] private TMP_Text custo_TXT;
    [SerializeField] private GameObject painelVender;
    [SerializeField] private GameObject botaoVenda;

    private void Start()
    {
        AtivarPainelPosicionar(true);
        CalcularCusto();
        custo_TXT.text = $"Sell {custo}";
    }

    void CalcularCusto()
    {
        custo = (int)(mecDef.MecDef_Serial.cost * 0.5f);
    }

    public void AtivarPainelPosicionar(bool value)
    {
        painelPosicionar.SetActive(value);
    }

    public void AtivarPainelVenda(bool value)
    {
        painelVender.SetActive(value);
    }

    public void Posicionar()
    {
        AtivarPainelPosicionar(false);
        botaoVenda.SetActive(true);
        AtivaCanva(false);
        mecDef.Comprar();
    }

    public void Cancelar()
    {
        mecDef.Cancelar();
    }

    public void Vender()
    {
        mecDef.Vender(custo);
    }

    public void PainelDeVenda(bool _value)
    {
        if (_value)
        {
            AtivarPainelVenda(true);
            botaoVenda.SetActive(false);
        }
        else
        {
            botaoVenda.SetActive(true);
            AtivarPainelVenda(false);
            AtivaCanva(false);
        }
    }

    public void AtivaCanva(bool value)
    {       
        if(canva!=null)
        {
            canva.gameObject.SetActive(value);
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
