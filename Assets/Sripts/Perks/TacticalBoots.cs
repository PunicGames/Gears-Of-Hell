using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticalBoots : MonoBehaviour
{
    private float defSpeed;
    public float bufSpeed;

    private Player playerScript;

    private void Start()
    {
        playerScript = GameObject.Find("Player_Character_FINAL").GetComponent<Player>();
        defSpeed = playerScript.speed;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (playerScript != null)
        {
            playerScript.speed *= bufSpeed;
        }
    }

    private void OnDisable()
    {
        if (playerScript != null)
        {
            playerScript.speed = defSpeed;
        }
    }
}
