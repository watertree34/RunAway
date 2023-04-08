using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class PoliceCar : MonoBehaviour
{
    public GameObject[] targetPosition; //타겟 지정
    int carNum; //자동차 선택을 위한 번호
    float curTime; //현재 시간
    public float pCarSpeed = 15; //속력
    public float waitingTime = 5; //기다리는 시간
    NavMeshAgent agent; //네비게이션 사용
    public float warningRange = 3; //경고UI가 생기는 범위
    public GameObject warningImage; //경고UI이미지
    AudioSource audiosource;

    void Start()
    {
        warningImage.SetActive(false);
        agent = GetComponent<NavMeshAgent>();
        agent.speed = pCarSpeed;
        agent.enabled = false;
      
    }

    void Update()
    {
        if (WholeStateMachine.Instance.nowState == WholeStateMachine.WholeState.Ready)
        {
            transform.position = new Vector3(5, 0, 15);
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        if (WholeStateMachine.Instance.nowState == WholeStateMachine.WholeState.Racing)
        {
            carNum = CarManager.Instance.CARNUMBER;  // 따라갈 자동차 번호 전체 상태머신에서 받아오기
            curTime += Time.deltaTime;
            if (curTime > waitingTime)
            {
                if (agent.enabled == false)
                {
                    agent.enabled = true;
                }

                agent.speed += 1.5f * Time.deltaTime;

                Vector3 dir = targetPosition[carNum].transform.position - transform.position;
                agent.destination = targetPosition[carNum].transform.position;
                gameObject.transform.forward = dir; //경찰차의 앞방향이 항상 목표를 향하게
                //Vector3.Lerp(transform.position, targetPosition[carNum].transform.position, Time.deltaTime * 5);
                float distance = Vector3.Distance(targetPosition[carNum].transform.position, transform.position);

                if (distance <= warningRange) //경고 범위 안에 들어왔다면
                {
                    //사이렌 울리기
                    SoundManager.Instance.SoundPlay("Siren");
                    //경고 UI창을 활성화시켜라
                    warningImage.SetActive(true);
                }
                else if (distance > warningRange)
                {
                    //사이렌 끄기
                    //그 외의 경우엔 UI 비활성화
                    warningImage.SetActive(false);
                }
            }
        }
    }

    private void OnDrawGizmos() //범위를 시각적으로 확인하기 위한 기즈모 표시
    {
        //경고범위 원으로 표시
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, warningRange);
    }

    void OnCollisionEnter(Collision other) //충돌했을 때
    {
        if (WholeStateMachine.Instance.nowState == WholeStateMachine.WholeState.Racing)
        {
            if (other.gameObject.name.Contains("Car"))
            {
                this.gameObject.transform.position = transform.position;
                WholeStateMachine.Instance.nowState = WholeStateMachine.WholeState.die;
            }
        }
    }
}
