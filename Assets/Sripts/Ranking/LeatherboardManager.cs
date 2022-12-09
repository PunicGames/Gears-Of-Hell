using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeatherboardManager : MonoBehaviour
{

    public static readonly int id = 9400;
    public int maxScores = 10;


    public GameObject scoreElement;
    public Transform scoreboardContent;

    public void OnEnable()
    {
        StartCoroutine(GetScores());
    }
    public void UpdateData()
    {
        StartCoroutine(GetScores());
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
