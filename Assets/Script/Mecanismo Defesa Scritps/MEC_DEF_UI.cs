using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class MEC_DEF_UI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("ScriptableOBJ e Prefab")]
    [SerializeField] private MEC_DEF_SO mecDef_SO;
    [SerializeField] private MEC_DEF_OBJ mecDef_OBJ;

    [Header("Imagem")]
    [SerializeField] private Image img;

    [Header("Level")]
    [SerializeField] private int lv = 1;

    [Header("Cost")]
    [SerializeField] private TMP_Text cost_TXT;

    [Header("Offset")]
    private float offsetZ = 1f;

    [Header("Other")]
    [SerializeField] private CanvasGroup canvasGroup;

    Vector3 worldPos;
    Camera cam;

    void Awake()
    {
        cam = Camera.main;
        offsetZ = 1;
    }

    void Start()
    {
        UpdateCostUI();
        SetarImagem();
    }

    void SetarImagem()
    {
        img.sprite = mecDef_SO.mecDefs[lv - 1].sprite;
    }

    void AlterarCG(float _alpha)
    {
        canvasGroup.alpha = _alpha;
    }

    void UpdateCostUI()
    {
        int _cost = mecDef_SO.mecDefs[lv - 1].cost;

        cost_TXT.text = _cost.ToString() + " P$";
    }

    MEC_DEF_OBJ InstanciarPrefab(Vector3 _posiiton)
    {
        Transform _pai = MEC_DEF_MANAGER.Instance.MecDef_Pai();

        MEC_DEF_OBJ _mecObj = Instantiate(mecDef_SO.mecDefs[lv-1].prefab, _posiiton, Quaternion.identity, _pai).GetComponent<MEC_DEF_OBJ>();

        return _mecObj;

    }

    Vector3 GetWorldPoint(Vector3 _pos)
    {
        Ray _r = cam.ScreenPointToRay(_pos);

        Vector3 PO = Vector3.zero;
        Vector3 PN = -cam.transform.forward;
        float t = Vector3.Dot(PO - _r.origin, PN) / Vector3.Dot(_r.direction, PN);
        Vector3 P = _r.origin + _r.direction * t;

        Debug.DrawLine(cam.transform.position, P);

        return P + new Vector3(0,0,offsetZ);
    }

    bool PodePosicionar()
    {
        Collider[] cols = Physics.OverlapSphere(worldPos, mecDef_SO.mecDefs[lv-1].placeRadius, mecDef_SO.mecDefs[lv - 1].obstacleLayer);

        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].gameObject != mecDef_OBJ.gameObject)
            {
                return false;
            }
        }

        return true;
    }

    bool TemDinheiroSuficiente()
    {
        int _custo = mecDef_SO.mecDefs[lv - 1].cost;

        return GAME_MANAGER.Instance.ChecarSeTemDinheiro(_custo);
    }

    #region Drag Methods
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!TemDinheiroSuficiente()) return;

        worldPos = GetWorldPoint(eventData.position);
        worldPos.y = 0;

        mecDef_OBJ = InstanciarPrefab(worldPos);

        mecDef_OBJ.ReceberConfiguracoes(mecDef_SO,lv-1);

        MEC_DEF_MANAGER.Instance.AtivarPainelDosMecanismos(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!TemDinheiroSuficiente()) return;

        worldPos = GetWorldPoint(eventData.position);
        worldPos.y = 0;
        if (mecDef_OBJ != null)
        {
            mecDef_OBJ.transform.position = worldPos;
            if(PodePosicionar()) 
            {
                mecDef_OBJ.PodePosicionar(true);
            }
            else
            {
                mecDef_OBJ.PodePosicionar(false);
            }

            mecDef_OBJ.TrocarTransparencia(0.5f);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        mecDef_OBJ.AtivaCanva(true);
    }
    #endregion

}
