using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundMusic : MonoBehaviour
{
    private static IEnumerator audioQueue = null;
    private AudioSource audioSource;
    [SerializeField] private Section section;
    [SerializeField] private Section[] sections;
    private int currentSectionIndex = 0;
    private static float currentEndTime = 0;
    void Start()
    {
        audioSource = BackGroundMusicSource.BackGroundMusic;
        if (audioSource == null)
            throw new System.Exception("No AudioSource found on " + this.name);

        if (audioQueue != null)
        {
            StopCoroutine(audioQueue);
            audioQueue = null;
        }

        if (!audioSource.isPlaying)
        {
            audioSource.time = section.Start;
            audioSource.Play();
            currentEndTime = section.End;
        }
        else
        {
            Debug.Log("Audio is already playing");
            float remainingTime = GetRemainingTime();
            Debug.Log("Remaining time: " + remainingTime);
            remainingTime = remainingTime < 0 ? 0 : remainingTime;
            Debug.Log("Remaining time: " + remainingTime);
            StartCoroutine(JumpToNextSection(remainingTime, section.Start, section.End));
        }

    }

    void Update()
    {
        if (audioQueue != null || sections.Length == 0)
            return;

        float timeRemaining = GetRemainingTime();
        if (timeRemaining <= 0.8f)
        {
            currentSectionIndex = (currentSectionIndex + 1) % sections.Length;
            Section tempSection = sections[currentSectionIndex];
            audioQueue = JumpToNextSection(timeRemaining, tempSection.Start, tempSection.End);
            StartCoroutine(audioQueue);
        }
    }

    private IEnumerator JumpToNextSection(float delay, float startingTime, float endingTime)
    {
        yield return new WaitForSeconds(delay);
        audioSource.time = startingTime;
        currentEndTime = endingTime;
        audioQueue = null;
    }

    public float GetRemainingTime()
    {
        AudioClip clip = audioSource.clip;
        if (clip == null || !audioSource.isPlaying)
        {
            return 0;
        }
        return currentEndTime - audioSource.time;
    }
}
