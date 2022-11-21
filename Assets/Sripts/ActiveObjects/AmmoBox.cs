using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    private bool picked = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!picked && other.gameObject.tag == "Player")
        {
            ShootSystem shootScript = other.gameObject.GetComponentInChildren<ShootSystem>();
            MeshRenderer mr = gameObject.GetComponentInChildren<MeshRenderer>();

            if (shootScript)
            {
                shootScript.addAmmoToWeapons();
                //Particle effects
                other.gameObject.GetComponent<Player>().onItemTaken.Invoke(effect.AMMO);

                mr.enabled = false;
                picked = true;
                gameObject.GetComponent<AudioSource>().Play();
                Invoke("AuxDestroy", 1f);
            }
        }
    }
    private void AuxDestroy()
    {
        Destroy(gameObject);
    }
}
