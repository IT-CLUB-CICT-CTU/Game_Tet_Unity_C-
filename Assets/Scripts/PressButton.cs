using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressButton : MonoBehaviour
{
    public AudioSource audioSource;

    public void PlaySound()
    {
        audioSource.Play();
    }
}
