using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerBotItemDetector : MonoBehaviour
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
        if (workerBot.itemObject == null)
        {
            if (other.CompareTag("Gear") && workerBot.currentGears < workerBot.MAXGEARSCAPACITY)
            {
                print("algo detecto gear");
                workerBot.itemObject = other.gameObject;
                workerBot.currentFSM1State = WorkerBotBehavior.FSM1_states.SEARCH;
            }

            if (other.CompareTag("Heal") && workerBot.currentHeals < workerBot.MAXHEALSCAPACITY)
            {
                workerBot.itemObject = other.gameObject;
                workerBot.currentFSM1State = WorkerBotBehavior.FSM1_states.SEARCH;
            }

            if (other.CompareTag("Ammo") && workerBot.currentAmmos < workerBot.MAXAMMOSCAPACITY)
            {
                workerBot.itemObject = other.gameObject;
                workerBot.currentFSM1State = WorkerBotBehavior.FSM1_states.SEARCH;
            }
        }
        
    }
}
