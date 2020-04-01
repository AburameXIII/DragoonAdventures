using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HurtScreen : MonoBehaviour
{
    public RawImage Hurt;
    public Animator HurtAnimator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetHurt()
    {
        HurtAnimator.SetTrigger("Hurt");
    }

}
