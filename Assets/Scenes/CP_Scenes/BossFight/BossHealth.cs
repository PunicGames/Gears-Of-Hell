using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BossHealth : MonoBehaviour
{
    [SerializeField] EnemyHealth eh;
    public float startingHealth;
    public float currentHealth;

    [SerializeField] ScheneryBehaviour sb;

    [SerializeField] private RectTransform lifeScaler;
    [SerializeField] private TextMeshProUGUI lifeText;

    Quaternion rot;

    void Start()
    {
        startingHealth = eh.startingHealth;
        currentHealth = startingHealth;
        UpdateLifeUI();

        transform.Rotate(60f, 45f, 0f);
        rot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = rot;
        if(eh.currentHealth != currentHealth)
        {
            currentHealth = eh.currentHealth;
            UpdateLifeUI();
            sb.CheckPhase((int)currentHealth);
        }
    }

    public void UpdateLifeUI()
    {
        lifeScaler.localScale = new Vector3(currentHealth / startingHealth, 0.61771f, 1);
        lifeText.text = (int)currentHealth + " / " + startingHealth;
    }
}
