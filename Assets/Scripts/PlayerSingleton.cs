using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSingleton : MonoBehaviour
{

    private static PlayerSingleton instance = null;
    public static PlayerSingleton Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if(instance !=null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        } else
        {
            instance = this;
        }
    }
}
