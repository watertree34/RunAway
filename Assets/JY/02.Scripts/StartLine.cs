using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLine : MonoBehaviour
{
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision other)
    {        
        Destroy(other.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
    }
}
