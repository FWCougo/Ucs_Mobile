using UnityEngine;
using UnityEngine.UI;

public class RECEIVE_DMG : MonoBehaviour, IDamageable
{
    [HideInInspector] public Image lifeImg;
    [SerializeField] protected float maxHP=10;
    [SerializeField] private float hp;
    public bool estaVivo = true;
    public float HP {  
        get { 
            return hp; 
        } 
        set {  
            hp = value; 
            AtualizarHp();
            if (hp <= 0) {
                Morrer();
            } 
        } 
    }

    public void Inicializar(float _maxHP, Image _lifeImg)
    {
        maxHP = _maxHP;
        hp = maxHP;
        if (lifeImg != null)
        {
            lifeImg = _lifeImg;
        }
        AtualizarHp();
    }

    protected virtual void Start()
    {
        estaVivo = true;
        hp = maxHP;
        AtualizarHp();
    }

    protected virtual void Morrer()
    {
        estaVivo = false;
        gameObject.SetActive(false);
        Destroy(gameObject, 1);
    }

    private void AtualizarHp()
    {
        if(lifeImg!=null)
        {
            lifeImg.fillAmount = hp/maxHP;
        }
    }

    public void ReceberDano(float _dano)
    {
        HP -= _dano;
    }
}