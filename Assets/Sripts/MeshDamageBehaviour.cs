using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDamageBehaviour : MonoBehaviour
{
   
    [SerializeField] List<Renderer> renderers = new List<Renderer>();
    float waitTime = 0.08f;


    void Start()
    {
       
        if (gameObject.tag == "Enemy")
            GetComponent<EnemyHealth>().takeDamage += ChangeColor;
        else
            GetComponent<Health>().takeDamage += ChangeColor;

    }
    void ChangeColor()
    {
        foreach (var r in renderers)
        {
            r.material.color = new Color(1, 0, 0);
        }

        StartCoroutine(ResetColor());

    }
    IEnumerator ResetColor()
    {
        yield return new WaitForSeconds(waitTime);
        foreach (var r in renderers)
        {
            r.material.color = new Color(1, 1, 1);
        }

    }
}
