using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.AI;

public class GAME_MANAGER : MonoBehaviour
{
    public static GAME_MANAGER Instance;

    [SerializeField] private SPAWN_INIMIGO spawnInimigo;

    [SerializeField] private bool pause;
    private CASA casa;

    [Header("FASES")]
    [SerializeField] private LEVEL_SO[] levels_SO;
    [SerializeField] private int selectedLevel = 0;
    [SerializeField] private int unlockedLevels = 0;
    public int UnlockedLevels
    {
        get { return unlockedLevels; }
        private set
        {
            unlockedLevels = value;
            SalvarLevel();
        }
    }
    [SerializeField] private int currentLevel = 0;

    [Header("MOEDAS")]
    [SerializeField] private int coins = 0;
    public int Coins
    {
        get { return coins; }
        private set 
        {
            coins = value;
            UpdateCoinTXT();
            SalvarMoedas();
        }
    }
    [SerializeField] private TMP_Text coins_TXT;

    [SerializeField] private int coinsSpent = 0;
    public int CoinsSpent
    {
        get { return coinsSpent; }
        private set
        {
            coinsSpent = value;
        }
    }

    [Header("PARAFUSOS")]
    [SerializeField] private int parafusos = 0;
    public int Parafusos
    {
        get { return parafusos; }
        private set
        {
            parafusos = value;
            SalvarParafusos();
        }
    }


    [Header("UPGRADE LEVELS")]
    [SerializeField] private int torretaLV = 0;
    public int TorretaLV
    {
        get { return torretaLV; }
        set
        {
            torretaLV = value;
            SalvarUpgrades();
        }
    }

    [SerializeField] private int obstaculoLV = 0;
    public int ObstaculoLV
    {
        get { return obstaculoLV; }
        set
        {
            obstaculoLV = value;
            SalvarUpgrades();
        }
    }

    [SerializeField] private int trapLV = 0;
    public int TrapLV
    {
        get { return trapLV; }
        set
        {
            trapLV = value;
            SalvarUpgrades();
        }
    }

    [SerializeField] private int slowLV = 0;
    public int SlowLV
    {
        get { return slowLV; }
        set
        {
            slowLV = value;
            SalvarUpgrades();
        }
    }


    [Header("INIMIGOS")]
    [SerializeField] private int inimigosDaWave = 0;
    public int InimigosDaWave
    {
        get { return inimigosDaWave; }
        private set
        {
            inimigosDaWave = value;
        }
    }
    [SerializeField] private int inimigosRestantes = 0;
    public int InimigosRestantes
    {
        get { return inimigosRestantes; }
        private set
        {
            inimigosRestantes = value;
            UpdateInimigosRestantesTXT();
        }
    }
    [SerializeField] private TMP_Text inimigosRestantes_TXT;

    [SerializeField] private int totalInimigosMortos = 0;
    public int TotalInimigosMortos
    {
        get { return totalInimigosMortos; }
        private set
        {
            totalInimigosMortos = value;
        }
    }
    [SerializeField] private TMP_Text totalInimigosMortos_TXT;

    private void Awake()
    {
        if(Instance==null)
            Instance = this;
        else 
            Destroy(gameObject);


        CarregarUpgrades();
        CarregarLevel();
        CarregarMoedas();
        CarregarParafusos();
    }

    private void Start()
    {        
        UpdateCoinTXT();
    }

    #region Coins
    void UpdateCoinTXT()
    {
        coins_TXT.text = Coins.ToString();
    }

    public void AddCoins(int _coins) 
    { 
        Coins += _coins;
    }

    public void RemoveCoins(int _coins)
    {
        Coins -= _coins;
        coinsSpent+= _coins;
    }

    /// <summary>
    /// Passe o CUSTO de alguma coisa como parametro. Retorna verdadeiro se pode comprar, ou falso
    /// </summary>
    /// <param name="_custo"></param>
    /// <returns></returns>
    public bool ChecarSeTemDinheiro(int _custo)
    {
        if (_custo > Coins)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void SalvarMoedas()
    {
        PlayerPrefs.SetInt("TOTAL_MOEDAS", Coins);
    }
    private void CarregarMoedas()
    {
        Coins = PlayerPrefs.GetInt("TOTAL_MOEDAS");
    }
    #endregion

    #region Parafusos
    public void AddParafuso(int _parafuso)
    {
        Parafusos += _parafuso;
    }
    public void RemoveParafuso(int _parafuso)
    {
        Parafusos -= _parafuso;
    }

    /// <summary>
    /// Passe o CUSTO de alguma coisa como parametro. Retorna verdadeiro se pode comprar, ou falso
    /// </summary>
    /// <param name="_parafuso"></param>
    /// <returns></returns>
    public bool ChecarSeTemParafusos(int _parafuso)
    {
        if (_parafuso > Parafusos)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    private void SalvarParafusos()
    {
        PlayerPrefs.SetInt("TOTAL_PARAFUSOS", Parafusos);
    }
    private void CarregarParafusos()
    {
        Parafusos = PlayerPrefs.GetInt("TOTAL_PARAFUSOS");
    }
    #endregion

    #region Inimigos
    void UpdateInimigosMortosTXT()
    {
        totalInimigosMortos_TXT.text = $"Destroyed : {totalInimigosMortos}";
    }


    void UpdateInimigosRestantesTXT()
    {
        inimigosRestantes_TXT.text = $"{InimigosRestantes}/{InimigosDaWave}";
    }

    public void SetInimigosDaWave(int _inimigos)
    {
        InimigosDaWave = _inimigos;
    }
    public void SetInimigosRestantes(int _inimigos)
    {
        InimigosRestantes = _inimigos;
    }

    public void RemoveInimigosDaWave()
    {
        InimigosRestantes--;
    }

    #endregion

    #region Level
    private void SalvarLevel()
    {
        PlayerPrefs.SetInt("UNLOCKED_LEVELS", UnlockedLevels);
    }
    private void CarregarLevel()
    {
        UnlockedLevels = PlayerPrefs.GetInt("UNLOCKED_LEVELS");

        if(UnlockedLevels == 0)
        {
            UnlockedLevels = 1;
        }
    }
    #endregion

    #region Upgrades
    private void SalvarUpgrades()
    {
        PlayerPrefs.SetInt("OBSTACULO_LV", obstaculoLV);
        PlayerPrefs.SetInt("SLOW_LV", slowLV);
        PlayerPrefs.SetInt("TRAP_LV", trapLV);
        PlayerPrefs.SetInt("TORRETA_LV", torretaLV);
    }
    private void CarregarUpgrades()
    {
        obstaculoLV = PlayerPrefs.GetInt("OBSTACULO_LV");
        slowLV = PlayerPrefs.GetInt("SLOW_LV");
        trapLV = PlayerPrefs.GetInt("TRAP_LV");
        torretaLV = PlayerPrefs.GetInt("TORRETA_LV");

        if (obstaculoLV == 0)
        {
            obstaculoLV = 1;
        }
        if (slowLV == 0)
        {
            slowLV = 1;
        }
        if (trapLV == 0)
        {
            trapLV = 1;
        }
        if (torretaLV == 0)
        {
            torretaLV = 1;
        }

    }
    #endregion

    [SerializeField] NavMeshAgent[] allAgents;
    private void GetAllNavMeshAgents()
    {
        allAgents = FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None);
    }
    private void ChangeAgentsState(bool _value)
    {
        foreach (NavMeshAgent agent in allAgents)
        {
            agent.enabled = _value;

            //if (agent.isActiveAndEnabled)
            //{
            //    agent.isStopped = _value;
            //}
        }
    }

    public void WON()
    {
        MENU_MANAGER.Instance.OpenMenu("WON_MENU");

        casa = FindFirstObjectByType<CASA>();

        int _HpDaCasa = ((int)casa.HP/(int)casa.MaxHP)*100;

        CANVAS_MANAGER.Instance.SetarDinheiroGasto_TXT(coinsSpent);
        CANVAS_MANAGER.Instance.SetarHPCasa_TXT(_HpDaCasa);

        if (unlockedLevels <= selectedLevel)
        {
            UnlockedLevels++;
            CANVAS_MANAGER.Instance.ActivateLevelButtons();
            AddParafuso((int)PontuacaoDoNivel());
        }
        else
        {
            AddParafuso((int)PontuacaoDoNivel() / 2);
        }
    }

    private float PontuacaoDoNivel()
    {
        float _pontuacaoTotal = 100;

        if(coinsSpent < 500)
        {
            _pontuacaoTotal *= 1;
        }
        else if(coinsSpent < 1000) 
        {
            _pontuacaoTotal *= 0.8f;
        }
        else
        {
            _pontuacaoTotal *= 0.6f;
        }

        return _pontuacaoTotal *= ((int)casa.HP / (int)casa.MaxHP);

    }

    public void StartGame(int _lv)
    {
        StartCoroutine(LoadGameScene(_lv));
    }

    private IEnumerator LoadGameScene(int _lv)
    {
        if (selectedLevel != 0)
        {
            yield return SceneManager.UnloadSceneAsync(selectedLevel);
        }

        selectedLevel = _lv;

        AsyncOperation operation = SceneManager.LoadSceneAsync(
            selectedLevel,
            LoadSceneMode.Additive
        );

        // Aguarda o carregamento terminar
        yield return operation;

        Transform _spawnTrans = GameObject.FindGameObjectWithTag("enemy_spawn").GetComponent<Transform>();

        LEVEL_SO _level_SO = levels_SO[_lv - 1];

        Coins = _level_SO.levelCoins;

        spawnInimigo.Trocar_CONF_ENEMY_WAVES(_level_SO.confWaves, _spawnTrans);

        // A cena terminou de carregar
        coinsSpent = 0;
        MENU_MANAGER.Instance.OpenMenu("GAME_MENU");
    }

    public void Pause()
    {
        pause = !pause;

        if(pause){
           // GetAllNavMeshAgents();
            Time.timeScale = 0;
            MENU_MANAGER.Instance.OpenMenu("PAUSE_MENU");
           // ChangeAgentsState(false);
        }
        else{
            Time.timeScale = 1;
            MENU_MANAGER.Instance.OpenMenu("GAME_MENU");
            //ChangeAgentsState(true);
        }

        

    } 
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
