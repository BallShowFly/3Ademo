using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curve : MonoBehaviour
{
    public AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(2, 6));
    public GameObject test;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        test.transform.position = new Vector3(test.transform.position.x,curve.Evaluate(Time.time),test.transform.position.z);
    }
}
