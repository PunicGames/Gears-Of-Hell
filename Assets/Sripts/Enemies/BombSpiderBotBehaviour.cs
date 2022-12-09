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
    [SerializeField] private SphereCollider explosionColl;

    public float timeUntilExplosion;

    public int bombDamage;

    private bool alreadyExploding = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        eh.onDeath += Death;
    }

    private void Update()
    {
        agent.SetDestination(player.transform.position);
        
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
        //print("morido");
        agent.enabled = false;
        eh.enabled = false;
        enemyColl.enabled = false;

        Invoke("Explode", timeUntilExplosion);
        enabled = false;
    }

    private void Explode()
    {
        Collider[] hitColliders = Physics.OverlapSphere(explosionColl.center, explosionColl.radius);
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
        explosionVfx.Play();
        smr1.enabled = false;
        smr2.enabled = false;
        Destroy(gameObject, explosionVfx.main.duration);
    }
}
