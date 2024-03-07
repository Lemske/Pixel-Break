
using UnityEngine;

public class ScoreCanvasController : MonoBehaviour //TODO: This is fine for now, but it should be more dynamic
{
    private int remainingScore = 0;

    [Header("Score Display")]
    [SerializeField] private ScoreCounter scoreField; //TODO: Add animation to score
    //TODO: Add animation to score
    [Header("Score Settings")]
    [SerializeField, Min(1)] private int secondsToImplementWholeScore = 1;
    public enum ScoreType //Lets see if i ever get to making some score text that acts differently from how it gets hit
    {
        Normal,
        OneShot,
        DirectHit
    }

    void Update()
    {
        if (remainingScore == 0)
            return;
        int score = Mathf.CeilToInt(remainingScore * Time.deltaTime / secondsToImplementWholeScore);
        scoreField.AddScore(score);
        remainingScore -= score;
    }

    public void AddScore(int score, ScoreType scoreType) //TODO: Add scoreType animation loops and count when to shift to normal
    {
        Debug.Log("Score added: " + score);
        remainingScore += score;
    }
}
