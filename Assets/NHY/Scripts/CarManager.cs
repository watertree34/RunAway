using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;


//자동차의 상태를 관리하는 상태머신
public class CarManager : MonoBehaviour
{
    // ////////////0614주연 추가 코드 - HP////////////
    public int maxHP = 3; //최대체력
    static int curHP = 3; //현재체력
    public Slider sliderHP;
    public int HP //프로퍼티
    {
        get { return curHP; }
        set
        {
            curHP = value;
            sliderHP.value = curHP;
        }
    }
    //스피드아이템 슬라이더
    public int maxBoost = 3; //최대 부스트 아이템 갯수
    static int CurBoost = 0; //현재 부스트 아이템 갯수
    public Slider sliderBoost;
    public int BOOSTITEM //프로퍼티
    {
        get { return CurBoost; }
        set
        {
            CurBoost = value;
            sliderBoost.value = CurBoost;
        }
    }
    /// //////////////////////////////


    //싱글톤으로 다른 스크립트에서도 접근가능
    public static CarManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    //자동차 상태
    public enum CarState
    {
        Idle,
        Ready, //준비상태   
        Choice, // 선택상태
        Cine,  // 시네마신 상태
        Racing, // 레이싱상태
        Finish, // 완주상태
        Die // 죽음상태
    }
    public CarState carState; // 현재상태


    //choice상태의 필요속성
    public GameObject[] Cars; // 레이싱할 자동차들
    int nowCarNumber;  // 레이싱할 자동차의 인덱스 번호

    public int CARNUMBER  // 프로퍼티로 선택한 인덱스 번호의 차 접근
    {
        get { return nowCarNumber; }
        set { nowCarNumber = value; }
    }


    // Start is called before the first frame update
    void Start()
    {
        //초기 나의 상태값 지정
        carState = CarState.Idle;
        // ////////////0614주연 추가 코드 - HP////////////
        //초기 HP
        sliderHP.maxValue = maxHP;
        HP = maxHP;            //에너미 추가하면 이걸 HP=maxHP로 바꿀것!!!!!!!

        //스피드 아이템 
        sliderBoost.maxValue = maxBoost;
        BOOSTITEM = 0;
        /// //////////////////////////////

    }

    // Update is called once per frame
    void Update()
    {

        switch (carState)  // 조건문이 아닌 스위치문을 통해 목차처럼 간편히 관리
        {
            case CarState.Idle:
                Idle();
                break;
            case CarState.Choice:
                Choice();
                break;
            case CarState.Cine:
                Cine();
                break;
            case CarState.Ready:
                Ready();
                break;

            case CarState.Racing:
                Racing();
                break;

            case CarState.Finish:
                Finish();
                break;

            case CarState.Die:
                Die();
                break;
        }
        print(carState);
    }



    private void Idle()
    {

    }


    ///선택 상태에서는 마우스의 움직임에 따라 자동차가 y축 회전하며 돌리면서 보게하고싶다
    float mx; // 마우스로 자동차 회전할때 각도
    public float rotSpeed = 50;
    public GameObject rotate; // 회전 발판
    private void Choice()
    {

        for (int i = 0; i < Cars.Length; i++)  // wholestatemachine에서 지정된 carnumber의 오브젝트만 켜고 나머지는 끔
        {
            if (i == nowCarNumber)
            {
                Cars[i].SetActive(true);
            }
            else
            {
                Cars[i].SetActive(false);
            }
        }

        //마우스 드래그의 입력에 따라 자동차를 좌우로 회전하고 싶다
        float h = Input.GetAxis("Mouse X");  //마우스를 좌 우로 스크롤
        mx += h * rotSpeed * Time.deltaTime;
        Cars[nowCarNumber].transform.eulerAngles = new Vector3(0, -mx, 0); // 자동차 회전
        rotate.transform.eulerAngles = new Vector3(0, -mx, 0); // 발판도 회전

    }
    float cy;
    private void Cine()  // 시네상태에서는 자동차의 위치가 안바뀌었으면 좋겠다-x축 -3, y축 0.7~1.2
    {

        if (nowCarNumber == 0) // 주황차일때 y축 위치
        {
            cy = 0.6f;
            if (WholeStateMachine.Instance.cineTime > 5)
            {
                cy = 1.2f;
            }
        }
        else if (nowCarNumber == 1) // 스포츠카일때 y축 위치
        {

            if (WholeStateMachine.Instance.cineTime < 5)
            {
                cy = 0.2f;
            }
            else
            {
                cy = 1;
            }
        }
        Cars[nowCarNumber].transform.position = new Vector3(-3, cy, Cars[nowCarNumber].transform.position.z);
        Cars[nowCarNumber].transform.eulerAngles = new Vector3(0, 0, 0);
    }

    //레디상태에서는 레디 카메라가 켜지고 3초동안 레디를 한 후 3초가 지나면 레이싱상태가 되고싶다
    //필요속성-현재 시간
    float currentTime;
    private void Ready()
    {
        if (nowCarNumber == 0) // 시네머신에서 바뀐 위치 맞춰줌
        { cy = 0.5f; }
        else if (nowCarNumber == 1)
        { cy = 0.1f; }
        Cars[nowCarNumber].transform.position = new Vector3(0, cy, 20);// 다시 원래대로
        Cars[nowCarNumber].transform.eulerAngles = new Vector3(0, 0, 0);

        //1. 5초동안 레디를 한 후 
        currentTime += Time.deltaTime;
       
      
        //2. 5초가 지나면 레이싱상태가 되고싶다
        if (currentTime >= 5.5f)
        {
            SoundManager.Instance.SoundPlay("go");  // go 효과음 재생!!
            SoundManager.Instance.SoundPlay("start");  // 시작엔진 효과음 재생!!
            WholeStateMachine.Instance.nowState = WholeStateMachine.WholeState.Racing;  // 전체 상태머신에서 바꾸면 자동차 상태머신도 바뀜
            currentTime = 0;
        }
    }

    //레이싱 상태에서는 자동차가 움직이고 충돌하게 하고싶다.
    private void Racing()
    {

        //Finish 상태 전환 조건-cardrive스크립트에 있음
        //Die상태 전환 조건= police car에 닿거나 hp가 0일때 enemy car에 닿으면 게임 오버


        //이상한 방향으로 가면 ui재생


    }

    private void Finish() // carDrive에서 자동차가 FinishLine에 닿으면 Finish
    {
        //다음 상태 전환 조건-ui버튼
    }

    private void Die()//carDrive에서 자동차의 라이프가 다 깎이거나 경찰차에 닿으면 Die
    {

        //다음 상태 전환 조건-ui버튼
    }
}

