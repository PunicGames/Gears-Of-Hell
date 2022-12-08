using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LootLockerManager : MonoBehaviour
{
    public TMP_InputField memberID, playerScore;
    private readonly int id = 9400;
    private readonly int maxScores = 10;


    public GameObject scoreElement;
    public Transform scoreboardContent;


    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(InitializeLootLocker());
    }

    public void SubmitData()
    {
        StartCoroutine(SubmitScore());
    }

    public void UpdateData()
    {
        StartCoroutine(GetScores());
    }

    public IEnumerator InitializeLootLocker()
    {
        bool done = false;
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                print("Setting up LootLocker");
                done = true;
            }
            else
            {
                print(response.Error);
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
    }

    [System.Obsolete]
    public IEnumerator SubmitScore()
    {
        bool done = false;
        LootLockerSDKManager.SubmitScore(memberID.text, int.Parse(playerScore.text), id, (response) =>
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

        memberID.text = "";
        playerScore.text = "";

        yield return new WaitWhile(() => done == false);
    }

    [System.Obsolete]
    public IEnumerator GetScores()
    {
        bool done = false;
        LootLockerSDKManager.GetScoreList(id, maxScores, (response) =>
        {
            if (response.success)
            {
                foreach (Transform child in scoreboardContent.transform)
                {
                    Destroy(child.gameObject);
                }

                LootLockerLeaderboardMember[] scores = response.items;

                for (int i = 0; i < scores.Length; i++)
                {
                    GameObject scoreboardElement = Instantiate(scoreElement, scoreboardContent);
                    scoreboardElement.GetComponent<ScoreElement>().NewScoreElement("#" + scores[i].rank.ToString(), scores[i].member_id, GetHourMinuteSeconds(scores[i].score));
                }
                done = true;
            }
            else
            {
                print(response.Error);
                done = true;
            }
                
        });

        yield return new WaitWhile(() => done == false);
    }

    private string GetHourMinuteSeconds(int n)
    {
        var minutes = (n / 60).ToString();
        var seconds = (n % 60).ToString();

        if (seconds.Length == 1)
            seconds = "0" + seconds;

        return minutes + ":" + seconds;
    }


}
