using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Mecanismo de Defesa", menuName ="Mecanismos")]
public class MEC_DEF_SO : ScriptableObject
{
    public MEC_DEF_SERIALIZED[] mecDefs;    
}

[Flags]
public enum TipoDefesa
{
    nenhum=0,
    disparoLinear =1<<0,
    obstaculo=1<<1,
    trapDeDano=1<<2,
    trapDeVelocidade=1<<3
}

[Serializable]
public class MEC_DEF_SERIALIZED
{
    [Header("Nome do Mecanismo")]
    public string name;
    [Header("Custo")]
    public int cost;
    [Header("Vida/Redistencia")]
    public float hp;
    [Header("Raio de Posicionamento")]
    public float placeRadius = 0.5f;
    [Header("Imagem")]
    public Sprite sprite;
    [Header("Prefab")]
    public GameObject prefab;
    [Header("Layer de Obstaculos")]
    public LayerMask obstacleLayer;
    [Header("Layer de Inimigo")]
    public LayerMask enemyLayer;

    [Header("Tipo de Defesa")]
    public TipoDefesa tpDefesa;

    [ShowIfFlag(nameof(tpDefesa), TipoDefesa.disparoLinear | TipoDefesa.trapDeDano)]
    public float dmg;
    [ShowIfFlag(nameof(tpDefesa), TipoDefesa.trapDeDano)]
    public float dmgRate;
    [ShowIfFlag(nameof(tpDefesa), TipoDefesa.trapDeDano)]
    public float dmgRadius;

    [ShowIfFlag(nameof(tpDefesa), TipoDefesa.disparoLinear)]
    public float shootRate;
    [ShowIfFlag(nameof(tpDefesa), TipoDefesa.disparoLinear)]
    public float shootRadius;
    [ShowIfFlag(nameof(tpDefesa), TipoDefesa.disparoLinear)]
    public float rotationSpeed;

    [ShowIfFlag(nameof(tpDefesa), TipoDefesa.trapDeVelocidade)]
    public float speedModifier;
    
}




