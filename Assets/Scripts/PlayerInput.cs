using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance
    {
        get { return instance; }
    }

    private static PlayerInput instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

   

    public bool Paused;
    public bool InputBlocked;

    private Vector2 MoveInput;
    private bool Running;
    private bool Ability1;
    private bool Ability2;
    private bool Ability4;
    private bool Ability5;

    public Vector2 GetMoveInput()
    {
        if (InputBlocked) return Vector2.zero;
        return MoveInput;
    }

    

    public bool IsRunning()
    {
        if (InputBlocked) return false;
        return Running;
    }

    
    public bool IsAbility1()
    {
        if (InputBlocked) return false;
        return Ability1;
    }

    public bool IsAbility2()
    {
        if (InputBlocked) return false;
        return Ability2;
    }

    public bool IsAbility4()
    {
        if (InputBlocked) return false;
        return Ability4;
    }

    public bool IsAbility5()
    {
        if (InputBlocked) return false;
        return Ability5;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1")) Ability1 = true;
        else Ability1 = false;

        if (Input.GetButton("Fire2")) Ability2 = true;
        else Ability2 = false;
        
        if (Input.GetButton("Fire3")) Running = true;
        else Running = false;

        if (Input.GetButton("Fire4")) Ability4 = true;
        else Ability4 = false;

        if (Input.GetButton("Fire5")) Ability5 = true;
        else Ability5 = false;

        MoveInput.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    public void ReleaseControl()
    {
        InputBlocked = true;
    }

    public void GainControl()
    {
        InputBlocked = false;
    }


}
