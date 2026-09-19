using UnityEngine;
using UnityEngine.UI;

public class RECEIVE_DMG : MonoBehaviour, IDamageable
{
    [SerializeField] private Image lifeImg;
    [SerializeField] protected float maxHP=10;
    [SerializeField] private float hp;
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
        lifeImg = _lifeImg;
        AtualizarHp();
    }

    protected virtual void Start()
    {
        hp = maxHP;
        AtualizarHp();
    }

    protected virtual void Morrer()
    {
        gameObject.SetActive(false);
        Destroy(gameObject, 1);
    }

    private void AtualizarHp()
    {
        lifeImg.fillAmount = hp/maxHP;
    }

    public void ReceberDano(float _dano)
    {
        HP -= _dano;
    }
}