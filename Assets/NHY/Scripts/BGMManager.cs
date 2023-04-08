using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public GameObject StartBgm;
    public GameObject ChoiceBgm;
    public GameObject CineBgm;
    public GameObject RacingBgm;

    // Start is called before the first frame update
    void Start()
    {
        StartBgm.SetActive(false);
        ChoiceBgm.SetActive(false);
        CineBgm.SetActive(false);
        RacingBgm.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (WholeStateMachine.Instance.nowState == WholeStateMachine.WholeState.start)
        {
            StartBgm.SetActive(true);
        }
        if (WholeStateMachine.Instance.nowState == WholeStateMachine.WholeState.choice)
        {
            StartBgm.SetActive(false);
            ChoiceBgm.SetActive(true); 
        }
        if (WholeStateMachine.Instance.nowState == WholeStateMachine.WholeState.cine)
        { 
            ChoiceBgm.SetActive(false);
            CineBgm.SetActive(true);
        }

        if (WholeStateMachine.Instance.nowState == WholeStateMachine.WholeState.Ready)
        {
            CineBgm.SetActive(false);
            RacingBgm.SetActive(true);
        }
    }
}
