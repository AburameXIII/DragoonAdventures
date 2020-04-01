using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    private static TargetManager instance;
    public static TargetManager Instance {
        get { return instance; }
    }

    public List<EnemyStats> VisableEnemies;
    private int CurrentTargetIndex;
    private EnemyStats CurrentTarget;
    public GameObject TargetUI;
    public Bar TargetUIHP;
    public GameObject PassiveTargetUI;
    public PlayerController Player;
    private bool LockedTarget;
    public bool AutoTarget;
    private Camera cam;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }

        cam = Camera.main;
    }


    private void GetNextTarget()
    {
        if (VisableEnemies.Count == 0) return;
        if(CurrentTargetIndex >= VisableEnemies.Count - 1)
        {
            CurrentTargetIndex = 0;
            CurrentTarget = VisableEnemies[0];
        } else
        {
            CurrentTargetIndex++;
            CurrentTarget = VisableEnemies[CurrentTargetIndex];
        }
    }

    private EnemyStats GetClosestTarget()
    {
        if (VisableEnemies.Count == 0) return null;
        float distance = float.MaxValue;
        EnemyStats closestTarget = null;
        foreach(EnemyStats enemy in VisableEnemies)
        {
            float targetDistance = (Player.transform.position - enemy.transform.position).magnitude;
            if(targetDistance < distance)
            {
                closestTarget = enemy;
                distance = targetDistance;
            }
        }
        return closestTarget;
    }

    public void AddEnemy(EnemyStats Enemy)
    {
        VisableEnemies.Add(Enemy);
    }

    public void RemoveEnemy(EnemyStats Enemy)
    {
        VisableEnemies.Remove(Enemy);
        if (CurrentTarget == null) return;
        if (CurrentTarget.Equals(Enemy))
        {
            CurrentTarget = null;
            TargetUI.SetActive(false);
            TargetUIHP.gameObject.SetActive(false);
            Player.Target = null;
            if (LockedTarget) LockedTarget = false;
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (LockedTarget)
            { //if player already has a target so Im going to look at the next one
                CurrentTarget.HealthBar = null;
                GetNextTarget();
                Player.Target = CurrentTarget;
            }
            if (CurrentTarget != null)
            { //Does the Current Target exist? If so then make sure UI and HP bar are correct
                PassiveTargetUI.SetActive(false);
                LockedTarget = true;
                TargetUI.SetActive(true);
                TargetUIHP.gameObject.SetActive(true);
                CurrentTarget.SetEnemyHPBar(TargetUIHP);
            }
            else
            { //It doesnt exist so disable Target UI
                LockedTarget = false;
                TargetUIHP.gameObject.SetActive(false);
                TargetUI.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            LockedTarget = false;
            TargetUIHP.gameObject.SetActive(false);
            TargetUI.SetActive(false);
        }

        if(!LockedTarget && AutoTarget)
        {
            if (CurrentTarget != null) CurrentTarget.HealthBar = null;
            CurrentTarget = GetClosestTarget();
            if (CurrentTarget != null)
            {
                Player.Target = CurrentTarget;
                PassiveTargetUI.SetActive(true);
                TargetUIHP.gameObject.SetActive(true);
                CurrentTarget.SetEnemyHPBar(TargetUIHP);
                PassiveTargetUI.transform.position = cam.WorldToScreenPoint(CurrentTarget.transform.position);
            }
            else
            {
                PassiveTargetUI.SetActive(false);
                TargetUIHP.gameObject.SetActive(false);
            }
        }

        if(CurrentTarget != null && LockedTarget)
        {
            TargetUI.transform.position = cam.WorldToScreenPoint(CurrentTarget.transform.position);
        }

        if (!AutoTarget)
        {
            PassiveTargetUI.SetActive(false);
        }
    }
}
