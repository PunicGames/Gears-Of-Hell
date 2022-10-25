using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestorUIinGame : MonoBehaviour
{

    [SerializeField]
    private GameObject mobileUI;
    private bool desktop = true;

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
    }

    void Start()
    {

        if (!desktop) {
            mobileUI.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
