using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    public AudioSource Combo1;
    public AudioSource Combo2;
    public AudioSource Combo3;
    public AudioSource Roll;
    public AudioSource Drink;
    public AudioSource Hurt;
    public AudioSource Jump;

    public void PlayRollSound()
    {
        Roll.Play();
    }

    public void PlayCombo1Sound()
    {
        Combo1.Play();
    }

    public void PlayCombo2Sound()
    {
        Combo2.Play();
    }

    public void PlayCombo3Sound()
    {
        Combo3.Play();
    }

    public void PlayHurtSound()
    {
        Hurt.Play();
    }

    public void PlayDrinkSound()
    {
        Drink.Play();
    }

    public void PlayJumpSound()
    {
        Jump.Play();
    }
}
