using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MEC_DEF_OBJ : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private MEC_DEF_SO mecDef_SO;
    [SerializeField] private SpriteRenderer mainSprite;
    [SerializeField] private int lv;

    [Header("Canvas Local")]
    [SerializeField] private bool canvaLigado;
    [SerializeField] private Canvas canva;
    [SerializeField] private GameObject painelCtz;
    [SerializeField] private GameObject botaoVenda;

    private RECEIVE_DMG receiveDMG;
    private DMG_CONTINUO dmgContinuo;
    private MEC_DEF_TORRETA torreta;

    [SerializeField] private float placeRadius;

    [Header("COLISAO")]
    [SerializeField] private Collider col;

    private void Awake()
    {
        canva = GetComponentInChildren<Canvas>();
        canva.gameObject.SetActive(false);
    }

    private void Start(){
        col.enabled = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos(){
        Gizmos.color = Color.yellowGreen;
        if(mecDef_SO != null ) Gizmos.DrawWireSphere(transform.position, placeRadius);
    }
#endif

    public void Inicializar(MEC_DEF_SO _mecDefSO, int _lv){
        col.enabled = true;

        placeRadius = _mecDefSO.mecDefs[_lv].placeRadius;

        mecDef_SO = _mecDefSO;
        lv = _lv;

        float _hp = _mecDefSO.mecDefs[_lv].hp;
        float _dmg = _mecDefSO.mecDefs[_lv].dmg;
        float _dmgRadius = _mecDefSO.mecDefs[_lv].dmgRadius;
        float _dmgRate = _mecDefSO.mecDefs[_lv].dmgRate;

        if (_hp != -1){
            receiveDMG = gameObject.AddComponent<RECEIVE_DMG>();
            receiveDMG.Inicializar(_hp, receiveDMG.lifeImg);
        }

        if(_dmg > 0){
            dmgContinuo = gameObject.AddComponent<DMG_CONTINUO>();
            dmgContinuo.Inicializar(_dmgRadius,_dmg,_dmgRate);
        }
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
            mainSprite.color = Color.red;
        }
        else
        {
            mainSprite.color = Color.white;
        }
        
    }


    public void PainelDeVenda(bool _value){
        if(_value)
        {
            painelCtz.SetActive(true);
            botaoVenda.SetActive(false);
        }
        else
        {
            botaoVenda.SetActive(true);
            painelCtz.SetActive(false);
        }        
    }

    public void Vender()
    {
        int _preco = (int)(mecDef_SO.mecDefs[lv].cost*0.5f);
        GAME_MANAGER.Instance.AddCoins(_preco);
        Destroy(gameObject);
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

    public void OnPointerClick(PointerEventData eventData)
    {
        AtivaCanva();
    }
}
