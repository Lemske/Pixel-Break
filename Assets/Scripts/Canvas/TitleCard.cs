using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TitleCard : MonoBehaviour
{
    [SerializeField] private GameObject titleCard;
    [SerializeField] private float yDeviation = 2f;
    private Vector3 initialPosition;
    private int goingUp = 1;
    private Vector3 targetPosition;
    [SerializeField] private float timer = 0.5f;
    [SerializeField] private float ySomething = 20f;
    [SerializeField] private float outlineWidth = 1f;
    private float usingTimer;

    void Start()
    {
        initialPosition = transform.position;
        targetPosition = new Vector3(initialPosition.x, initialPosition.y + yDeviation, initialPosition.z);
        usingTimer = timer;

        TextMeshProUGUI textMeshPro = titleCard.GetComponent<TextMeshProUGUI>();

        textMeshPro.enableVertexGradient = true;
        textMeshPro.enableWordWrapping = true;
        textMeshPro.outlineWidth = outlineWidth;
        textMeshPro.outlineColor = Color.green;
    }

    void Update()
    {
        if (usingTimer > 0)
        {
            usingTimer -= Time.deltaTime;
            return;
        }
        usingTimer = timer;
        float newPosY = transform.position.y + ((yDeviation / ySomething) * goingUp);
        transform.position = new Vector3(initialPosition.x, newPosY, initialPosition.z);

        if ((goingUp == 1 && transform.position.y <= targetPosition.y) || (goingUp == -1 && transform.position.y >= targetPosition.y))
            return;



        goingUp = goingUp == 1 ? -1 : 1;
        targetPosition = new Vector3(initialPosition.x, initialPosition.y + yDeviation * goingUp, initialPosition.z);
    }
}
