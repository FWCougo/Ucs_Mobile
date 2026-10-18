using System;
using UnityEngine;

[CreateAssetMenu(fileName = "WAVE")]
public class CONF_ENEMY_WAVES : ScriptableObject
{
    public ENEMY_WAVE[] waves;
}

[Serializable]
public class ENEMY_WAVE
{
    public ENEMY_E_QUANTITY[] inimigo_e_quantidade;
}

[Serializable]
public class ENEMY_E_QUANTITY
{
    public INIMIGO inimigo;
    public int qtdParaSpawnar = 1;
    public float rateDeSpawn = 0.5f;
}