using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangerSounds : MonoBehaviour
{
    public AudioSource Spin;
    public AudioSource Shoot;
    public AudioSource Hurt;

    public void PlaySpinSound()
    {
        Spin.Play();
    }

    public void PlayShootSound()
    {
        Shoot.Play();
    }

    public void PlayHurtSound()
    {
        Hurt.Play();
    }
}
