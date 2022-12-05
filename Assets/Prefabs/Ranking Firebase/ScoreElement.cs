using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreElement : MonoBehaviour
{

    public TMP_Text usernameText;
    public TMP_Text timeText;
    public TMP_Text xpText;

    public void NewScoreElement (string _username, int time, int _xp)
    {
        usernameText.text = _username;
        timeText.text = time.ToString();
        xpText.text = _xp.ToString();
    }

}
