using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// 게임의 상태에 따라 UI가 나오게 하고싶다
public class UIManager : MonoBehaviour
{
    //싱글톤
    public static UIManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public Canvas start;     // 시작상태 UI캔버스
    public Canvas howToPlay;// 게임 방법 상태 UI캔버스
    public Canvas choice;// 자동차 선택 상태 UI캔버스
    public Canvas cine; // 시네마신 상태 UI캔버스
    public Canvas Racing;  // 레이싱 상태의 ui 캔버스
    public Text racingTime; // 레이싱 타임
    public Text FinishracingTime; // 레이싱 타임
    float racingT;
    public Canvas finish;// 완주 상태 UI캔버스
    public Canvas die;// 죽음 상태 UI캔버스

    public enum UIState
    {
        start,
        howToPlay,
        choice,
        cine,
        Ready,
        Racing,
        finish,
        die
    }
    public UIState currentUIState; //현재 UI상태


   
    void Start()
    {
        currentUIState = UIState.start;
        start.enabled = false;
        howToPlay.enabled = false;
        cine.enabled = false;
        choice.enabled = false;
        Racing.enabled = false;
        finish.enabled = false;
        die.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentUIState)  // UI상태머신의 목차
        {
            case UIState.start:
                start.enabled = true;
                howToPlay.enabled = false;
                choice.enabled = false;
                break;
            case UIState.howToPlay:
                start.enabled = false;
                howToPlay.enabled = true;
                break;

            case UIState.choice:
                start.enabled = false;
                choice.enabled = true;
                break;

            case UIState.cine:
                choice.enabled = false;
                cine.enabled = true;
                break;
            case UIState.Ready:
                cine.enabled = false;
                break;

            case UIState.Racing:
                Racing.enabled = true;
                racingT += Time.deltaTime;
                racingTime.text = "Racing Time: " + racingT.ToString("N2"); // 소숫점 둘째자리까지 레이싱타임표시
                break;

            case UIState.finish:
                Racing.enabled = false;
                finish.enabled = true;
                FinishracingTime.text =  racingTime.text; // 소숫점 둘째자리까지 레이싱타임표시

                break;
            case UIState.die:
                Racing.enabled = false;
                die.enabled = true;
                break;

        }


    }
}
