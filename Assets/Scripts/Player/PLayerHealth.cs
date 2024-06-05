using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PLayerHealth : MonoBehaviour //TODO: check if interface is needed
{
    [SerializeField] private int health = 100;
    [SerializeField] private int meleeHitRadius = 1;
    [SerializeField] private HealthBarController healthBar;

    private void Start()
    {
        healthBar.InitializeHealth(health);
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        healthBar.UpdateHealthBar(health);
        if (health <= 0)
        {
            Cursor.lockState = CursorLockMode.None; //TODO: should be able to move with xbox controller

            SceneManager.LoadScene("GameOver");
        }
    }

    public int GetHitRadius() //TODO: might be better to set in a general movement script
    {
        return meleeHitRadius;
    }
}
