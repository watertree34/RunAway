using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyCar : MonoBehaviour
{
    public GameObject target; //목표물 설정

    float speed; //속력 지정
    public float minSpeed = 8; //최소속력
    public float maxSpeed = 16; //최대속력
    NavMeshAgent agent; //내비게이션 메쉬 에이전트

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        speed = Random.Range(minSpeed, maxSpeed);
        agent.speed = speed;
        agent.enabled = false;

    }

    void Update()
    {
        if (WholeStateMachine.Instance.nowState == WholeStateMachine.WholeState.Racing)
        {
            //에이전트 활성화시키기
            if (agent.enabled == false)
            {
                agent.enabled = true;
            }
            Vector3 dir = target.transform.position - transform.position; //방향 설정
            agent.destination = target.transform.position; //이동시키기
        }
    }

    void OnCollisionEnter(Collision other) //다른 오브젝트랑 부딪혔을 때
    {
        if (other.gameObject.name.Contains("Car"))
        {
            CarManager.Instance.HP--;
            if (CarManager.Instance.HP <= 0) //플레이어 체력이 0이라면
            {
                WholeStateMachine.Instance.nowState = WholeStateMachine.WholeState.die;
            }
        }

        else if (other.gameObject.name.Contains("Start"))
        {
            Destroy(this.gameObject);
        }
    }
}
