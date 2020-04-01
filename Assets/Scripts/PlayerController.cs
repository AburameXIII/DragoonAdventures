using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float ForwardSpeed = 2f;
    public float CurrentSpeed = 2f;
    
    public PlayerInput Input;
    public Animator Animator;
    public Rigidbody Rigidbody;

    public PlayerStats Stats;

    public enum PlayerState { Move, Dodge, Combo1, Combo2, Combo3, Jump, Hold, Death, Suffer }
    public PlayerState CurrentState = PlayerState.Move;

    private float DodgeCooldown = 0f;
    public float DodgeCooldownDuration = 0.5f;

    private float AttackCooldown = 0.4f;
    public float AttackCooldownDuration = 0.2f;
    public bool CanClick= true;
    public int Clicks = 0;

    private float PotionCooldown = 0f;
    public float PotionCooldownDuration = 0.2f;

    public EnemyStats Target;

    public List<Transform> AttackPoints;
    public float AttackRadius;
    public float JumpRadius;
    public LayerMask EnemyLayerMask;

    public TextUI RestUI;

    public int Combo1Damage;
    public int Combo2Damage;
    public int Combo3Damage;
    public int JumpDamage;
    public int ComboStamina;


    private bool Landed = false;
    private float CurrentAttackID;
    private Vector3 SufferDirection;
    public ParticleSystem JumpParticles;
    private bool MoveToTargetJump = false;


    public void RecoverStamina()
    {
        if (Stats.CurrentStamina < Stats.MaxStamina && !Input.IsRunning())
        {
            Stats.ChangeStamina(Time.deltaTime * 20);
        }
    }


    void Update()
    {
        switch (CurrentState)
        {
            case PlayerState.Move:
                //Debug.Log("Move");
                Move();
                UsePotion();
                CheckDodgeRoll();
                ComboCount();
                RecoverStamina();
                CheckJump();
                break;
            case PlayerState.Dodge:
                //Debug.Log("Dodge");
                DodgeRollMovement();
                break;
            case PlayerState.Combo1:
                //Debug.Log("Combo1");
                MoveCombo();
                ComboCount();
                CheckForDamage(Combo1Damage);
                break;
            case PlayerState.Combo2:
                //Debug.Log("Combo2");
                MoveCombo();
                ComboCount();
                CheckForDamage(Combo2Damage);
                break;
            case PlayerState.Combo3:
                //Debug.Log("Combo3");
                CheckForDamage(Combo3Damage);
                MoveCombo();
                break;
            case PlayerState.Hold:
                //Debug.Log("Hold");
                break;
            case PlayerState.Death:
                //Debug.Log("Death");
                break;
            case PlayerState.Suffer:
                MoveBackwards();
                break;
            case PlayerState.Jump:
                MoveJump();
                CheckForJumpDamage();
                break;
        }
        DodgeCooldown -= Time.deltaTime;
        PotionCooldown -= Time.deltaTime;
        AttackCooldown -= Time.deltaTime;

        CheckDie();
    }



    public void CheckJump()
    {
        if (!Input.InputBlocked && Input.IsAbility5() && AttackCooldown <= 0 && Stats.CurrentDragoon.Equals(Stats.MaxDragoon))
        {
            AttackCooldown = AttackCooldownDuration;
            CurrentState = PlayerState.Jump;
            StopAllCoroutines();
            Animator.SetTrigger("Jump");
            Stats.Invunerable = true;
            CurrentAttackID = Random.Range(0, 9999999999999999);
            Stats.ChangeDragoon(-Stats.MaxDragoon);
        }
    }

    public void MoveToTarget()
    {
        if (Target == null) return;
        else
        {
            MoveToTargetJump = true;
        }
    }

    public void Land()
    {
        Landed = true;
    }

    public void EndJump()
    {
        Stats.Invunerable = false;
        CurrentState = PlayerState.Move;
        Landed = false;
    }

    public void DontMoveToTarget()
    {
        MoveToTargetJump = false;
        JumpParticles.Play();
    }

    private void MoveJump()
    {
        if (Target && MoveToTargetJump)
        {
            CurrentSpeed = 100;
            Vector3 direction = (Target.transform.position - transform.position).normalized;
            if (direction.magnitude < 3f) CurrentSpeed = 70f;
            if (direction.magnitude < 2f) CurrentSpeed = 50f;
            if (direction.magnitude < 1f) CurrentSpeed = 10f;
            if (direction.magnitude < 0.8f) CurrentSpeed = 2f;
            if (direction.magnitude < 0.5f) return;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = lookRotation;

            Vector3 MoveVector = new Vector3(direction.x, 0, direction.z);
            Rigidbody.MovePosition(transform.position + MoveVector * CurrentSpeed * Time.deltaTime);
        }
        else
        {
            return;
        }
    }


    private void CheckForJumpDamage()
    {
        if (Landed)
        {
            Vector3 direction = Vector3.zero;
            if (Target != null)
            {
                direction = (Target.transform.position - transform.position).normalized;
            }
            foreach (Transform t in AttackPoints)
            {
                if (t.gameObject.activeSelf)
                {
                    Collider[] EnemyColliders = Physics.OverlapSphere(t.position, JumpRadius, EnemyLayerMask);

                    foreach (Collider c in EnemyColliders)
                    {
                        c.GetComponent<EnemyStats>().TakeDamage(JumpDamage, CurrentAttackID, direction);
                    }
                }
            }
        }
    }

    public void CheckDie()
    {
        if(Stats.CurrentHealth == 0)
        {
            CurrentState = PlayerState.Death;
            StopAllCoroutines();
            Animator.SetTrigger("Die");
            GameManager.Instance.GameOver();
            enabled = false;
        }
    }

    private void Move()
    {
        if (CurrentState.Equals(PlayerState.Combo2)) Debug.Log("HERE");
        if (!Input.InputBlocked)
        {
            //Set Running
            
            CurrentSpeed = ForwardSpeed;
            if (Input.IsRunning() && Stats.CurrentStamina > 0)
            {
                CurrentSpeed *= 2;
                Stats.ChangeStamina(-Time.deltaTime * 10);
            }
            Animator.SetBool("Run", Input.IsRunning() && Stats.CurrentStamina > 0);

            //Set Movement
            Vector3 MoveInput = Input.GetMoveInput();
            if (MoveInput.sqrMagnitude != 0)
            {
                Animator.SetBool("InputDetected", true);                
            }
            else
            {
                Animator.SetBool("InputDetected", false);
            }


            //MoveInput.Normalize();
            Vector3 MoveVector = new Vector3(MoveInput.x, 0, MoveInput.y);
            

            MoveVector *= CurrentSpeed;

            Rigidbody.MovePosition(transform.position + MoveVector * Time.deltaTime);

            if(MoveInput.sqrMagnitude != 0)
            {
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(MoveInput.x, 0, MoveInput.y));
                transform.rotation = lookRotation;
            }
            
        }

    }


    private void MoveBackwards()
    {
        
        CurrentSpeed = 1;
        
        Vector3 ForwardVector;
        if (SufferDirection != null && SufferDirection.magnitude>0) ForwardVector = SufferDirection;
        else ForwardVector = - transform.forward;


        Vector3 MoveVector = new Vector3(ForwardVector.x, 0, ForwardVector.z);
        Rigidbody.MovePosition(transform.position + MoveVector * CurrentSpeed * Time.deltaTime);

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(-ForwardVector.x, 0, -ForwardVector.z));
        transform.rotation = lookRotation;
    }

    private void DodgeRollMovement()
    {
        CurrentSpeed = 8;
        Vector3 ForwardVector = transform.forward;
        Vector3 MoveVector = new Vector3(ForwardVector.x, 0, ForwardVector.z);
        Rigidbody.MovePosition(transform.position + MoveVector * CurrentSpeed * Time.deltaTime);
    }

    private void MoveCombo()
    {
        CurrentSpeed = 1;
        Vector3 direction;
        if (Target)
        {
            direction = (Target.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = lookRotation;
        }  else
        {
            direction = transform.forward;
        }

        Vector3 MoveVector = new Vector3(direction.x, 0, direction.z);
        Rigidbody.MovePosition(transform.position + MoveVector * CurrentSpeed * Time.deltaTime);
    }

    private void CheckForDamage(int damage)
    {
        Vector3 direction = Vector3.zero;
        if (Target != null)
        {
            direction = (Target.transform.position - transform.position).normalized;
        }
        foreach(Transform t in AttackPoints)
        {
            if (t.gameObject.activeSelf) { 
                Collider[] EnemyColliders = Physics.OverlapSphere(t.position, AttackRadius, EnemyLayerMask);

                foreach (Collider c in EnemyColliders)
                {
                    c.GetComponent<EnemyStats>().TakeDamage(damage, CurrentAttackID, direction);
                    Stats.ChangeDragoonOnce(5, CurrentAttackID);
                }
            }
        }
    }


    public void ComboCheck()
    {
        CanClick = false;
        Debug.Log("CHECK COMBO, " + CurrentState + " and CLICKS " + Clicks);
        if(CurrentState.Equals(PlayerState.Combo1) && Clicks == 1)
        {
            Animator.SetInteger("Combo", 0);
            CanClick = true;
            Clicks = 0;
            CurrentState = PlayerState.Move;
        }
        else if(CurrentState.Equals(PlayerState.Combo1) && Clicks >= 2)
        {
            if (Stats.CurrentStamina >= ComboStamina)
            {

                Stats.ChangeStamina(-ComboStamina);
                CurrentState = PlayerState.Combo2;
                CurrentAttackID = Random.Range(0, 9999999999999999);
                Animator.SetInteger("Combo", 2);
                CanClick = true;
            } else
            {
                Animator.SetInteger("Combo", 0);
                CanClick = true;
                Clicks = 0;
                CurrentState = PlayerState.Move;
            }


            
        } else if(CurrentState.Equals(PlayerState.Combo2) && Clicks == 2)
        {
            Animator.SetInteger("Combo", 0);
            CanClick = true;
            Clicks = 0;
            CurrentState = PlayerState.Move;
        }
        else if (CurrentState.Equals(PlayerState.Combo2) && Clicks >= 3)
        {
            if (Stats.CurrentStamina >= ComboStamina)
            {
                Stats.ChangeStamina(-ComboStamina);
                Animator.SetInteger("Combo", 3);
                CurrentAttackID = Random.Range(0, 9999999999999999);
                CanClick = true;
                CurrentState = PlayerState.Combo3;
            } else
            {
                    Animator.SetInteger("Combo", 0);
                    CanClick = true;
                    Clicks = 0;
                    CurrentState = PlayerState.Move;
            }
        }
        else if (CurrentState.Equals(PlayerState.Combo3))
        {
            Animator.SetInteger("Combo", 0);
            CanClick = true;
            Clicks = 0;
            CurrentState = PlayerState.Move;
        } else
        {
            Animator.SetInteger("Combo", 0);
            CanClick = true;
            Clicks = 0;
            CurrentState = PlayerState.Move;
        }
    }

    private IEnumerator PerformDodgeRoll()
    {
        Animator.SetTrigger("DodgeRoll");
        Stats.Invunerable = true;
        yield return new WaitForSeconds(0.25f);
        Stats.Invunerable = false;
        CurrentState = PlayerState.Move;
    }

    private void CheckDodgeRoll()
    {
        if (DodgeCooldown <= 0)
        {
            if (!Input.InputBlocked && Input.IsAbility1() && Stats.CurrentStamina >= 10)
            {
           
                Stats.ChangeStamina(-10);
                CurrentState = PlayerState.Dodge;
                StartCoroutine(PerformDodgeRoll());
                DodgeCooldown = DodgeCooldownDuration;
            
            }
        }

    }



    private void ComboCount()
    {
        if (!Input.InputBlocked && Input.IsAbility4())
        {
            if (CanClick)
            {
                Clicks++;
            }

            if (Clicks == 1)
            {
                    if (Stats.CurrentStamina >= ComboStamina)
                    {

                        Stats.ChangeStamina(-ComboStamina);
                        CurrentState = PlayerState.Combo1;
                        Animator.SetInteger("Combo", 1);
                        CurrentAttackID = Random.Range(0, 9999999999999999);
                        return;
                    }
                Clicks = 0;
            }
        }
    }





    private void UsePotion()
    {
        if (PotionCooldown <= 0)
        {
            if (!Input.InputBlocked && Input.IsAbility2())
            {
                Stats.UsePotion();
                PotionCooldown = PotionCooldownDuration;
            }
        }
    }


    public void Suffer(Vector3 Direction)
    {
        
        if (CurrentState.Equals(PlayerState.Death) || CurrentState.Equals(PlayerState.Jump) || CurrentState.Equals(PlayerState.Suffer)) return;
        CurrentState = PlayerState.Suffer;
        SufferDirection = Direction;
        StartCoroutine(SufferAction());
        Clicks = 0;
        Stats.ChangeDragoon(1);
    }


    IEnumerator SufferAction()
    {
        
        Animator.ResetTrigger("DodgeRoll");
        Animator.SetTrigger("Suffer");
        yield return new WaitForSeconds(0.33f);
        CurrentState = PlayerState.Move;
        SufferDirection = Vector3.zero;
        
    }

}
