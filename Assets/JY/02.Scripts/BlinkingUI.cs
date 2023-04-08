using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkingUI : MonoBehaviour
{
    float blinkTime; //UI가 깜빡이게 하기 위한 시간
    
    void Update()
    {
        blinkTime += Time.deltaTime;

        if (blinkTime < 0.5f)
        {
            GetComponent<Image>().color = new Color(1, 1, 1, 1 - blinkTime);

        }
        else
        {
            GetComponent<Image>().color = new Color(1, 1, 1, blinkTime);

            if (blinkTime >= 1)
            {
                blinkTime = 0;
            }
        }
    }
}
