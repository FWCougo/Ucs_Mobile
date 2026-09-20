using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class MEC_DEF_UI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("ScriptableOBJ e Prefab")]
    [SerializeField] private MEC_DEF_SO mecDef_SO;
    [SerializeField] private MEC_DEF_OBJ mecDef_OBJ;

    [Header("Layer")]
    [SerializeField] LayerMask obstacleLayer;

    [Header("Level")]
    [SerializeField] private int lv = 1;

    [Header("Cost")]
    [SerializeField] private TMP_Text cost_TXT;

    [Header("Other")]
    [SerializeField] private CanvasGroup canvasGroup;

    Vector3 worldPos;
    Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    void Start()
    {
        UpdateCostUI();
    }

    void AlterarCG(float _alpha)
    {
        canvasGroup.alpha = _alpha;
    }

    void UpdateCostUI()
    {
        int _cost = mecDef_SO.mecDefs[lv - 1].cost;

        cost_TXT.text = _cost.ToString();
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

        return P;
    }

    bool PodePosicionar()
    {
        Collider[] cols = Physics.OverlapSphere(worldPos, mecDef_SO.mecDefs[lv-1].placeRadius, obstacleLayer);

        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].gameObject != mecDef_OBJ.gameObject)
            {
                return false;
            }
        }

        return true;
    }

    void ChecaDisponibilidade()
    {
        if (!ChecarSeTemDinheiro())
        {
            AlterarCG(0.5f);
        }
    }

    bool ChecarSeTemDinheiro()
    {
        int _cost = mecDef_SO.mecDefs[lv - 1].cost;
        int _caixinha = GAME_MANAGER.Instance.Coins;

        if (_cost > _caixinha) 
        { 
            return false; 
        }
        else 
        {             
            return true;
        }
    }

    void Comprar()
    {
        int _cost = mecDef_SO.mecDefs[lv - 1].cost;

        GAME_MANAGER.Instance.RemoveCoins(_cost);
    }

    #region Drag Methods
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!ChecarSeTemDinheiro()) return;

        worldPos = GetWorldPoint(eventData.position);
        worldPos.y = 0;

        mecDef_OBJ = InstanciarPrefab(worldPos);

        MEC_DEF_MANAGER.Instance.FadeMecDef_CG(0.25f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!ChecarSeTemDinheiro()) return;

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
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(!ChecarSeTemDinheiro()) return; 

        MEC_DEF_MANAGER.Instance.FadeMecDef_CG(1);

        if (PodePosicionar())
        {
            mecDef_OBJ.Inicializar(mecDef_SO, lv-1);
            Comprar();

            ChecaDisponibilidade();
        }
        else
        {
            Destroy(mecDef_OBJ.gameObject);
        }

    }
    #endregion

}
