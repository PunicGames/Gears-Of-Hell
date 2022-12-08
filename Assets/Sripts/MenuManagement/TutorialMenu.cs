using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMenu : MonoBehaviour
{
    private bool desktop = true;
    [SerializeField] private GameObject PcGameplay;
    [SerializeField] private GameObject MobileGameplay;

    [SerializeField] private GameObject PcControls;
    [SerializeField] private GameObject MobileControls;

    private void Awake()
    {
        if (Application.isMobilePlatform)
        {
            desktop = false;

        }
        else if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            desktop = true;

        }

        //desktop = false;
    }

    public void ActivateControlsScreen()
    {
        if (desktop)
        {
            PcControls.SetActive(true);
        }
        else
        {
            MobileControls.SetActive(true);
        }
    }

    public void DeactivateControlsScreen()
    {
        if (desktop)
        {
            PcControls.SetActive(false);
        }
        else
        {
            MobileControls.SetActive(false);
        }
    }

    public void ActivateGameplayScreen()
    {
        if (desktop)
        {
            PcGameplay.SetActive(true);
        }
        else
        {
            MobileGameplay.SetActive(true);
        }
    }

    public void DeactivateGameplayScreen()
    {
        if (desktop)
        {
            PcGameplay.SetActive(false);
        }
        else
        {
            MobileGameplay.SetActive(false);
        }
    }
}
