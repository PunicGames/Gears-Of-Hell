using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SubmitScoreManager : MonoBehaviour
{
    
    public TMP_Text phrase;
    public TMP_Text faultPhrase;
    public TMP_InputField nickname;
    public GameRegistry gameRegistry;
    private int score;

    private readonly string[] tierOnePhrase =   { "Sometimes life is complicated.",
                                                    "Don't worry, it happens to all of us.",
                                                    "Good for not having a keyboard, isn't it?",
                                                    "Next time turn on the screen.",
                                                    "Sorry in the shop we don't sell hands to play this game.",
                                                    "Have you thought about starting to use a unicycle as a method of transport?" 
                                                };
    private readonly string[] tierTwoPhrase =   { "Well well well, you're getting the hang of it.",
                                                    "Impressive, now try to dodge.",
                                                    "Good game, but it's about surviving as long as possible.",
                                                    "So you've tried the rifle, haven't you?",
                                                    "That was fine, but remember the bullets hurts.",
                                                    "That was good, but don't freak out either."
                                                };

    private readonly string[] tierThreePhrase =   { "I think you know what you are doing.",
                                                    "There's a future number one in you boy, keep it up.",
                                                    "That was a long time, wasn't it?",
                                                    "I'd have another game, you were very close to making it, my boy.",
                                                    "Sometimes it gets messy, yes.",
                                                    "Try the electric barrier, it will probably save you next time."
                                                };
    private readonly string[] tierFourPhrase =   { "From PunicGames thank you very much for your support.",
                                                    "I can't give you much advice, have you been drinking water?",
                                                    "We are probably looking at a new record.",
                                                    "You are a god of GEARS OF HELL"
                                                };


    public GameObject ranking, submit, fault;

    public void OnEnable()
    {
        score = gameRegistry.GetScore();
        phrase.text = GetMotivationalPhrase();
    }

    private string GetMotivationalPhrase()
    {
        if (score <= 60)
        {
            return tierOnePhrase[Random.Range(0, tierOnePhrase.Length)];
        }
        else if (score <= 600)
        {
            return tierTwoPhrase[Random.Range(0, tierTwoPhrase.Length)];
        }
        else if (score <= 1800)
        {
            return tierThreePhrase[Random.Range(0, tierThreePhrase.Length)];
        } 
        else
        {
            return tierFourPhrase[Random.Range(0, tierFourPhrase.Length)];
        }
    }

    public void SendData()
    {
        var cachedName = nickname.text;
        // checkeamos que el nombre sea correcto
        if (string.IsNullOrEmpty(cachedName) || string.IsNullOrWhiteSpace(cachedName))
        {
            faultPhrase.text = "Empty name.";
            fault.active = true;
            return;
        }
        else if (cachedName.Length >= 14) {
            faultPhrase.text = "Whoops, this name is to big.";
            fault.active = true;
            return;
        }
            

        // mandamos la info al leatherboard
        StartCoroutine(SubmitScore(cachedName));
    }


    [System.Obsolete]
    public IEnumerator SubmitScore(string nickname)
    {
        bool done = false;
        LootLockerSDKManager.SubmitScore(nickname, score, LeaderboardManager.id, (response) =>
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

        yield return new WaitWhile(() => done == false);

        submit.active = false;
        ranking.active = true;
    }
}
