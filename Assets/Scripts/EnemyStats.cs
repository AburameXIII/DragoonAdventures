using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{

    public float MaxHealth = 100f;
    public float CurrentHealth;
    private Camera Cam;
    private bool AddOnlyOnce;
    public Bar HealthBar;
    private float LastAttackID;
    public Enemy Enemy;
    public GameObject HurtPrefab;
    public bool Invunerable;

    private void Start()
    {
        Cam = Camera.main;
        AddOnlyOnce = true;
    }

    public void SetEnemyHPBar(Bar HPBar)
    {
        HealthBar = HPBar;
        HealthBar.ChangeCurrentMax(CurrentHealth, MaxHealth);
    }

    private void Update()
    {
        Vector3 EnemyPosition = Cam.WorldToViewportPoint(transform.position);
        bool OnScreen = EnemyPosition.z > 0 && EnemyPosition.x > 0 && EnemyPosition.x < 1 && EnemyPosition.y > 0 && EnemyPosition.y < 1;

        if(OnScreen && AddOnlyOnce)
        {
            AddOnlyOnce = false;
            TargetManager.Instance.AddEnemy(this);
        }

        if(!OnScreen && !AddOnlyOnce)
        {
            AddOnlyOnce = true;
            TargetManager.Instance.RemoveEnemy(this);
        }


        CheckDeath();
    }

    public void TakeDamage(float Amount, float AttackID, Vector3 direction)
    {
        if (LastAttackID != AttackID && !Invunerable)
        {
            CurrentHealth -= Amount;
            Mathf.Clamp(CurrentHealth, 0, MaxHealth);
            if (HealthBar != null) HealthBar.ChangeCurrentValue(CurrentHealth);
            LastAttackID = AttackID;

            Enemy.Suffer(direction);
            Instantiate(HurtPrefab, transform);
        }
    }

    private void CheckDeath()
    {
        if (CurrentHealth <= 0)
        {
            TargetManager.Instance.RemoveEnemy(this);
            Enemy.Die();
        }
    }
}
