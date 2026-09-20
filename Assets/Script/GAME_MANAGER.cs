using UnityEngine;
using TMPro;

public class GAME_MANAGER : MonoBehaviour
{
    public static GAME_MANAGER Instance;

    [Header("Moedas")]
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

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateCoinTXT();
    }

    void UpdateCoinTXT()
    {
        coins_TXT.text = Coins.ToString() + " $";
    }

    public void AddCoins(int _coins) 
    { 
        Coins += _coins;
    }

    public void RemoveCoins(int _coins)
    {
        Coins -= _coins;
    }
}
