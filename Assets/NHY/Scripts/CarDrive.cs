using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]   // 리지드바디 

//자동차가 움직이게 하고싶다
//위 아래 방향키를 누르면 앞 뒤로 힘을 주고, 좌우 방향키를 누르면 바퀴를 회전한다
//shift를 누르면 드리프트를 한다

public class CarDrive : MonoBehaviour
{
    //트랙 벗어났을때 필요속성
    Vector3 nowPosition;   //트랙에 있을때 위치
    Quaternion nowRotation;
    float groundoutTime;
    float groundInTime;
    int outCount;

    //바퀴 콜라이더, 바퀴 메쉬
    public WheelCollider[] wheels = new WheelCollider[4]; //휠 콜라이더를 받아온다.
    public Transform[] tires = new Transform[4]; //바퀴가 돌아가는 걸 표현하기위한 메쉬를 받아온다.
                                                 // (주의)!!유니티에서 콜라이더를 메쉬 안에 넣지말것!!(바퀴가 날아감)


    public float maxF = 900000f; //자동차 바퀴를 돌리는 힘의 최댓값
    public float rot = 30;    // 바퀴 방향바꿀때 회전 최대각도

    //부스트에 필요한 속성
    public float boostPower = 1000000.0f; //자동차를 밀어주는 힘-부스트
    Rigidbody rb;
    public GameObject boostEffect;
    //드리프트에 필요한 속성
    WheelFrictionCurve wfc;
    public float maxBrakeF = 50;
    public float brakePower = 30;//자동차 브레이크를 땡겨주는 힘
    public float driftStiffness = 0.5f; // 드리프트할때 전체 마찰력, 뒷바퀴 마찰력을 현재에 비해 어느 비율로 줄일지
    float originStif;
    public TrailRenderer righttrail;//드리프트의 trail 이펙트
    public TrailRenderer lefttrail;
    public GameObject driftEffect;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); //리지드바디를 받아온다.


        righttrail.enabled = false;
        lefttrail.enabled = false;


        for (int i = 0; i < 4; i++)
        {
            wheels[i].steerAngle = 0;// steerAngle=조종 방향, y축을 중심으로 바퀴를 회전하게 한다-wheel 콜라이더를!(메쉬x)
        }

        wfc = new WheelFrictionCurve // 바퀴 콜라이더의 커브값 마찰력
        {
            extremumSlip = wheels[2].sidewaysFriction.extremumSlip, // 옆으로 가는 값들을 넣어준다. 이때 휠 콜라이더의 마찰력은 다 같으니까 3번째로 그냥 함
            extremumValue = wheels[2].sidewaysFriction.extremumValue,
            asymptoteSlip = wheels[2].sidewaysFriction.asymptoteSlip,
            asymptoteValue = wheels[2].sidewaysFriction.asymptoteValue,
            stiffness = wheels[2].sidewaysFriction.stiffness

        };
        originStif = wfc.stiffness;//원래 마찰값


        rb.centerOfMass = new Vector3(0, -0.5f, 0); //무게중심을 살짝 밑으로 맞춰서 안정적으로 주행하도록 한다
    }



    private void FixedUpdate()

    {
        if (CarManager.Instance.carState == CarManager.CarState.Racing) // 레이싱상태에서만 작동하게
        {

            ////1. 위 아래 방향키를 누르면 앞 뒤로 이동한다
            wheels[0].motorTorque = maxF * Input.GetAxis("Vertical") * Time.deltaTime;
            wheels[1].motorTorque = maxF * Input.GetAxis("Vertical") * Time.deltaTime;
            wheels[2].motorTorque = maxF * Input.GetAxis("Vertical") * Time.deltaTime;
            wheels[3].motorTorque = maxF * Input.GetAxis("Vertical") * Time.deltaTime;

            ////2. 좌우 방향키를 누르면 바퀴를 회전한다
            //float direction = rot * Input.GetAxis("Horizontal");
            //carSteer(direction);
            wheels[0].steerAngle = rot * Input.GetAxis("Horizontal");
            wheels[1].steerAngle = rot * Input.GetAxis("Horizontal");
        }
    }
    private void Update()

    {
        if (CarManager.Instance.carState == CarManager.CarState.Racing) // 레이싱상태에서만 작동하게
        {
            float a = Input.GetAxis("Vertical"); // 위아래 방향키에 따라 가속도를 받을것
            if (a != 0)
            {
                SoundManager.Instance.SoundPlay("accel");

            }
            //3.shift키를 누르면 드리프트한다    // 브레이크를 시뮬레이션 하기위해서는 모터 토크에 음수를 사용하지 않고, brakeTorque를 대신 사용
            //    //Wheel Collider값에서 마찰력
            //    //Forward Friction , SideWays Friction 값 조절을 통해 마찰력 조절  //stiffness를 통해 드리프트 할것!!!
            //    //https://micropilot.tistory.com/2663 참고할것
            float drift = Input.GetAxis("Fire3");
            CarDrift(drift);

            //4. ctrl키를 누르면 부스트를 쓴다

            if (CarManager.Instance.BOOSTITEM > 0) // 부스트 아이템이 있을때만 쓴다
            {
                if (Input.GetButtonDown("Fire1"))   // ctrl키나 마우스 왼쪽을 누르면 앞으로 힘을 준다-부스트
                {
                    CarManager.Instance.BOOSTITEM -= 1;
                    StartCoroutine(Boost());
                    SoundManager.Instance.SoundPlay("boost");
                }
            }

            //타이어(바퀴의 메쉬)를 돌리는 역할-콜라이더는 돌아가지 않기 때문에
            for (int i = 0; i < 4; i++)
            {

                Quaternion quat;
                Vector3 pos;
                wheels[i].GetWorldPose(out pos, out quat);
                tires[i].position = pos;
                tires[i].rotation = quat;

            }
            //맵을 벗어나거나 자동차가 오브젝트에 끼이면 가까운 위치에서 다시 시작하고 싶다
            Ray downray = new Ray(transform.position, -transform.up); //1. 자동차의 밑 방향 레이를 만든다
            Debug.DrawRay(downray.origin, downray.direction * 10f, Color.red, 1f);
            RaycastHit hitInfo = new RaycastHit(); // 던지는 시선의 정보를 hitInfo에 저장할거다
            int layerMask = 1 << LayerMask.NameToLayer("Track");  // 트랙 레이어만 충돌 체크함
                                                                  //layerMask = ~layerMask; //트랙이 아닌 레이어로 하려면 이렇게
            if (Physics.Raycast(downray, out hitInfo, 3, layerMask) == false) //2. 시선을 던지고 3m안에 트랙이 안 닿으면
            {
                groundoutTime += Time.deltaTime;
                print("악!!!!");
                //3. 트랙에 닿지 않았다면 8초가 지나고 가까운 위치로
                if (groundoutTime >= 8)
                {

                    transform.position = nowPosition;  // 밑에 트랙에 닿았을때 정보로 다시 돌아감

                    transform.rotation = nowRotation;
                    transform.up = Vector3.up;
                    print("트랙다시돌아옴!!!!!");

                    groundoutTime = 0;
                    outCount++;
                }

                if (outCount > 3)
                {
                    WholeStateMachine.Instance.nowState = WholeStateMachine.WholeState.die;
                }
            }



            Ray carForwardRay = new Ray(transform.position, this.transform.forward); // 앞방향 레이 
        }
    }




    #region//////////////////////자동차 이동/////////////////////
    //private void carMove(float a)  // 자동차 이동
    //{
    //    if (a > 0)  // 위 방향키를 누르면 
    //    {
    //        for (int i = 0; i < 4; i++)
    //        {
    //            wheels[i].motorTorque = maxF * a; //바퀴를 돌린다.   
    //                                              //motorTorque-휠 축에 대한 모터 토크를 뉴턴 미터로 나타냅니다. 방향의 따라서 양수 또는 음수의 값을 설정할 수 있습니다.   
    //                                              //정격 전압과 정격 주파수에서, 속도의 돌연한 감소 없이 전동기가 발생할 수 있는 정상 상태에서의 비동기 토크의 최댓값. 
    //                                              //속도가 증가함에 따라 토크가 연속적으로 감소하는 비동기 전동기에는 적용하지 않는다.
    //            wheels[i].brakeTorque = 0;        //바퀴가 돌아갈때는 브레이크값 안넣음
    //        }

    //        SoundManager.Instance.SoundPlay("accel");
    //    }
    //    else if (a == 0)   // 아무것도 안누르면
    //    {
    //        for (int i = 0; i < 4; i++)
    //        {
    //            wheels[i].motorTorque = 0;   // 바퀴를 안돌린다

    //        }
    //    }
    //    else if (a < 0)// 아래 방향키를 누르면 
    //    {

    //        for (int i = 0; i < 4; i++)
    //        {
    //            wheels[i].motorTorque = maxF * a; //바퀴를 돌린다.   

    //            wheels[i].brakeTorque = 0;        //바퀴가 돌아갈때는 브레이크값 안넣음
    //        }

    //        SoundManager.Instance.SoundPlay("accel");
    //    }



    //}
    #endregion

    //////// 자동차 좌우 조종///////
    private void carSteer(float direction)
    {

        for (int i = 0; i < 2; i++) // 앞바퀴만 회전한다.
        {

            wheels[i].steerAngle = direction;

        }

    }

    /////드리프트////////////////
    private void CarDrift(float drift)
    {


        if (drift > 0)  // 3-1. shift를 누르면
        {
            // 3-2. 드리프트 한다= 뒷바퀴가 미끄러진다->앞바퀴의 조종이 빠르게 적용됨

            //3-2-1. 뒷바퀴가 미끄러지게 하고싶다 = 뒷바퀴의 마찰값만 줄여준다

            wfc.stiffness = driftStiffness * originStif;
            wheels[2].sidewaysFriction = wfc; // 휠 콜라이더의 2,3인덱스가 뒷바퀴이므로 뒷바퀴의 stiffness값만 변경
            wheels[3].sidewaysFriction = wfc;


            ////3-2-2. 속도를 줄어들게 하고싶다
            //for (int i = 2; i < 4; i++)
            //{
            //    wheels[i].motorTorque = 0;   // 바퀴를 안돌리고

            //    wheels[i].brakeTorque = maxBrakeF * brakePower;  //브레이크 파워를 준다

            //}
            ////드리프트 그리기!!

            righttrail.enabled = true; // trail을 이용해 그림
            lefttrail.enabled = true;
            GameObject drifteff = Instantiate(driftEffect);
            drifteff.transform.position = transform.position;
            Destroy(drifteff, 0.5f);

        }
        else// 아니면 정상작동(stiffness=1)
        {

            wfc.stiffness = originStif;
            wheels[2].sidewaysFriction = wfc; // 휠 콜라이더의 2,3인덱스가 뒷바퀴이므로 뒷바퀴의 stiffness값만 정상으로 변경
            wheels[3].sidewaysFriction = wfc;


            righttrail.enabled = false;
            lefttrail.enabled = false;
        }
    }


    ///////////// 부스트 //////////////////--대시발판 스크립트에도 있다
    float boostEffTime = 0.3f;
    public IEnumerator Boost()
    {
        rb.AddForce(transform.forward * boostPower); //앞쪽으로 힘을 준다.   힘=질량*가속도

        print("boost");
        while (boostEffTime > 0)  // 부스트 이펙트
        {
            boostEffTime -= Time.deltaTime;
            yield return new WaitForSeconds(0.1f);
            GameObject boostEff = Instantiate(boostEffect);  // 부스트 이펙트 생성
            boostEff.transform.position = transform.position;
            Destroy(boostEff, 0.5f);
            yield return null;
        }
        boostEffTime = 0.3f;

    }

    //////////////피니쉬라인 충돌////////////////////

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Finish")) // 만약 자동차가 finishline에 닿으면
        {
            WholeStateMachine.Instance.nowState = WholeStateMachine.WholeState.finish; // 상태전환
        }
    }



    ////////////트랙 감지////////////////////
    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.tag == "track") // 만약 트랙에 닿으면 가까운 위치 저장
        {
            groundInTime += Time.deltaTime;
            if (groundInTime >= 0.3f)
            {

                nowPosition = transform.position;//가까운 위치 저장

                nowRotation = transform.rotation;//가까운 로테이션도 저장

                print("save!!!!");
                groundInTime = 0; // 0.3초마다 갱신
            }


            outCount = 0;
            if (Input.GetButtonDown("Vertical")) // 가속도 높이기 효과음
            {
                SoundManager.Instance.SoundPlay("accelUp");
            }

        }

    }

    private void OnDrawGizmos() // 앞방향 레이의 기즈모
    {
        Debug.DrawRay(transform.position, transform.forward * 10, Color.red);


    }



}