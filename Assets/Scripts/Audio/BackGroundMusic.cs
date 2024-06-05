using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundMusic : MonoBehaviour
{
    private static IEnumerator audioQueue = null;
    private AudioSource audioSource;
    [SerializeField] private AudioClip startingAudio;
    [SerializeField] private AudioClip[] Loop;
    private int currentLoopIndex = 0;
    void Start()
    {
        audioSource = BackGroundMusicSource.BackGroundMusic;
        if (audioSource == null)
            return;
        audioSource.transform.position = transform.position;
        if (audioQueue != null)
            StopCoroutine(audioQueue);

        float timeRemaining = GetRemainingTime();

        if (startingAudio != null)
        {
            audioQueue = PlayFirst(timeRemaining);
            StartCoroutine(audioQueue);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (audioQueue != null && Loop.Length == 0)
            return;
        float timeRemaining = GetRemainingTime();
        if (timeRemaining <= 0.8f)
        {
            audioQueue = PlayNextLoop(timeRemaining);
            StartCoroutine(audioQueue);
        }
    }

    private IEnumerator PlayNextLoop(float delay) //Should have combined these play functions and just given them the audio to play as a parameter
    {
        yield return new WaitForSeconds(delay);
        audioSource.clip = Loop[currentLoopIndex];
        audioSource.Play();
        currentLoopIndex = (currentLoopIndex + 1) % Loop.Length;
        audioQueue = null;
    }

    private IEnumerator PlayFirst(float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.clip = startingAudio;
        audioSource.Play();
        audioQueue = null;
    }

    public float GetRemainingTime()
    {
        AudioClip clip = audioSource.clip;
        if (clip == null || !audioSource.isPlaying)
        {
            return 0;
        }

        return clip.length - audioSource.time;
    }
}
