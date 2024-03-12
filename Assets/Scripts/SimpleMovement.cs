using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovement : MonoBehaviour //TODO: make this more general, this is for testing
{
    private Vector3 playerLocation;
    private PLayerHealth pLayerHealth;
    [SerializeField] private float speed = 5;
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackBufferTime = 1;
    private float attackBufferTimer = 0;
    void Start()
    {
        GameObject player = GameObject.Find("Player");
        playerLocation = GameObject.Find("Player").transform.position;
        pLayerHealth = player.GetComponent<PLayerHealth>();
    }

    void Update()
    {
        if (pLayerHealth.GetHitRadius() > Vector3.Distance(playerLocation, transform.position))
        {
            attackBufferTimer -= Time.deltaTime;
            if (attackBufferTimer <= 0)
            {
                pLayerHealth.TakeDamage(damage);
                attackBufferTimer = attackBufferTime;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, playerLocation, speed * Time.deltaTime);
        }
    }
}
