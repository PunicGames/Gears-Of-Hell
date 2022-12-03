using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class outerRing : MonoBehaviour
{

    [SerializeField] private ProspectorBehaviour m_Prospector;

    [SerializeField] private LayerMask m_LayerMask;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") { 
            Debug.Log("Anillo grande entra.");
            m_Prospector.insideOuterRing = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player") { 
            Debug.Log("Anillo grande sale.");
            m_Prospector.insideOuterRing = true;
            m_Prospector.fromOutside = true;
            m_Prospector.fromInside = false;
        }
    }

    public int checkNumEnemiesInRange()
    {
        // Checks for number of enemies
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity, m_LayerMask);
        return hitColliders.Length;
    }

    public float checkEnemiesHealthStatus() {
        // TODO: Quitar de la ecuación la vida propia y arreglar el número de colisionadores

        float healthRatio = 0.0f;
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity, m_LayerMask);
        // Checks for number of enemies
        foreach (Collider eC in hitColliders) { 
            
        }

        return 1.0f;
    }
}
