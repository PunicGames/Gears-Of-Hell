using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FireSpiderBehavior : MonoBehaviour
{

    // Components
    [HideInInspector] private Transform playerPos;
    [HideInInspector] private NavMeshAgent agent;
    [HideInInspector] private EnemyHealth eH;
    [HideInInspector] private CapsuleCollider collider;
    [HideInInspector] private SkinnedMeshRenderer skinRender;
    [SerializeField] private MeshRenderer bombMesh;
    [SerializeField] private ParticleSystem explosionVfx;
    [SerializeField] private GameObject explosionCollider;
    [SerializeField] private GameObject explosionRange;

    // Explosion variables
    [SerializeField] private float timeUntilExplosion;
    [SerializeField] private int bombDamage;
    private bool alreadyExploding = false;
    private bool exploded = false;

    // Sounds
    [Space]
    [SerializeField] private AudioClip tictac, boom;
    private AudioSource audioSource;

    // Damage Targets
    [Space][SerializeField] private LayerMask m_LayerMask;

    // FSM States
    private enum State { CHASING, EXPLODING, DEAD}
    private State currentState;

    // Fire
    [SerializeField] private GameObject fireObj;

    private void Start()
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        audioSource = gameObject.GetComponent<AudioSource>();
        eH = gameObject.GetComponent<EnemyHealth>();
        collider = gameObject.GetComponent<CapsuleCollider>();
        skinRender = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();

        eH.onDeath += Death;

        currentState = State.CHASING;
    }

    private void FixedUpdate()
    {
        FSM();
    }


    private void FSM() {
        switch (currentState) {
            case State.CHASING:
                Chase(playerPos.position);
                break;
            case State.EXPLODING:
                if (!alreadyExploding) { 
                    alreadyExploding = true;

                    Invoke(nameof(Death), timeUntilExplosion * 2.5f);

                    explosionRange.SetActive(true);

                    //Play sounds
                    audioSource.clip = tictac;
                    audioSource.loop = true;
                    audioSource.Play();
                }

                agent.speed *= 1.005f;
                Chase(playerPos.position);
                break;
            case State.DEAD:
                if (!exploded) {
                    exploded = true;
                    DeactivateComponents();
                    Explode();
                }
                break;
            default:
                break;
        }
    }

    private void Chase(Vector3 position) { 
        agent.SetDestination(position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            currentState = State.EXPLODING;
        }
    }

    private void Death()
    {
        currentState = State.DEAD;
    }

    private void DeactivateComponents() {
        agent.Stop();
        //agent.enabled = false;
        //eH.enabled = false;
        skinRender.enabled = false;
        bombMesh.enabled = false;
    }
    private void Explode()
    {
        GetComponent<CapsuleCollider>().enabled = false;
        Collider[] hitColliders = Physics.OverlapSphere(explosionCollider.transform.position, explosionCollider.GetComponent<SphereCollider>().radius * explosionCollider.transform.localScale.x * transform.localScale.x, m_LayerMask, QueryTriggerInteraction.Ignore);
        foreach (var hc in hitColliders)
        {
            if (hc.tag == "Enemy")
            {
                hc.GetComponent<EnemyHealth>().TakeDamage(bombDamage);
            }
            else if (hc.tag == "Player")
            {
                hc.GetComponent<Health>().TakeDamage(bombDamage);
            }
        }
        explosionRange.SetActive(false);

        //Playing Boom sound
        audioSource.clip = boom;
        audioSource.loop = false;
        audioSource.Play();

        // Play particle system
        explosionVfx.Play();

        Instantiate(fireObj, transform.position, Quaternion.identity);
        Destroy(gameObject, explosionVfx.main.duration);
    }
}
