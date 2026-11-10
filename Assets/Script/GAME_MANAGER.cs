using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.AI;

public class GAME_MANAGER : MonoBehaviour
{
    public static GAME_MANAGER Instance;

    [SerializeField] private bool pause;

    [Header("FASES")]
    [SerializeField] private int levels = 0;
    public int Levels
    {
        get { return levels; }
        private set
        {
            levels = value;
        }
    }

    [Header("MOEDAS")]
    [SerializeField] private int coins = 0;
    public int Coins
    {
        get { return coins; }
        private set 
        {
            coins = value;
            UpdateCoinTXT();
        }
    }
    [SerializeField] private TMP_Text coins_TXT;

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


        CarregarLevel();
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
        PlayerPrefs.SetInt("UNLOCKED_LEVELS", Levels);
    }
    private void CarregarLevel()
    {
        Levels = PlayerPrefs.GetInt("UNLOCKED_LEVELS");

        if(Levels == 0)
        {
            Levels = 1;
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
