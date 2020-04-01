using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed;
    public Rigidbody Rigidbody;
    public float BaseDamage;
    private bool Damaged = false;

    public float Livetime = 0;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody.velocity = transform.forward * Speed;
    }

    private void Update()
    {
        Livetime += Time.deltaTime;

        if(Livetime >= 5f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !Damaged)
        {
            other.GetComponent<PlayerStats>().TakeDamage(BaseDamage, transform.forward);
            Damaged = true;
        }
    }
}
