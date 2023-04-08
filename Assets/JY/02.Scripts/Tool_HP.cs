using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool_HP : MonoBehaviour
{
    public GameObject effectFac;

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Car"))
        {
            CarManager.Instance.HP++;

            Destroy(gameObject);
            GameObject effect = Instantiate(effectFac);
            effect.transform.position = transform.position;
        }
    }
}
