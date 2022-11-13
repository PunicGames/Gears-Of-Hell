using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moneda : MonoBehaviour
{
    [HideInInspector]
    public int value;

    private AudioSource coinSound;
    [SerializeField] GameObject coinMesh;

    private void Start()
    {
        coinSound = GetComponent<AudioSource>();
        coinSound.volume *= AudioManager.getGeneralVolume();
    }

    void Update()
    {
        transform.Rotate(Vector3.up * 150 * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            CoinSystem coins = other.gameObject.GetComponent<CoinSystem>();
            if (coins != null)
            {
                coins.AddCoin(value);
            }
            coinMesh.SetActive(false);
            transform.GetComponent<SphereCollider>().enabled = false;

            coinSound.Play();
            Destroy(gameObject, 1);
        }
    }
}
