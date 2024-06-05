using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Section
{
    [SerializeField] private float start;
    [SerializeField] private float end;

    public float Start => start;
    public float End => end;
}
