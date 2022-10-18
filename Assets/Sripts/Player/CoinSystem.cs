using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinSystem : MonoBehaviour
{
    public int totalCoinsInGame;

    // Display coin
    private Text coinDisplay;
    void Start()
    {
        totalCoinsInGame = 0;
        coinDisplay = GameObject.Find("CoinCounter").GetComponent<Text>();
    }

    public void AddCoin(int newCoin) {
        totalCoinsInGame += newCoin;
        coinDisplay.text = totalCoinsInGame.ToString();
    }
}
