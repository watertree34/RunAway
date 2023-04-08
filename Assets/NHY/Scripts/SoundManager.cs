using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance; // 싱글톤
    private void Awake()
    {
        Instance = this;
        audiosource = GetComponent<AudioSource>();

    }

    AudioSource audiosource;

    public AudioClip accelUpSound;

    public AudioClip accelSound;

    public AudioClip readySound;
    public AudioClip goSound;
    public AudioClip startEngineSound;

    public AudioClip buttonSound;

    public AudioClip boostSound;

    public AudioClip Siren;


    public void SoundPlay(string audioName)
    {
        if (audioName == "button")    // 가속도 높이는 소리
        {


            audiosource.PlayOneShot(buttonSound);

        }
        if (audioName == "start")    // 시작엔진 소리
        {


            audiosource.PlayOneShot(startEngineSound);

        }
        if (audioName == "ready")    // 레디 소리
        {

            if (audiosource.isPlaying) return;
            else
            {

                audiosource.PlayOneShot(readySound);

            }


        }
        if (audioName == "go")    // go 소리
        {


            audiosource.PlayOneShot(goSound);

        }
        if (audioName == "accelUp")    // 가속도 높이는 소리
        {


            audiosource.PlayOneShot(accelUpSound);

        }
        if (audioName == "accel")    // 엑셀
        {

            if (audiosource.isPlaying) return;
            else
            {

                audiosource.PlayOneShot(accelSound);

            }
        }
        if (audioName == "boost")    // 가속도 높이는 소리
        {


            audiosource.PlayOneShot(boostSound);

        }
        if (audioName == "Siren")    // 삐용삐용
       {
            if (audiosource.isPlaying) return;
            else
            {

                audiosource.PlayOneShot(Siren);

            }

        }

    }

}


//public void Awake()
//{

//    readysource.clip = ready; //오디오에 ready이라는 파일 연결
//    readysource.loop = true; //반복 여부
//    //audioSource.volume = 1.0f; //0.0f ~ 1.0f사이의 숫자로 볼륨을 조절
//    //audioSource.loop = true; //반복 여부
//    //audioSource.mute = false; //오디오 음소거

//    //audioSource.Play(); //오디오 재생
//    //audioSource.Stop(); //오디오 멈추기

//    readysource.playOnAwake = false;
//    ////활성화시 해당씬 실행시 바로 사운드 재생이 시작됩니다.
//    ////비활성화시 Play()명령을 통해서만 재생됩니다.

//    //audioSource.priority = 0;
//    ////씬안에 모든 오디오소스중 현재 오디오 소스의 우선순위를 정한다.
//    //// 0 : 최우선, 256 : 최하, 128 : 기본값
//}


