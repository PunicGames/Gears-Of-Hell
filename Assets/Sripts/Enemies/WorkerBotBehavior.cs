using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class WorkerBotBehavior : MonoBehaviour
{

    [SerializeField] int attackDamage = 10;
    [SerializeField] float rollSpeed = 10.0f;
    [SerializeField] int maxCapacity = 5;
    [SerializeField] private Collider weaponCollider;

    private Animator animator;
    private GameObject player;
    NavMeshAgent agent;
    
    bool canAttack = false;

    private enum state { IDLE, PURSUE, ATTACK };
    private state currentLocomotionState = state.PURSUE; //la fsm 1
    private state currentActionState = state.IDLE; //la fsm 2

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
