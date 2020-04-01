using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : Enemy { 
    public float DetectionDistance = 5f;

    Transform target;
    public NavMeshAgent agent;
    public EnemyStats Stats;

    public enum EnemyState { Wander, Prepare, Lunge, Hold, GetClose, Melee, Suffer, Die}
    public EnemyState CurrentState = EnemyState.Wander;

    public float WanderCooldown;
    private float WanderTimer;
    public float WanderRadius;

    private float Distance;
    public float LungeDistance;
    public float LungeSpeed;
    public float WalkSpeed;
    public float MeleeDistance;
    public float BaseDamage;

    public Animator Animator;

    public Transform AttackTransform;
    public float AttackRadius;
    public LayerMask EnemyLayerMask;


    private bool CheckForDamage;

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
                if ((agent.destination - transform.position).sqrMagnitude <= 1)
                {
                    
                    Animator.SetBool("Walk", false);
                }


                Distance = Vector3.Distance(transform.position, target.position);
                if (Distance <= DetectionDistance)
                {
                    agent.SetDestination(target.position);
                    CurrentState = EnemyState.GetClose;
                }
                break;

            case EnemyState.Prepare:
                //Debug.Log("Prepare");
                CurrentState = EnemyState.Lunge;
                //Stats.Invunerable = true;
                FacePlayer();
                StartCoroutine(LungeAttack());
                break;
            case EnemyState.Hold:
                //Debug.Log("Hold");
                break;
            case EnemyState.GetClose:
                //Debug.Log("GetClose");
                agent.SetDestination(target.position);
                Animator.SetBool("Walk", true);
                FacePlayer();
                Distance = Vector3.Distance(transform.position, target.position);
                if (Distance <= MeleeDistance)
                {
                    Animator.SetBool("Walk", false);
                    CurrentState = EnemyState.Melee;
                }
                else if (Distance <= LungeDistance)
                {
                    Animator.SetBool("Walk", false);
                    CurrentState = EnemyState.Prepare;
                }
                break;
            case EnemyState.Melee:
                //Debug.Log("Melee");
                CurrentState = EnemyState.Hold;
                StartCoroutine(MeleeAttack());
                
                break;


                
        }

        if (CheckForDamage)
        {
            Collider[] GotPlayer = Physics.OverlapSphere(AttackTransform.position, AttackRadius, EnemyLayerMask);

            foreach (Collider c in GotPlayer)
            {
                Vector3 Direction = transform.forward.normalized;
                c.GetComponent<PlayerStats>().TakeDamage(BaseDamage, Direction);
                CheckForDamage = false;
            }
        }

    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(AttackTransform.position, AttackRadius);
    }


    IEnumerator LungeAttack()
    {

        //Change NavMeshAgent
        agent.SetDestination(target.position);
        agent.isStopped = true;
        //Do Lunge Animation with Damage Event
        Animator.SetTrigger("Lunge");
        yield return new WaitForSeconds(1f); //Animation Time
        CheckForDamage = false;
        agent.speed = WalkSpeed;
        //Change State
        Stats.Invunerable = false;
        CurrentState = EnemyState.GetClose;
    }

    IEnumerator MeleeAttack()
    {

        //Change NavMeshAgent
        agent.isStopped = true;
        Animator.SetTrigger("Melee");
        yield return new WaitForSeconds(0.5f); //Animation Time
        //MeleeAttackEvent();
        agent.isStopped = false;
        //Change State
        CurrentState = EnemyState.GetClose;
    }

    public void CheckForDamageEvent()
    {
        CheckForDamage = true;
    }


    public void StartLunge()
    {
        agent.isStopped = false;
        agent.speed = LungeSpeed;
        CheckForDamageEvent();
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
        CheckForDamage = false;
        Animator.ResetTrigger("Melee");
        Animator.ResetTrigger("Lunge");
        Animator.SetTrigger("Suffer");
        agent.speed = WalkSpeed;
        agent.SetDestination(transform.position + Direction * WalkSpeed);
        yield return new WaitForSeconds(0.25f);
        CurrentState = EnemyState.GetClose;
    }

    public override void Suffer(Vector3 Direction)
    {
        if (CurrentState.Equals(EnemyState.Die) || CurrentState.Equals(EnemyState.Prepare) || CurrentState.Equals(EnemyState.Lunge) || CurrentState.Equals(EnemyState.Suffer)) return;
        CurrentState = EnemyState.Suffer;
        StopAllCoroutines();
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
            StopAllCoroutines();
            CurrentState = EnemyState.Die;

            GameManager.Instance.MonsterKilled();

            Animator.ResetTrigger("Melee");
            Animator.ResetTrigger("Lunge");
            Animator.ResetTrigger("Suffer");
            Animator.SetTrigger("Die");
        }
    }

}
