using UnityEngine;

public class MED_DEF_TORRETA : MonoBehaviour
{
    [SerializeField] private INIMIGO inimigoAtual;
    [SerializeField] private bool atirando;


    private void PegarInimigoMaisProximo()
    {

    }

    private void FixedUpdate()
    {
        if(!atirando) PegarInimigoMaisProximo();
    }

}
