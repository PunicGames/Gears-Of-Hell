using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDamageBehaviour : MonoBehaviour
{
    List<Material> mats = new List<Material>();
    [SerializeField] float waitTime = 0.08f;


    void Start()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            mats.Add(r.material);
        }

        if (gameObject.tag == "Enemy")
            GetComponent<EnemyHealth>().takeDamage += ChangeColor;
        else
            GetComponent<Health>().takeDamage += ChangeColor;

    }
    void ChangeColor()
    {
        foreach (var m in mats)
        {
            m.color = new Color(1, 0, 0);
        }

        StartCoroutine(ResetColor());

    }
    IEnumerator ResetColor()
    {
        yield return new WaitForSeconds(waitTime);
        foreach (var m in mats)
        {
            m.color = new Color(1, 1, 1);
        }
    }

}
