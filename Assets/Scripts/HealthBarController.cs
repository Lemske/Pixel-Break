using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : MonoBehaviour
{
    [SerializeField] private RectTransform healthColorBarPrefab;
    [SerializeField] private RectTransform container;
    private float initialWidth;
    private float initialX;
    private int totalHealth;

    public void InitializeHealth(int totalHealth)
    {
        this.totalHealth = totalHealth;
        initialWidth = healthColorBarPrefab.sizeDelta.x;
        initialX = healthColorBarPrefab.anchoredPosition.x;
        Debug.Log(initialWidth);
        Debug.Log(initialX);
    }

    public void UpdateHealthBar(int health)
    {
        float percentage = (float)health / totalHealth;
        float widthLeft = initialWidth * percentage;
        healthColorBarPrefab.sizeDelta = new Vector2(widthLeft, healthColorBarPrefab.sizeDelta.y);
        healthColorBarPrefab.anchoredPosition = new Vector2(initialX - (initialWidth - widthLeft) / 2, healthColorBarPrefab.anchoredPosition.y);
    }
}
