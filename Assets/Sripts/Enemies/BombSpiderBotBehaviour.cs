using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BombSpiderBotBehaviour : MonoBehaviour
{
    private GameObject player;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] private EnemyHealth eh;
    [SerializeField] private CapsuleCollider enemyColl;
    [SerializeField] private SkinnedMeshRenderer smr1;
    [SerializeField] private MeshRenderer smr2;

    [SerializeField] private ParticleSystem explosionVfx;
    [SerializeField] private GameObject explosionColl;
    [SerializeField] private GameObject explosionRange;

    public float timeUntilExplosion;

    public int bombDamage;

    private bool alreadyExploding = false;

    private GameObject currentTarget;

    [Space]
    public AudioClip tictac, boom;
    private AudioSource audioSource;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        audioSource = gameObject.GetComponent<AudioSource>();

        currentTarget = player;

        eh.onDeath += Death;
    }

    private void Update()
    {
        agent.SetDestination(currentTarget.transform.position);
        
    }

    public void SetTarget(GameObject go)
    {
        currentTarget = go;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            if (!alreadyExploding)
            {
                alreadyExploding = true;
                TriggerExplosion();
            }
            
        }
    }

    private void Death()
    {
        if (!alreadyExploding)
        {
            alreadyExploding = true;
            TriggerExplosion();
        }
    }

    private void TriggerExplosion()
    {
        agent.enabled = false;
        eh.enabled = false;
        enemyColl.enabled = false;

        explosionRange.SetActive(true);

        //Playing 'tictac'
        audioSource.clip = tictac;
        audioSource.loop = true;
        audioSource.Play();

        Invoke("Explode", timeUntilExplosion);
        enabled = false;
    }

    private void Explode()
    {
        Collider[] hitColliders = Physics.OverlapSphere(explosionColl.transform.position, explosionColl.GetComponent<SphereCollider>().radius * explosionColl.transform.localScale.x * transform.localScale.x);
        foreach(var hc in hitColliders)
        {
            if(hc.tag == "Enemy")
            {
                hc.GetComponent<EnemyHealth>().TakeDamage(bombDamage);
            }else if(hc.tag == "Player")
            {
                hc.GetComponent<Health>().TakeDamage(bombDamage);
            }
        }
        explosionRange.SetActive(false);

        //Playing Booom
        audioSource.clip = boom;
        audioSource.loop = false;
        audioSource.Play();

        explosionVfx.Play();
        smr1.enabled = false;
        smr2.enabled = false;
        Destroy(gameObject, explosionVfx.main.duration);
    }
}
