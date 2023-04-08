using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCarManager : MonoBehaviour
{
    public Transform[] spawnPoints; //생성 장소 지정
    public GameObject enemyFactory; // 에너미공장
    float curTime; //현재시간
    float createTime; //생성시간
    public float minCreateTime = 2; //최소생성시간
    public float maxCreateTime = 7; //최대생성시간


    void Start()
    {
        createTime = Random.Range(minCreateTime, maxCreateTime);
    }


    void Update()
    {
        if (WholeStateMachine.Instance.nowState == WholeStateMachine.WholeState.Racing)
        {
            curTime += Time.deltaTime;
            if (curTime > createTime)
            {
                GameObject enemyCar = Instantiate(enemyFactory); //에너미자동차 만들어서
                int i = Random.Range(0, spawnPoints.Length); //랜덤한 i값에 따라
                enemyCar.transform.position = spawnPoints[i].transform.position; //위치시키기
                curTime = 0;
                createTime = Random.Range(minCreateTime, maxCreateTime);
            }
        }
    }
}
