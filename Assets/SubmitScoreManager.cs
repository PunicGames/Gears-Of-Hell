using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SubmitScoreManager : MonoBehaviour
{
    
    public TMP_Text phrase;
    public GameRegistry gameRegistry;
    public TMP_InputField nickname;
    private int score;

    public GameObject ranking, submit;

    public void OnEnable()
    {
        score = gameRegistry.GetScore();
        phrase.text = GetMotivationalPhrase();
    }

    private string GetMotivationalPhrase()
    {
        return "Wow, you are incredible!'";
    }

    public void SendData()
    {
        // checkeamos que el nombre sea correcto
        var nick = nickname.text;
        if (nick == "")
            return;

        // mandamos la info al leatherboard
        StartCoroutine(SubmitScore(nick));
    }


    [System.Obsolete]
    public IEnumerator SubmitScore(string nickname)
    {
        bool done = false;
        LootLockerSDKManager.SubmitScore(nickname, score, LeatherboardManager.id, (response) =>
        {
            if (response.statusCode == 200)
            {
                print("Score submitted successfull");
                done = true;
            }
            else
            {
                print(response.Error);
                done = true;
            }
        });

        submit.active = false;
        ranking.active = true;

        yield return new WaitWhile(() => done == false);
    }
}
