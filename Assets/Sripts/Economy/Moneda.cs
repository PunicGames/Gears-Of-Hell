using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moneda : MonoBehaviour
{
    [HideInInspector]
    public int value;

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
            Destroy(gameObject);
        }
    }
}
