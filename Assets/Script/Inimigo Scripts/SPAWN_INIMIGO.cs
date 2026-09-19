using System;
using System.Collections;
using UnityEngine;

public class SPAWN_INIMIGO : MonoBehaviour
{
    [SerializeField] ENEMYPOOL[] inimigoPool;
    [SerializeField] private int inimigoAtualPool;

    [SerializeField] private float spawnRate;

    [SerializeField] private float waitToStart=3;

    [SerializeField] private Transform inimigosConteiner;

    [ContextMenu("Começar a Spawnar")]
    public void ComeçarRound()
    {
        StartCoroutine("SpawnarInimigos");
    }

    public void ChangeSpawnRate(float f)
    {
        spawnRate = f;
    }

    /// <summary>
    /// Acelera o Spawn em X%. SpawnRate mínimo de 0.1/s
    /// </summary>
    /// <param name="porcentagem"></param>
    public void AcelerarSpawn(float porcentagem)
    {
        if (porcentagem > 100) porcentagem = 100;
        if (porcentagem < 0) porcentagem = 0;

        porcentagem /= 100;
        spawnRate -= (spawnRate * porcentagem);

        if (spawnRate < 0.1) spawnRate = 0.1f;
    }

    IEnumerator SpawnarInimigos()
    {
        WaitForSeconds _wait = new WaitForSeconds(waitToStart);
        yield return _wait;

        _wait = new WaitForSeconds(spawnRate);

        int _nInimigos = inimigoPool[inimigoAtualPool].qtdParaSpawnar;

        for (int i = 0; i < _nInimigos; i++)
        {
            INIMIGO _inimigo = inimigoPool[inimigoAtualPool].inimigo;
            _inimigo = Instantiate(_inimigo, transform.position, Quaternion.identity, inimigosConteiner);
            _inimigo.ReceberDestino();

            yield return _wait;
        }

        inimigoAtualPool++;

        yield return null;   
    }
}

[Serializable]
public class ENEMYPOOL
{
    public INIMIGO inimigo;
    public int qtdParaSpawnar=1;
}
