using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medic : MonoBehaviour
{
    public float healRate;
    public float healPercentAmount;

    private Health healthScript;

    // PopUp
    private PopUp popup;
    [SerializeField] private Transform popupPosition;

    private void Start()
    {
        healthScript = GetComponentInParent<Health>();
        gameObject.SetActive(false);
        popup = GetComponentInParent<PopUp>();
    }

    private void OnEnable()
    {
        InvokeRepeating("MedicHeal", 0f, healRate);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void MedicHeal()
    {
        if(healthScript.currentHealth < healthScript.maxHealth)
        {
            float lifeHealed = healthScript.maxHealth / 100 * healPercentAmount;
            healthScript.Heal(lifeHealed);
        }

    }
}
