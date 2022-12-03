using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class innerRing : MonoBehaviour
{

    [SerializeField] private ProspectorBehaviour m_Prospector;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") { 
            Debug.Log("Anillo pequeño entra.");
            m_Prospector.insideInnerRing = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player") { 
            Debug.Log("Anillo pequeño sale.");
            m_Prospector.insideInnerRing = false;
            m_Prospector.fromOutside = false;
            m_Prospector.fromInside = true;
        }
    }
}
