using System;
using System.Collections;
using System.Runtime.InteropServices;
//using System.Collections.Generic;
//using System.Text;
using UnityEngine;
//using UnityEngine.SocialPlatforms;

//카메라를 관리하는 스크립트

//자동차의 상태에 따라 카메라를 크고 끄며 카메라 움직임을 제어하고 싶다.
//카메라는 총 레디 카메라-3개
//레이싱 중 카메라 - 2개
//Finish카메라-1개로 구성할 것이다.

public class CameraManager : MonoBehaviour
{

    //필요속성 : 타겟 자동차, 카메라들, 카메라 상태, 현재 카메라 상태
    public GameObject[] targetPosition;        // 따라다닐 타겟 오브젝트(자동차)
    public GameObject finishLine;


    public Camera ChoiceCam; // 선택 카메라

    public Camera ReadyCamera0; // 배경레디
    public Camera ReadyCamera1; // 레디상태의 카메라
    public Camera ReadyCamera2;
    public Camera ReadyCamera3;
    public float ReadyCameraSpeed = 3;

    int carNum;


    public Camera CineCam; // 시네마씬 카메라
    public Camera RacingCameraOutside; // 레이싱 상태 카메라
    public Camera MinimapCam;  //미니맵 카메라
    public GameObject[] outCamPosition;
    public GameObject[] inCamWindow;

    public float smoothing = 6; // 따라가는 부드러움
    public Camera[] RacingCameraInside;
    bool outside;
    public GameObject[] dieCamPosition;//죽음 상태 카메라 위치
    public Camera FinishLinecamera;//finish상태의 카메라

    float currentTime;  // 현재 시간

    //카메라 상태
    public enum CameraState
    {
        Choice, //선택상태
        Cine, // 시네마씬 상태
        Ready, //준비상태 
        Racing, // 레이싱상태
        Die,   ///죽음 상태
        Finish, // 완주상태
    }


    public CameraState nowCamera; // 현재 카메라 상태

    public static CameraManager Instance; //싱글톤
    private void Awake()
    {
        Instance = this;
    }


    void Start()
    {
        nowCamera = CameraState.Choice; // 처음상태 레디
                                        //카메라 상태는 일단 다 false로 해두고 스위치문을 통해 켜고 제어할 카메라를 결정한다
        carNum = CarManager.Instance.CARNUMBER;  // 따라갈 자동차를 전체 상태머신에서 결정한 자동차로 한다
        ChoiceCam.enabled = false;
        CineCam.enabled = false;
        ReadyCamera0.enabled = false;
        ReadyCamera1.enabled = false;
        ReadyCamera2.enabled = false;
        ReadyCamera3.enabled = false;
        RacingCameraOutside.enabled = false;
        RacingCameraInside[carNum].enabled = false;

        FinishLinecamera.enabled = false;
        currentTime = 0;
        outside = true;
    }

    private void FixedUpdate()   // fixedupdate에 해야 카메라와 오브젝트가 같이 움직일때 떨림현상이 없다
    {
        //1. 게임의 상태에 따라
        //->wholeStateMachine에서 제어

        //2. 카메라를 크고 끄며 카메라 움직임을 제어하고 싶다.


        switch (nowCamera)
        {
            case CameraState.Choice:
                Choice();
                break;
            case CameraState.Cine:
                Cine();
                break;
            case CameraState.Ready:
                ReadyCam();
                break;
            case CameraState.Racing:
                RacingCam();
                break;
            case CameraState.Die:
                DieCam();
                break;
            case CameraState.Finish:
                FinishCam();
                break;
        }

    }



    private void Choice()
    {
        ChoiceCam.enabled = true;
        carNum = CarManager.Instance.CARNUMBER;  // 따라갈 자동차를 전체 상태머신에서 결정한 자동차로 한다

        //마우스 휠의 입력에 따라 카메라를 확대 축소 하고싶다
        float a = Input.GetAxis("Mouse ScrollWheel");
        ChoiceCam.fieldOfView += -a * 20;
        ChoiceCam.fieldOfView = Mathf.Clamp(ChoiceCam.fieldOfView, 10, 70);

    }
    private void Cine()
    {
        ChoiceCam.enabled = false;
        CineCam.enabled = true;
    }

    ////////////레디상태/////////////
    //카메라가 1초에 한번씩 자동차의 여러방향을 움직이며 찍게하고 싶다
    private void ReadyCam()
    {

        CineCam.enabled = false;
        currentTime += Time.deltaTime;
        //배경2초
        if (currentTime <= 2)
        {
            ReadyCamera0.enabled = true;
            ReadyCamera0.transform.position += ReadyCamera0.transform.forward * 10 * Time.deltaTime;
            ReadyCamera0.transform.Rotate(2 * Time.deltaTime, 10 * Time.deltaTime, 5 * Time.deltaTime);

           
        }

        //레디1초
        else if (currentTime <= 3)
        {
            ReadyCamera0.enabled = false;
            ReadyCamera1.enabled = true;
            ReadyCamera1.transform.position += ReadyCameraSpeed * Vector3.forward * Time.deltaTime;

            SoundManager.Instance.SoundPlay("ready");  // ready 효과음 재생!!


        }
        //2초
        else if (currentTime <= 4)
        {
            ReadyCamera1.enabled = false;
            ReadyCamera2.enabled = true;
            ReadyCamera2.transform.position += ReadyCameraSpeed * Vector3.right * Time.deltaTime;

            SoundManager.Instance.SoundPlay("ready");  // ready 효과음 재생!!

        }
        //3초
        else if (currentTime <= 5)
        {
            ReadyCamera2.enabled = false;
            ReadyCamera3.enabled = true;
            ReadyCamera3.transform.position += ReadyCameraSpeed * -Vector3.forward * Time.deltaTime;
            SoundManager.Instance.SoundPlay("ready");  // ready 효과음 재생!!

        }


    }

    ////////////레이싱 상태////////////
    //outside카메라가 자동차를 따라다니고 inside카메라는 안쪽에 고정
    //스페이스바를 눌러 시점을 두개로 나누어 보고싶다

    private void RacingCam()
    {
        currentTime = 0;
        ReadyCamera3.enabled = false;

        if (Input.GetButtonDown("Jump"))
        {
            //outside = outside ? false : true;    // c#은 bool반전할때!쓰지말고 ?로 쓴다!!! 이때, bool변수=bool변수? 참일때:거짓일때로 바꿔줌
            print("change");
            if (outside == true)
            {
                outside = false;
            }
            else if (outside == false)
            {
                outside = true;
            }
        }
        if (outside)//이거는 밖에서 자동차를 보는 시점
        {
            inCamWindow[carNum].SetActive(true);
            RacingCameraOutside.enabled = true;
            RacingCameraInside[carNum].enabled = false;
            RacingCameraOutside.transform.position = Vector3.Lerp(RacingCameraOutside.transform.position, outCamPosition[carNum].transform.position, Time.deltaTime * smoothing);   // 카메라 타겟으로 러프이동
            RacingCameraOutside.transform.LookAt(targetPosition[carNum].transform); //타겟 쳐다봄


        }
        else//안에서 자동차를 보는 시점
        {
            inCamWindow[carNum].SetActive(false);
            RacingCameraInside[carNum].enabled = true;
            RacingCameraOutside.enabled = false;
        }

        //미니맵 카메라
        MinimapCam.transform.position = new Vector3(targetPosition[carNum].transform.position.x, targetPosition[carNum].transform.position.y + 22, targetPosition[carNum].transform.position.z);

    }

    ////////////Die상태///////////////
    ///레이싱 상태의 카메라가 자동차를 멀리서 관망하는 시점으로 간다
    private void DieCam()
    {
        if (outside)
        {
            RacingCameraOutside.transform.position = Vector3.Lerp(RacingCameraOutside.transform.position, dieCamPosition[carNum].transform.position, Time.deltaTime * smoothing);   // 카메라 타겟으로 러프이동
            RacingCameraOutside.transform.LookAt(targetPosition[carNum].transform); //타겟 쳐다봄
        }
        else
        {

            RacingCameraInside[carNum].transform.position = Vector3.Lerp(RacingCameraInside[carNum].transform.position, dieCamPosition[carNum].transform.position, Time.deltaTime * smoothing);   // 카메라 타겟으로 러프이동
            RacingCameraInside[carNum].transform.LookAt(targetPosition[carNum].transform); //타겟 쳐다봄
        }

    }

    ////////////Finish상태///////////////
    ///카메라의 앞방향을 바라보고싶다-트랙의 피니쉬라인보다 뒤에 카메라를 설치할것
    private void FinishCam()
    {
        RacingCameraInside[carNum].enabled = false;
        RacingCameraOutside.enabled = false;
        //1. Finish상태일때 해당 카메라를 켜고
        FinishLinecamera.enabled = true;
        //2. 카메라의 앞방향을 바라보고싶다.
        FinishLinecamera.transform.forward = CarManager.Instance.Cars[carNum].transform.forward; // 카메라의 앞방향을 나타내는 Ray를 바라봄
        FinishLinecamera.transform.LookAt(targetPosition[carNum].transform);  // 타겟을 바라보는 함수
    }
}

