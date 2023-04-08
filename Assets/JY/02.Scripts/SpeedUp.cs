using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUp : MonoBehaviour
{
    public GameObject effectFac;

    void Start()
    {

    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Car"))
        {

            //부스트업을 하고싶다~!~!
            //0614하영추가코드

            CarManager.Instance.BOOSTITEM++;
            print("boostTriger");
            CarManager.Instance.BOOSTITEM = Mathf.Clamp(CarManager.Instance.BOOSTITEM, 0, CarManager.Instance.maxBoost);// 최대치를 정하고 그 이상 속도가 안올라가게 하기

            Destroy(gameObject);
            GameObject effect = Instantiate(effectFac);
            effect.transform.position = transform.position;
        }
    }
}
