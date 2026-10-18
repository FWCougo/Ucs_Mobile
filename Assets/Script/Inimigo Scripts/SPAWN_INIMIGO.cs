using System;
using System.Collections;
using UnityEngine;

public class SPAWN_INIMIGO : MonoBehaviour
{
    [Header("WAVES")]
    [SerializeField] CONF_ENEMY_WAVES confWaves;
    [SerializeField] private int waveAtual;

    [Header("TEMPORIZADOR")]
    [SerializeField] private float waitToStart=3;

    [Header("NAO MEXER")]
    [SerializeField] private GameObject waveBTN;
    [SerializeField] private Transform inimigosConteiner;
    [SerializeField] private float spawnRate;

    [ContextMenu("Começar a Spawnar")]
    public void ComeçarRound()
    {
        StartCoroutine("SpawnarInimigos");
        waveBTN.SetActive(false);
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
        //PEGA O NUMERO DE CONFIGURACOES DE INIMIGOS QUE VAO SER SPAWNADOS NESSA WAVE
        int _nInimigosConfAtual = confWaves.waves[waveAtual].inimigo_e_quantidade.Length;
        int _idInimigoAtual = 0;

        //PEGA O NUMERO TOTAL DE INIMIGOS QUE VÃO APARECER NESSA WAVE
        int _nInimigosNaWave = 0;
        for (int i = 0; i < _nInimigosConfAtual; i++)
        {
            _nInimigosNaWave += confWaves.waves[waveAtual].inimigo_e_quantidade[i].qtdParaSpawnar;
        }
        GAME_MANAGER.Instance.SetInimigosDaWave(_nInimigosNaWave);

        //ESPERA X TEMPO PARA COMEÇAR O ROUND
        WaitForSeconds _wait = new WaitForSeconds(waitToStart);
        yield return _wait;

        //COMEÇA A SPAWNAR A CONFIGURACAO DE INIMIGOS ATUAL DA WAVE EM ANDAMENTO
        while (_idInimigoAtual < _nInimigosConfAtual)
        {
            ENEMY_E_QUANTITY _enemy_e_qtd_ATUAL = confWaves.waves[waveAtual].inimigo_e_quantidade[_idInimigoAtual];

            float currentSpawnRate = _enemy_e_qtd_ATUAL.rateDeSpawn;
            ChangeSpawnRate(currentSpawnRate);
            _wait = new WaitForSeconds(spawnRate);
            int _nInimigos = _enemy_e_qtd_ATUAL.qtdParaSpawnar;

            for (int i = 0; i < _nInimigos; i++)
            {
                INIMIGO _inimigo = _enemy_e_qtd_ATUAL.inimigo;
                _inimigo = Instantiate(_inimigo, transform.position, Quaternion.identity, inimigosConteiner);
                _inimigo.ReceberDestino();

                yield return _wait;
            }

            _idInimigoAtual++;
        }

        waveAtual++;

        while(GAME_MANAGER.Instance.InimigosDaWave > 0)
        {
            yield return null;
        }

        waveBTN.SetActive(true);

        yield return null;      
    }
}
