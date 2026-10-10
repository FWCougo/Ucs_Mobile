using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class MEC_DEF_OBJ : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private MEC_DEF_SERIALIZED mecDef_Serial;
    public MEC_DEF_SERIALIZED MecDef_Serial
    {
        get { return mecDef_Serial; }
    }

    [SerializeField] private SpriteRenderer mainSprite;
    [SerializeField] private int lv;


    [Header("Canvas Local")]
    [SerializeField] private MEC_DEF_CANVA mecDef_Canva;

    
    private RECEIVE_DMG receiveDMG;
    private DMG_CONTINUO dmgContinuo;
    private MEC_DEF_TORRETA torreta;
    [SerializeField] private GameObject areaDeEfeito_GO;

    [SerializeField] private float placeRadius;
    private bool posicionado = false;

    [Header("COLISAO")]
    [SerializeField] private Collider col;
    [SerializeField] private NavMeshObstacle obstacle;

    private void Awake()
    {
        mecDef_Canva = GetComponentInChildren<MEC_DEF_CANVA>();        
    }

    private void Start(){
        AtivarColisoes(false);
        AtivarAreaDeEfeito(true);
        AtivaCanva(false);
        posicionado = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos(){
        Gizmos.color = Color.yellowGreen;
        if(mecDef_Serial != null ) Gizmos.DrawWireSphere(transform.position, placeRadius);
    }
#endif

    private void AtivarColisoes(bool value)
    {
        if(col!=null) col.enabled = value;
        if (obstacle != null) obstacle.enabled = value;
    }

    private void AtivarAreaDeEfeito(bool value)
    {
        if(areaDeEfeito_GO!=null) areaDeEfeito_GO.SetActive(value);
    }

    public void ReceberConfiguracoes(MEC_DEF_SO _mecDefSO, int _lv)
    {
        mecDef_Serial = _mecDefSO.mecDefs[_lv];
        lv = _lv;
    }

    public void Inicializar(){

        AtivarColisoes(true);
        AtivarAreaDeEfeito(false);
        TrocarCorSprite(Color.white);       

        placeRadius = mecDef_Serial.placeRadius;       

        float _hp = mecDef_Serial.hp;
        float _dmg = mecDef_Serial.dmg;
        float _dmgRadius = mecDef_Serial.dmgRadius;
        float _dmgRate = mecDef_Serial.dmgRate;
        float _shootRate = mecDef_Serial.shootRate;
        float _shootRadius = mecDef_Serial.shootRadius;
        float _rotSpeed = mecDef_Serial.rotationSpeed;
        LayerMask enemyLayer = mecDef_Serial.enemyLayer;
        TipoDefesa tpDef = mecDef_Serial.tpDefesa;

        if (tpDef == TipoDefesa.trapDeDano || tpDef == TipoDefesa.trapDeVelocidade || tpDef == TipoDefesa.disparoLinear || tpDef == TipoDefesa.obstaculo)
        {
            receiveDMG = gameObject.AddComponent<RECEIVE_DMG>();
            receiveDMG.Inicializar(_hp, receiveDMG.lifeImg);
        }

        if(tpDef == TipoDefesa.trapDeDano){
            dmgContinuo = gameObject.AddComponent<DMG_CONTINUO>();
            dmgContinuo.Inicializar(_dmgRadius,_dmg,_dmgRate);
        }

       // if(tpDef == TipoDefesa.disparoLinear)
       // {
       //     torreta = gameObject.AddComponent<MEC_DEF_TORRETA>();
       //     torreta.Inicializar(_dmg, _shootRadius, _shootRate, _rotSpeed, enemyLayer);
       // }
    }

    public void TrocarTransparencia(float _alpha)
    {
        Color _col;

        _col = mainSprite.color;
        _col.a = _alpha;
        mainSprite.color = _col;
    }

    /// <summary>
    /// Altera a cor do Mecanismo se não pode posicionar. Passar 'true' se pode posicionar, 'false' se não pode.
    /// </summary>
    /// <param name="_pode"></param>
    public void PodePosicionar(bool _pode)
    {
        if(!_pode)
        {
           TrocarCorSprite(Color.red);
        }
        else
        {
            TrocarCorSprite(Color.forestGreen);
        }
    }

    public void TrocarCorSprite(Color _color)
    {
        mainSprite.color =_color;
    }

    public void Comprar()
    {
        int _cost = mecDef_Serial.cost;
        GAME_MANAGER.Instance.RemoveCoins(_cost);
        MEC_DEF_MANAGER.Instance.AtivarPainelDosMecanismos(true);
        posicionado = true;
        
        Inicializar();
    }

    public void Cancelar()
    {
        MEC_DEF_MANAGER.Instance.AtivarPainelDosMecanismos(true);
        Destroy(gameObject);
    }

    public void Vender(int _preco)
    {
        GAME_MANAGER.Instance.AddCoins(_preco);
        Destroy(gameObject);
    }

    public void AtivaCanva(bool value)
    {
        mecDef_Canva.AtivaCanva(value);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!posicionado) return;
        mecDef_Canva.AtivaCanva();
    }
}
