using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public float maxHealth = 100;
    public float currentHealth;
    public float flashSpeed = 5f;
    public Color damageColor = new Color(1.0f, 0.0f, 0.0f, 0.1f);

    Player playerMovement; // Referencia a dicho script para desactivarlo si el jugador muere para que no se pueda mover.
    bool isDead;

    //Perks barriers
    public bool electricBarrier;

    // Display health
    private RectTransform lifeScaler;

    // PopUp
    private PopUp popup;
    [SerializeField] private Transform popupPosition;

    private void Awake()
    {
        playerMovement = GetComponent<Player>();
        popup = GetComponent<PopUp>();
        currentHealth = maxHealth;

        electricBarrier = false;
    }

    private void Start()
    {
        lifeScaler = GameObject.Find("LifeScaler").GetComponent<RectTransform>();
    }


    private void Update()
    {
        //Debug.Log("Vida actual: " + currentHealth);
    }

    public void TakeDamage(float amount)
    {
        popup.Create(popupPosition.position, (int)amount, PopUp.TypePopUp.DAMAGE, false, 0.5f);

        if (currentHealth > amount)
            currentHealth -= amount;
        else
            currentHealth = 0;

        lifeScaler.localScale = new Vector3(currentHealth / 100, 1, 1);

        if (currentHealth <= 0 && !isDead)
        {
            Death();
        }
    }

    public void Heal(float amount) {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            popup.Create(popupPosition.position, (int)amount, PopUp.TypePopUp.LIFE, true, 0.5f);
        }

        lifeScaler.localScale = new Vector3(currentHealth/100, 1, 1);
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
        SceneManager.LoadScene("Menu");
        Destroy(gameObject);
    }
}
