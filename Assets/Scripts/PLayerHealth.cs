using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLayerHealth : MonoBehaviour //TODO: check if interface is needed
{
    [SerializeField] private int health = 100;
    [SerializeField] private int meleeHitRadius = 1;
    //[SerializeField] private HealthBarController healthBar; //TODO: make healthbar UI Controller

    public void TakeDamage(int damage)
    {
        health -= damage;
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
