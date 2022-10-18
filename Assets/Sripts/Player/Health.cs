using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float maxHealth = 100;
    public float currentHealth;
    private Image damageImage;
    public float flashSpeed = 5f;
    public Color damageColor = new Color(1.0f, 0.0f, 0.0f, 0.1f);

    Player playerMovement; // Referencia a dicho script para desactivarlo si el jugador muere para que no se pueda mover.
    bool isDead;
    bool damaged;

    // Display
    private Text hpDisplay;

    private void Awake()
    {
        playerMovement = GetComponent<Player>();
        currentHealth = maxHealth;
    }

    private void Start()
    {
        damageImage = GameObject.Find("DamagedBlood").GetComponent<Image>();
        hpDisplay = GameObject.Find("Vida").GetComponent<Text>();
    }


    private void FixedUpdate()
    {
        if (damaged)
        {
            damageImage.color = damageColor;
        }
        else {
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
        damaged = false;

        
            
    }

    private void Update()
    {
        if (hpDisplay != null)
        {
            hpDisplay.text = currentHealth.ToString();
        }
        //Debug.Log("Vida actual: " + currentHealth);
    }

    public void TakeDamage(float amount) {
        damaged = true;

        currentHealth -= amount;
        Debug.Log("Vida actual: " + currentHealth);

        if (currentHealth <= 0 && !isDead) {
            Death();
        }
    }

    private void Death() { 
        isDead = true;
        playerMovement.enabled = false;
        // Faltaría poner sistema de animaciones o audios, etc. Por eso está esto en un método a parte

        Destroy(gameObject, 3);
    }
}
