using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashBoardCarTrigger : MonoBehaviour
{
    public float dashBoostPower = 500000.0f; //자동차를 밀어주는 힘-부스트
    Rigidbody rb;
    void Start()
    {

        rb = GetComponent<Rigidbody>(); //리지드바디를 받아온다.

    }

    void Update()
    {

    }
    private void OnCollisionEnter(Collision other)
    {
        //발판 트리거에 닿으면 대시하고십다~!~!
        if (other.gameObject.name.Contains("Dash"))
        {
            //0614하영추가///////
            rb.AddForce(transform.forward * dashBoostPower); //앞쪽으로 힘을 준다.   힘=질량*가속도
            print("Dash");

        }
    }
   
}
