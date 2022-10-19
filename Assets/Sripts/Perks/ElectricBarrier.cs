using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricBarrier : MonoBehaviour
{
    public float cooldown;

    private Health healthScript;

    private GameObject visualBarrier;

    private void Start()
    {
        healthScript = GameObject.Find("Player_Character").GetComponent<Health>();
        visualBarrier = GameObject.Find("VisualElectricBarrier");
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        ActivateBarrier();
    }

    private void OnDisable()
    {
        DeactivateBarrier();
    }

    

    public void ConsumeBarrier()
    {
        print("barrera consumida");
        DeactivateBarrier();
        InvokeRepeating("ActivateBarrier", cooldown, 0f);
    }

    private void ActivateBarrier()
    {
        if (healthScript != null)
        {
            healthScript.electricBarrier = true;
            visualBarrier.SetActive(true);
            print("barrera activada");
        } 
    }

    private void DeactivateBarrier()
    {
        if (healthScript != null)
        {
            healthScript.electricBarrier = false;
            visualBarrier.SetActive(false);
        }
    }
}
