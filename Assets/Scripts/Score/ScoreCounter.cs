using TMPro;
using UnityEngine;

public class ScoreCounter : MonoBehaviour //TODO: Add scoreType animation loops
{
    [SerializeField] private TextMeshProUGUI scoreField;

    public void AddScore(int score)
    {
        scoreField.text = (int.Parse(scoreField.text) + score).ToString();
    }

}
