using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateShops : MonoBehaviour
{

    [SerializeField]
    private GameObject tienda;

    [SerializeField]
    private Transform[] posicionesIniciales;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < posicionesIniciales.Length; i++) { 
            GameObject t = Instantiate(tienda, posicionesIniciales[i].position, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
