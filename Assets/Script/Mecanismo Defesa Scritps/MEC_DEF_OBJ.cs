using UnityEngine;
using UnityEngine.UI;

public class MEC_DEF_OBJ : MonoBehaviour { 
    [SerializeField] private MEC_DEF_SO mecDef_SO;
    [SerializeField] private SpriteRenderer mainSprite;
    [SerializeField] private int lv;

    [SerializeField] private RECEIVE_DMG receiveDMG;
    [SerializeField] private DMG_CONTINUO dmgContinuo;
    [SerializeField] private Image lifeImg;

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellowGreen;
        if(mecDef_SO != null ) Gizmos.DrawWireSphere(transform.position, mecDef_SO.mecDefs[lv].placeRadius);
    }

    #endif

    public void Inicializar(MEC_DEF_SO _mecDefSO, int _lv)
    {
        mecDef_SO = _mecDefSO;
        lv = _lv;

        float _hp = _mecDefSO.mecDefs[_lv].hp;
        float _dmg = _mecDefSO.mecDefs[_lv].dmg;
        float _dmgRadius = _mecDefSO.mecDefs[_lv].dmgRadius;
        float _dmgRate = _mecDefSO.mecDefs[_lv].dmgRate;

        if (_hp != -1)
        {
            receiveDMG = gameObject.AddComponent<RECEIVE_DMG>();
            receiveDMG.Inicializar(_hp,lifeImg);
        }

        if(_dmg > 0)
        {
            dmgContinuo = gameObject.AddComponent<DMG_CONTINUO>();
            dmgContinuo.Inicializar(_dmgRadius,_dmg,_dmgRate);
        }
        
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

}
