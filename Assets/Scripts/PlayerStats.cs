using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float MaxHealth = 100f;
    public float CurrentHealth;
    public float MaxStamina = 50f;
    public float CurrentStamina;

    public float MaxDragoon = 30f;
    public float CurrentDragoon;

    public Bar HealthBar;
    public Bar StaminaBar;
    public Bar DragoonBar;

    public bool Invunerable = false;

    public Potion Potions;
    public PlayerController Player;
    public HurtScreen Hurt;

    public GameObject PlayerHurtPrefab;
    public PlayerSounds Sounds;

    private float CurrentAttackID;


    void Start()
    {
        CurrentHealth = MaxHealth;
        CurrentStamina = MaxStamina;
        CurrentDragoon = 0;
    }

    public void ChangeDragoon(float Amount)
    {
        CurrentDragoon += Amount;
        CurrentDragoon = Mathf.Clamp(CurrentDragoon, 0, MaxDragoon);
        DragoonBar.ChangeCurrentValue(CurrentDragoon);
    }

    public void ChangeDragoonOnce(float Amount, float AttackID)
    {
        if(AttackID != CurrentAttackID)
        {

            CurrentAttackID = AttackID;
            CurrentDragoon += Amount;
            CurrentDragoon = Mathf.Clamp(CurrentDragoon, 0, MaxDragoon);
            DragoonBar.ChangeCurrentValue(CurrentDragoon);
        }
        
    }

    public void ChangeStamina(float Amount)
    {
        CurrentStamina += Amount;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0, MaxStamina);
        StaminaBar.ChangeCurrentValue(CurrentStamina);
    }

    public void TakeDamage(float Amount, Vector3 Direction)
    {
        if (!Invunerable) { 
            CurrentHealth -= Amount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
            HealthBar.ChangeCurrentValue(CurrentHealth);
            Player.Suffer(Direction);
            Hurt.GetHurt();
            Instantiate(PlayerHurtPrefab, transform);
        }
    }

    public void UsePotion()
    {
        if(CurrentHealth != MaxHealth && Potions.Any())
        {
            Sounds.PlayDrinkSound();
            Potions.Use();
            CurrentHealth += Potions.HealAmount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
            HealthBar.ChangeCurrentValue(CurrentHealth);
            
        }
    }
}
