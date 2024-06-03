using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreField;
    void Awake()
    {
        scoreField.text = ScoreCanvasController.totalScore.ToString();
    }
}
