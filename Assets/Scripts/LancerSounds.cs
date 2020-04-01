using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LancerSounds : MonoBehaviour
{
    public AudioSource Lunge;
    public AudioSource Melee;
    public AudioSource Hurt;

    public void PlayLungeSound()
    {
        Lunge.Play();
    }

    public void PlayMeleeSound()
    {
        Melee.Play();
    }

    public void PlayHurtSound()
    {
        Hurt.Play();
    }
}
