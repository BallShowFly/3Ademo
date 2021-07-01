using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("腰射状态下x,y轴旋转参数")]
    public float y_sensitivity;
    public float x_sensitivity;
    public float x_Negmaxrotang;
    public float x_maxrotang;
    public float smooth;
    public float y_Negmaxrotang;
    public float y_maxrotang;

    [Header("腰射状态下z轴旋转参数")]
    public float z_xpercentage;
    public float z_ypercentage;
    public float z_maxrotang;
    public float z_Negmaxrotang;
    public Motion player;

    [Header("ADS状态下x,y轴旋转参数")]
    public float y_sensitivity_ads;
    public float x_sensitivity_ads;
    public float x_Negmaxrotang_ads;
    public float x_maxrotang_ads;
    public float y_Negmaxrotang_ads;
    public float y_maxrotang_ads;


    [Header("ADS状态下z轴旋转参数")]
    public float z_maxrotang_ads;
    public float z_Negmaxrotang_ads;


    [Header("Sway")]
    public Animator moveweaponsway;
    private float moveswayspeed;
    public GameObject weaponpoint;
    public Weapon playerweapon;

    private Quaternion originrot;

    // Start is called before the first frame update
    void Start()
    {
        originrot = weaponpoint.transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        lookweaponsway();
    }


    void lookweaponsway()
    {
        #region  weaponsway follow by look
        if (playerweapon.T_adstime > 0)
        {
            return;
        }

        float t_x_sensitivity;
        float t_y_sensitivity;

        float t_x_maxrotang;
        float t_x_Negmaxrotang;

        float t_z_Negmaxrptang;
        float t_z_maxrotang;

        float t_y_Negmaxrotang;
        float t_y_maxrotang;


        if (playerweapon._Aimmode == Weapon.AimMode.ads)
        {
            t_x_sensitivity = x_sensitivity_ads;
            t_y_sensitivity = y_sensitivity_ads;

            t_x_maxrotang = x_maxrotang_ads;
            t_x_Negmaxrotang = x_Negmaxrotang_ads;

            t_z_Negmaxrptang = z_Negmaxrotang_ads;
            t_z_maxrotang = z_maxrotang_ads;

            t_y_Negmaxrotang = y_Negmaxrotang_ads;
            t_y_maxrotang = y_maxrotang_ads;

        }
        else
        {
            t_x_sensitivity = x_sensitivity;
            t_y_sensitivity = y_sensitivity;

            t_x_maxrotang = x_maxrotang;
            t_x_Negmaxrotang = x_Negmaxrotang;

            t_z_Negmaxrptang = z_Negmaxrotang;
            t_z_maxrotang = z_maxrotang;

            t_y_maxrotang = y_maxrotang;
            t_y_Negmaxrotang = y_Negmaxrotang;
        }



        //Calculate input
        float t_xmousemove = Input.GetAxis("Mouse X") * t_x_sensitivity * Time.deltaTime ;
        t_xmousemove = Mathf.Clamp(t_xmousemove, t_x_Negmaxrotang, t_x_maxrotang);
        float t_ymousemove = Input.GetAxis("Mouse Y") * t_y_sensitivity * Time.deltaTime - playerweapon.Y_adj*400;
        t_ymousemove = Mathf.Clamp(t_ymousemove, t_y_Negmaxrotang, t_y_maxrotang);



        //Calculate RotateAngle
        Quaternion t_xadj = Quaternion.AngleAxis(t_xmousemove, -Vector3.up);
        Quaternion t_yxdj = Quaternion.AngleAxis(t_ymousemove, Vector3.right);

        float t_z = t_xmousemove * z_xpercentage + t_ymousemove * z_ypercentage + playerweapon.X_adj * 6000;
        t_z = Mathf.Clamp(t_z, t_z_Negmaxrptang, t_z_maxrotang);
        Quaternion t_zxdj = Quaternion.AngleAxis(t_z, -Vector3.forward);
        Quaternion t_delta = originrot * t_xadj * t_yxdj * t_zxdj;

        //rotate
        weaponpoint.transform.localRotation = Quaternion.Lerp(weaponpoint.transform.localRotation, t_delta, smooth * Time.deltaTime);




        #endregion



    }
    /*
    void Moveweaponway()
    {
        if (moveswayspeed > 1)
        {
        moveswayspeed = player.Pramovespeed / player.walkmovespeed / Time.deltaTime;
            moveswayspeed = 1;
        }
        moveweaponsway.speed = moveswayspeed;
    }
    */



}
