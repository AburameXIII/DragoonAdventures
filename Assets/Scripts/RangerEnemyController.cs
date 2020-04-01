using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangerEnemyController : Enemy { 
    public float DetectionDistance = 5f;

    Transform target;
    public NavMeshAgent agent;
    public EnemyStats Stats;

    public enum EnemyState { Wander, Shoot, Decide, Spin, Dash, Suffer, Die}
    public EnemyState CurrentState = EnemyState.Wander;

    public float WanderCooldown;
    private float WanderTimer;
    public float WanderRadius;

    private float Distance;
    public float WalkSpeed;
    public float DashSpeed;
    public float RangeDistance;
    public float MeleeDistance;
    public float BaseDamage;

    public Animator Animator;


    public LayerMask EnemyLayerMask;

    public GameObject BulletPrefab;
    public Transform FirePoint;

    public float AttackRadius;

    private bool CheckForDamage1 = false;
    private bool CheckForDamage2 = false;
    public Transform AttackTransform1;
    public Transform AttackTransform2;


    private void Start()
    {
        target = PlayerSingleton.Instance.gameObject.transform;
        agent.Warp(transform.position);
        agent.speed = WalkSpeed;
    }

    private void Update()
    {


        switch (CurrentState) {
            case EnemyState.Wander:
                //Debug.Log("Wander");
                WanderTimer += Time.deltaTime;

                if (WanderTimer >= WanderCooldown)
                {
                    Vector3 randDirection = Random.insideUnitSphere * WanderRadius;
                    randDirection += transform.position;
                    NavMeshHit navHit;
                    NavMesh.SamplePosition(randDirection, out navHit, WanderRadius, -1);
                    agent.SetDestination(navHit.position);
                    WanderTimer = 0;
                    Animator.SetBool("Walk", true);
                }
                
                if(agent.remainingDistance < 0.1f)
                {
                    Animator.SetBool("Walk", false);
                }


                Distance = Vector3.Distance(transform.position, target.position);
                if (Distance <= DetectionDistance)
                {
                    agent.SetDestination(target.position);
                    CurrentState = EnemyState.Decide;
                }
                break;

            case EnemyState.Decide:
                FacePlayer();
                
                Distance = Vector3.Distance(transform.position, target.position);
                if (Distance <= MeleeDistance)
                {
                    agent.isStopped = true;
                    Animator.SetTrigger("Spin");
                    CurrentState = EnemyState.Spin;
                }
                else
                {
                    agent.isStopped = true;
                    Animator.SetBool("Walk", false);
                    Animator.SetTrigger("Shoot");
                    CurrentState = EnemyState.Shoot;
                }
                break;
            case EnemyState.Dash:
                if(agent.remainingDistance <= 0.1f)
                {
                    CurrentState = EnemyState.Decide;
                    Animator.SetBool("Walk", false);
                }
                break;
            case EnemyState.Shoot:
                FacePlayer();
                break;

                
        }

        if (CheckForDamage1)
        {
            Collider[] GotPlayer = Physics.OverlapSphere(AttackTransform1.position, AttackRadius, EnemyLayerMask);

            foreach (Collider c in GotPlayer)
            {
                Vector3 Direction = transform.forward.normalized;
                c.GetComponent<PlayerStats>().TakeDamage(BaseDamage, Direction);
                CheckForDamage1 = false;
            }
        }

        if (CheckForDamage2)
        {
            Collider[] GotPlayer = Physics.OverlapSphere(AttackTransform2.position, AttackRadius, EnemyLayerMask);

            foreach (Collider c in GotPlayer)
            {
                Vector3 Direction = transform.forward.normalized;
                c.GetComponent<PlayerStats>().TakeDamage(BaseDamage, Direction);
                CheckForDamage2 = false;
            }
        }

    }


    public void CheckForDamage()
    {
        CheckForDamage1 = true;
        CheckForDamage2 = true;
    }

    public void GoDash()
    {
        agent.isStopped = false;
        agent.speed = DashSpeed;
        Vector3 randDirection = Random.onUnitSphere * WanderRadius;
        randDirection += transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, WanderRadius, -1);
        agent.SetDestination(navHit.position);
        Animator.SetBool("Walk", true);
        CurrentState = EnemyState.Dash;
    }

    public void GoDecide()
    {
        CurrentState = EnemyState.Decide;
    }

    public void Shoot()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        Instantiate(BulletPrefab, FirePoint.position, lookRotation);
    }

    private void FacePlayer()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = lookRotation;        
    }


    IEnumerator SufferAction(Vector3 Direction)
    {
        agent.isStopped = false;
        Animator.ResetTrigger("Spin");
        Animator.ResetTrigger("Shoot");
        Animator.SetTrigger("Suffer");
        agent.speed = WalkSpeed;
        agent.SetDestination(transform.position + Direction * WalkSpeed);
        yield return new WaitForSeconds(0.25f);
        CurrentState = EnemyState.Decide;
    }

    public override void Suffer(Vector3 Direction)
    {
        if (CurrentState.Equals(EnemyState.Die) || CurrentState.Equals(EnemyState.Spin) || CurrentState.Equals(EnemyState.Suffer)) return;
        CurrentState = EnemyState.Suffer;
        StartCoroutine(SufferAction(Direction));
    }


    public void Destroy()
    {
        Destroy(this.gameObject);
    }

    public override void Die()
    {
        if (!CurrentState.Equals(EnemyState.Die))
        {
            Animator.ResetTrigger("Suffer");
            Animator.ResetTrigger("Shoot");
            Animator.ResetTrigger("Spin");
            StopAllCoroutines();
            CurrentState = EnemyState.Die;
            GameManager.Instance.MonsterKilled();
            Animator.SetTrigger("Die");
        }
    }
}
