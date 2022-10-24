using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            ShootSystem shootScript = GameObject.Find("ShotOrigin").GetComponent<ShootSystem>();
            ParticleSystem ps = gameObject.GetComponentInChildren<ParticleSystem>();
            MeshRenderer mr = gameObject.GetComponentInChildren<MeshRenderer>();
            if (shootScript != null)
            {
                for (int i = 0; i < shootScript.guns.getGuns().Length; i++)
                {
                    var sg = shootScript.guns.getGuns()[i];

                    if (shootScript.availableGuns[i])
                    {

                        sg.totalBullets = sg.maxTotalBullets - sg.magazineSize;

                    }
                }

                mr.enabled = false;
                ps.Play();
                Invoke("AuxDestroy", ps.main.duration);
            }
        }
    }

    private void AuxDestroy()
    {
        Destroy(gameObject);
    }
}
