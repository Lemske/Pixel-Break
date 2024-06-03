using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (healthBar == null)
        {
            Debug.Log("WHYYY");
            return;
        }
        healthBar.UpdateHealthBar(health);
        if (health <= 0)
        {
            Debug.Log("Øv, du sutter");
        }
    }

    public int GetHitRadius() //TODO: might be better to set in a general movement script
    {
        return meleeHitRadius;
    }
}
