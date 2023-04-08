using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.SceneManagement;
using Cinemachine;

public class WholeStateMachine : MonoBehaviour
{
    //싱글톤
    public static WholeStateMachine Instance;
    private void Awake()
    {
        Instance = this;
    }
    public enum WholeState  //게임 상태
    {
        start,  // 시작 상태
        how,    //게임 방법 상태
        choice,  //선택 상태
        cine,    // 시네머신 샷 상태
        Ready,   //레디 상태
        Racing,   //레이싱 상태
        finish,   //완주 상태
        die   //죽음 상태
    }
    public WholeState nowState; //현재 게임 상태

    public GameObject choiceObject;  // 선택상태 배경 오브젝트들
    public GameObject policeCarObject; //경찰차
    public PlayableDirector playableDirector;  // 시네머신으로 만든 타임라인을 재생하기 위한 pd
    public float cineTime; // 시네머신 재생 시간

    // Start is called before the first frame update
    void Start()
    {
        nowState = WholeState.start; // 시작할땐 start 상태
        choiceObject.SetActive(false);
        policeCarObject.SetActive(false);

      
    }

    // Update is called once per frame
    public void Update()
    {
        switch (nowState)  // 전체 상태머신의 목차
        {
            case WholeState.start: // 시작 --> start UI에서 시작버튼을 누르면 Choice로 상태전환 --> ok! - 버튼들은 이 스크립트 밑에 있음
                StartState();
                break;
            case WholeState.how:
                HowState();     // 게임 방법--> How To Play UI에서 시작 버튼을 누르면 start로 상태전환-->ok!
                break;
            case WholeState.choice: // 선택  --> ChoiceUI에서 선택하면 carstatemachine의 nowcarnumber바꾸고 시작 버튼을 누르면 시네마씬으로 상태전환 -->OK!
                ChoiceState();
                break;
            case WholeState.cine: // 시네마 씬 --> 선택하고 시작버튼을 누르면 나옴, 재생 시간이 다 되거나 skip버튼을 누르면 준비로 상태전환
                CineState();
                break;
            case WholeState.Ready:  // 준비 --> CarStateMachine에서 5초가 지나면 레이싱으로 상태전환 -- OK!
                ReadyState();
                break;
            case WholeState.Racing:  // 레이싱 --> CarDrive에서 Oncollision에서 FinishLine에 닿으면 완주로 상태 전환 --OK!
                RacingState();
                break;
            case WholeState.finish:  // 완주-->Finish UI에서 재시작 버튼을 누르면 현재 씬 재로드
                FinishState();
                break;
            case WholeState.die:  // 죽음--> Finish UI에서 재시작 버튼을 누르면 현재 씬 재로드
                DieState();
                break;

        }
        print("WholeState:" + nowState);
    }

  

    public void StartState()  // 시작 타이틀 상태
    {

        CarManager.Instance.carState = CarManager.CarState.Idle;  //자동차 상태 Idle
        UIManager.Instance.currentUIState = UIManager.UIState.start;         //UI상태 start
        CameraManager.Instance.nowCamera = CameraManager.CameraState.Choice;   // 카메라 상태 choice



    }
    public void HowState()  // 게임 방법 상태
    {
        nowState = WholeState.how;
        UIManager.Instance.currentUIState = UIManager.UIState.howToPlay;         //UI상태 howToPlay

    }

    public void ChoiceState()  // 자동차 정하기 상태
    {
        CarManager.Instance.carState = CarManager.CarState.Choice;   //자동차 상태 Idle
        UIManager.Instance.currentUIState = UIManager.UIState.choice;        //UI상태 choice 
    }
    private void CineState()  // 시네마씬 상태
    {
        CarManager.Instance.carState = CarManager.CarState.Cine;   //자동차 상태 Ready
        UIManager.Instance.currentUIState = UIManager.UIState.cine;        //UI상태 cine
        CameraManager.Instance.nowCamera = CameraManager.CameraState.Cine;    // 카메라 상태 Cine
        playableDirector.Play();  // 플레이!!
        cineTime += Time.deltaTime;
        if(cineTime >= 10) // 10초되면 끝
        {
            playableDirector.Stop();
            nowState = WholeState.Ready;
        }
    }
    public void ReadyState()  // 경주 레디 상태
    {
        CarManager.Instance.carState = CarManager.CarState.Ready;   //자동차 상태 Ready
        UIManager.Instance.currentUIState = UIManager.UIState.Ready;        //UI상태 Ready
        CameraManager.Instance.nowCamera = CameraManager.CameraState.Ready;    // 카메라 상태 Ready
    }

    public void RacingState() // 레이싱 상태
    {
        CarManager.Instance.carState = CarManager.CarState.Racing;    //자동차 상태 Racing
        UIManager.Instance.currentUIState = UIManager.UIState.Racing;           //UI상태 Racing
        CameraManager.Instance.nowCamera = CameraManager.CameraState.Racing;    // 카메라 상태 Racing

    }

    public void FinishState()  //완주 상태
    {
        CarManager.Instance.carState = CarManager.CarState.Finish;    //자동차 상태 Finish
        UIManager.Instance.currentUIState = UIManager.UIState.finish;           //UI상태 Finish
        CameraManager.Instance.nowCamera = CameraManager.CameraState.Finish;    // 카메라 상태 Finish

    }
    public void DieState()  //미완주 죽음 상태
    {
        CarManager.Instance.carState = CarManager.CarState.Die;   //자동차 상태 Die
        UIManager.Instance.currentUIState = UIManager.UIState.die;          //UI상태 Die
        CameraManager.Instance.nowCamera = CameraManager.CameraState.Die;    // 카메라 상태 Finish
    }

    /////////////////////버튼 기능//////////////
    public void OnClickStart() // 타이틀의 시작으로 전환
    {
        SoundManager.Instance.SoundPlay("button");
        nowState = WholeState.start;
    }
    public void OnClickHowToPlay()  // 게임 방법으로 전환
    {
        SoundManager.Instance.SoundPlay("button");
        nowState = WholeState.how;
    }
    public void OnClickStartchoice() // 선택상태로 전환
    {
        SoundManager.Instance.SoundPlay("button");
        choiceObject.SetActive(true);
        nowState = WholeState.choice;
    }
    public void OnClickChoice0() // 0번 자동차 선택
    {
        SoundManager.Instance.SoundPlay("button");
        CarManager.Instance.CARNUMBER = 0;


    }
    public void OnClickChoice1() // 1번 자동차 선택
    {
        SoundManager.Instance.SoundPlay("button");
        CarManager.Instance.CARNUMBER = 1;

    }

    
    public void OnClickGameStart()  // 시네마씬으로 전환
    {
        SoundManager.Instance.SoundPlay("button");
        choiceObject.SetActive(false);
        policeCarObject.SetActive(true);
        nowState = WholeState.cine;

    }
    public void OnClickSkipCine()  // 시네마씬 스킵해서 게임시작
    {
        SoundManager.Instance.SoundPlay("button");
        playableDirector.Stop();  // 정지
        nowState = WholeState.Ready;
    }
    public void OnClickRestart()  // 게임 다시시작
    {
        SoundManager.Instance.SoundPlay("button");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //Time.timeScale = 1;  //게임오버때 Time.timeScale = 0으로 멈춘 시간을 다시 풀어줌
    }
    public void OnClickExit()  // 게임 종료
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
          Application.Quit();
#endif
    }

}