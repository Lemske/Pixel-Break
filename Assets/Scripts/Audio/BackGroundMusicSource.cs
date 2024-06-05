using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundMusicSource : MonoBehaviour
{
    public static AudioSource BackGroundMusic;
    void Awake()
    {
        if (BackGroundMusic != null)
        {
            Debug.Log("Destroying duplicate BackGroundMusicSource");
            Destroy(this.gameObject);
            return;
        }
        BackGroundMusic = this.GetComponent<AudioSource>();
        GameObject.DontDestroyOnLoad(this.gameObject);
    }
}
