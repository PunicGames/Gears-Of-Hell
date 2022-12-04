using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    //Screen object variables
    public GameObject loginUI;
    public GameObject registerUI;
    public GameObject dataUI;
    public GameObject rankingUI;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    //Functions to change the login screen UI
    public void LoginScreen() //Back button
    {
        loginUI.SetActive(true);
        registerUI.SetActive(false);
        dataUI.SetActive(false);
        rankingUI.SetActive(false);
    }
    public void RegisterScreen() // Regester button
    {
        loginUI.SetActive(false);
        registerUI.SetActive(true);
        dataUI.SetActive(false);
        rankingUI.SetActive(false);
    }

    public void DataScreen()
    {
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        dataUI.SetActive(true);
        rankingUI.SetActive(false);
    }

    public void RankingScreen()
    {
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        dataUI.SetActive(false);
        rankingUI.SetActive(true);
    }
}