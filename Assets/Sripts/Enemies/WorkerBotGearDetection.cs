using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerBotGearDetection : MonoBehaviour
{
    [SerializeField] WorkerBotBehavior workerBot;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (enabled)
        {
            if (other.gameObject.CompareTag("Gear"))
            {
                //si detecta una moneda cambia el estado del workerbot a search
                if(!workerBot.isRecolecting) {
                    workerBot.isRecolecting = true;
                    workerBot.gearObjetive = other.transform;
                }
            }
        }
    }
}
