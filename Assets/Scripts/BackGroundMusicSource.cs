using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundMusicSource : MonoBehaviour
{
    public static AudioSource BackGroundMusic;
    void Start()
    {
        if (BackGroundMusic != null)
        {
            Destroy(this.gameObject);
            return;
        }
        BackGroundMusic = this.GetComponent<AudioSource>();
        GameObject.DontDestroyOnLoad(this.gameObject);
    }
}
