using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BigSpiderBotBehaviour : MonoBehaviour
{

    private GameObject player;

    [SerializeField] private NavMeshAgent agent;

    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject grenade;

    [SerializeField] private float attackRange;

    [SerializeField] private GameObject currentTarget;

    //Percepciones
    private bool alreadyAttacked = false;
    private bool inRange = false;
    private bool canAttack = false;

    //Estados
    private enum spiderState { IDLE, PURSUE, ATTACK };
    private spiderState currentState = spiderState.IDLE;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        currentTarget = player;
    }

    private void Update()
    {
        FSM();
    }

    private void FSM()
    {

        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance <= attackRange)
            inRange = true;
        else
            inRange = false;

        switch (currentState)
        {
            case spiderState.IDLE:

                IdleAction();

                if(canAttack && inRange)
                {
                    currentState = spiderState.ATTACK;
                }
                else if (!inRange)
                {
                    currentState = spiderState.PURSUE;
                }

                break;

            case spiderState.PURSUE:

                PursueAction();

                if(!canAttack && inRange)
                {
                    currentState = spiderState.IDLE;
                }
                else if (inRange && canAttack)
                {
                    currentState = spiderState.ATTACK;
                }

                break;

            case spiderState.ATTACK:

                AttackAction();

                if(alreadyAttacked && inRange)
                {
                    currentState = spiderState.IDLE;
                }
                else if (alreadyAttacked && !inRange)
                {
                    currentState = spiderState.PURSUE;
                }

                break;
        }
    }

    private void IdleAction()
    {

    }

    private void PursueAction()
    {
        agent.SetDestination(currentTarget.transform.position);
    }

    private void AttackAction()
    {

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
