using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Mecanismo de Defesa", menuName ="Mecanismos")]
public class MEC_DEF_SO : ScriptableObject
{
    public MEC_DEF_SERIALIZED[] mecDefs;    
}

[Serializable]
public class MEC_DEF_SERIALIZED
{
    public string name;
    public int cost;
    public float dmg;
    public float dmgRate;
    public float dmgRadius;
    public float speedModifier;
    public float hp;
    public float placeRadius = 0.5f;
    public GameObject prefab;
}
