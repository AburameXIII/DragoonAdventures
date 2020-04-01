using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform Player;
    public float CameraHeight;
    public float CameraDistance;

    void Update()
    {
       transform.position = new Vector3(Player.position.x, CameraHeight, Player.position.z - CameraDistance);
    }


    
}
