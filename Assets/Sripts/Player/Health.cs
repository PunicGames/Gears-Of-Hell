using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    //Perks barriers
    public bool electricBarrier;

    // Display health
    private Text lifeDisplay;

    private void Awake()
    {
        playerMovement = GetComponent<Player>();
        currentHealth = maxHealth;

        electricBarrier = false;
    }

    private void Start()
    {
        damageImage = GameObject.Find("DamagedBlood").GetComponent<Image>();
        lifeDisplay = GameObject.Find("LifeCounter").GetComponent<Text>();
        lifeDisplay.text = currentHealth.ToString();
    }


    private void FixedUpdate()
    {
        if (damaged)
        {
            damageImage.color = damageColor;
        }
        else
        {
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
        damaged = false;



    }

    private void Update()
    {
        //Debug.Log("Vida actual: " + currentHealth);
    }

    public void TakeDamage(float amount)
    {
        damaged = true;

        if (currentHealth > amount)
            currentHealth -= amount;
        else
            currentHealth = 0;

        lifeDisplay.text = currentHealth.ToString();
        //Debug.Log("Vida actual: " + currentHealth);

        if (currentHealth <= 0 && !isDead)
        {
            Death();
        }
    }

    private void Death()
    {
        isDead = true;
        int r = Random.Range(0, 2);
        //Randomly choose death animation type
        switch (r)
        {
            case 0:
                playerMovement.playerAnimator.SetFloat("death_type",0);
                break;
            case 1:
                playerMovement.playerAnimator.SetFloat("death_type", .5f);
                break;
            case 2:
                playerMovement.playerAnimator.SetFloat("death_type", 1);
                break;

        }
        playerMovement.playerAnimator.SetTrigger("death");
        playerMovement.enabled = false;



        // Faltaría poner sistema de animaciones o audios, etc. Por eso está esto en un método a parte

        Invoke("LoadMenu", 3);
    }

    public void LoadMenu()
    {
        Destroy(gameObject);
        SceneManager.LoadScene("Menu");
    }
}
