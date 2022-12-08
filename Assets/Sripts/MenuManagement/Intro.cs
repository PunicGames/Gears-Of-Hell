using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}
