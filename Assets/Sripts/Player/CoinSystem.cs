using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinSystem : MonoBehaviour
{
    public int totalCoinsInGame;

    // Display coin
    private TextMeshProUGUI coinDisplay;

    // PopUp
    private PopUp popup;
    [SerializeField] private Transform popupPosition;

    void Start()
    {
        totalCoinsInGame = 0;
        coinDisplay = GameObject.Find("CoinCounter").GetComponent<TextMeshProUGUI>();
        popup = GetComponent<PopUp>();
    }

    public void AddCoin(int newCoin) {
        totalCoinsInGame += newCoin;
        coinDisplay.text = totalCoinsInGame.ToString();
        popup.Create(popupPosition.position, newCoin, PopUp.TypePopUp.MONEY, true, 0.5f);
    }

    public void SpendCoin(int newCoin)
    {
        totalCoinsInGame -= newCoin;
        coinDisplay.text = totalCoinsInGame.ToString();
        popup.Create(popupPosition.position, newCoin, PopUp.TypePopUp.MONEY, false, 0.5f);
    }
}
