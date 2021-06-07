using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsToHipAnimaitionState : MonoBehaviour
{
    public Animator weaponanimator;

    // Start is called before the first frame update


    // Update is called once per frame

    void CompleteAdsTohip()
    {
        weaponanimator.SetTrigger("IsCompleteAdstoHip");
    }
}
