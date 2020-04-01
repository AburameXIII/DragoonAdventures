using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Potion : MonoBehaviour
{
    public int HealAmount;
    public int InitialPotionAmount;
    public int CurrentPotionAmount;

    public Animator PotionAnimator;
    public Text AmountText;

    // Start is called before the first frame update
    void Start()
    {
        InitialPotionAmount = CurrentPotionAmount;
        AmountText.text = CurrentPotionAmount.ToString();
    }


    public void Use()
    {
        if (CurrentPotionAmount > 0)
        {
            CurrentPotionAmount--;
            PotionAnimator.SetTrigger("Use");
            if(CurrentPotionAmount == 0)
            {
                PotionAnimator.SetBool("Empty", true);
            }
            AmountText.text = CurrentPotionAmount.ToString();
        }
    }

    public void Refresh()
    {
        CurrentPotionAmount = InitialPotionAmount;
        PotionAnimator.SetBool("Empty", false);
        AmountText.text = CurrentPotionAmount.ToString();
    }


    public bool Any()
    {
        return CurrentPotionAmount > 0;
    }
}
