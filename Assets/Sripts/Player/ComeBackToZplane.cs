using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComeBackToZplane : MonoBehaviour
{
    [SerializeField] private Transform initPos;
    private Transform playerT;

    private void Start()
    {
        playerT = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Toca plano.");
            playerT.position = initPos.position;
        }
    }
}
