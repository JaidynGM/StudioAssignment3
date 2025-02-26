using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private InputManager inputManager;
    private int coinCount = 0;

    private Coin[] coins;

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        coins = FindObjectsByType<Coin>(FindObjectsSortMode.None);
    }

    private void Start()
    {
        inputManager.OnResetPressed.AddListener(ResetCoins); 
    }

    public void AddCoin()
    {
        coinCount++;
        UpdateCoinUI();
    }

    private void ResetCoins()
    {
        coinCount = 0;
        UpdateCoinUI();

        foreach (Coin coin in coins)
        {
            coin.Respawn();
        }
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = $"Coins: {coinCount}";
        }
    }
}
