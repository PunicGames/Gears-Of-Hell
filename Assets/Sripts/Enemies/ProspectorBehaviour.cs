using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ProspectorBehaviour : MonoBehaviour
{
   
    Transform player;
    NavMeshAgent agent;
    Animator animator;

    private void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player").transform;
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        transform.LookAt(player.position);

        if (distance > agent.stoppingDistance)
        {
            Chase();
            //Debug.Log(GetComponent<Rigidbody>().velocity);
        }
        else
        {
            animator.SetBool("isMoving", false);

        }
    }

    private void Chase()
    {
        agent.SetDestination(player.position);
        animator.SetBool("isMoving", true);
    }

}
