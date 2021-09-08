using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveMotion : MonoBehaviour
{
    [Header("basic")]
    public float time_total;
    protected float time_current;
    public GameObject _object;
    protected Quaternion origin_rot;
    protected Vector3 origin_pos;
    public Transform origin_tran;
    public bool isopreate;
    static protected float curve_xminvalue = 1;
    static protected float curve_xmaxvalue = 1;
    static protected float curve_yminvalue = 1;
    static protected float curve_ymaxvalue = 1;
    public AnimationCurve curve = new AnimationCurve(new Keyframe(curve_xminvalue, curve_xmaxvalue), new Keyframe(curve_yminvalue, curve_ymaxvalue));


    [Header("position")]
    public float X_position;
    public float Y_position;
    public float Z_position;

    [Header("rotation")]
    public float X_rotation;
    public float Y_rotation;
    public float Z_rotation;

 


    // Start is called before the first frame update
    void Start()
    {
        origin_rot = _object.transform.localRotation;
        origin_pos = _object.transform.localPosition;
        time_current = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isopreate == true)
        {
            opreate();
        }  
    }
 
    public void opreate()
    {
        if(time_current >= time_total)
        {
            time_current = 0;
            isopreate = false;
            return;
        }
        time_current += Time.deltaTime;
        //旋转
        float time_ratio = curve_xmaxvalue / time_total;
        float t_x = curve.Evaluate(time_current * time_ratio) * X_rotation;
        float t_y = curve.Evaluate(time_current * time_ratio) * Y_rotation;
        float t_z = curve.Evaluate(time_current * time_ratio) * Z_rotation;
        Quaternion t_xadj = Quaternion.AngleAxis(t_x, -Vector3.up);
        Quaternion t_yadj = Quaternion.AngleAxis(t_y, Vector3.right);
        Quaternion t_zadj = Quaternion.AngleAxis(t_z, -Vector3.forward);
        _object.transform.localRotation = origin_rot * t_xadj * t_yadj * t_zadj;

        //位移
        float t_x_1 = curve.Evaluate(time_current * time_ratio) * X_position;
        float t_y_1 = curve.Evaluate(time_current * time_ratio) * Y_position;
        float t_z_1 = curve.Evaluate(time_current * time_ratio) * Z_position;
  
        _object.transform.localPosition = origin_pos + new Vector3(t_x_1, t_y_1, t_z_1);


    }

}
