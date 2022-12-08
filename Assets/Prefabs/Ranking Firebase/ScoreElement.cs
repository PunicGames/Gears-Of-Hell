using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreElement : MonoBehaviour
{

    public TMP_Text usernameText;
    public TMP_Text timeText;
    public TMP_Text xpText;
    public TMP_Text ordinal;

    public void NewScoreElement (string _ordinal, string _username, string _time, int _xp)
    {
        ordinal.text = _ordinal;
        usernameText.text = _username;
        timeText.text = _time;
        xpText.text = _xp.ToString();
    }

}
