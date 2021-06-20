﻿using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("equip")]
    public Transform weaponparent;
    public Weapondata[] loadout;
    private GameObject currentweapon;
    public Animator weaponanimator;
    public Motion player;
    public Animator camera_animator;

    [Header("ads")]
    public float adstime;
    private float t_adstime;
    private float fov_current;
    
    public float T_adstime
    {
        get => this.t_adstime;
    }
    private AimMode _aimmode;
    public GameObject weaponsway;
    public AimMode _Aimmode
    {
        get => this._aimmode;
    }

    public enum AimMode
    {
        hip, ads
    }

    [Header("adscamera")]
    private bool iscaladstime;
    public Camera camera;
    private float normalfov = 60;
    private float adsfov = 48;
    public float targetfov;
    public float startfov;
    private bool ani_iswitchtoads;
    private bool isfovchaging;
    private float adscam_time_current = 0;
    private float adscam_time_need;
    public float smooth;

    static private float curve_xminvalue = 1;
    static private float curve_xmaxvalue = 1;
    static private float curve_yminvalue = 1;
    static private float curve_ymaxvalue = 1;

    public AnimationCurve adscurve = new AnimationCurve(new Keyframe(curve_xminvalue,curve_yminvalue),new Keyframe(curve_xmaxvalue,curve_ymaxvalue));

    [Header("shoot")]
    public float firerate;
    public float firerate_single;
    private float t_firerate;
    private float t_randomValue;

    [Header("shootcamera")]
    public float X;
    public float Y;
    public float Z;
    public float shootcamera_time;
    public GameObject cameraparent;
    private Quaternion originrot;

    static private float curve_xminvalue_shoot = 1;
    static private float curve_xmaxvalue_shoot = 1;
    static private float curve_yminvalue_shoot = 1;
    static private float curve_ymaxvalue_shoot = 1;

    public AnimationCurve ShootCameraCurve = new AnimationCurve(new Keyframe(curve_xmaxvalue_shoot, curve_xmaxvalue_shoot), new Keyframe(curve_yminvalue_shoot, curve_ymaxvalue_shoot));

    private bool isCamera_shaking;
    private float shootCamera_currenttime;



    // Start is called before the first frame updateisfovchaging
    void Start()
    {
        //默认装备武器
        equip(0);
        _aimmode = AimMode.hip;

        //ads初始化
        iscaladstime = false;
        targetfov = adsfov;
        fov_current = camera.fieldOfView;
        isfovchaging = false;

        //射击初始化
        t_firerate = 0;
        ani_iswitchtoads = false;
        originrot = cameraparent.transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        //按“1”装备武器
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            equip(0);
        }

        setweaponanimator();

        //ads
        ads();

        if(isfovchaging == true)
        {
            ads_cam();
        }

        //shoot
        if(_aimmode == AimMode.ads)
        {
            Shoot();
        }

        if(isCamera_shaking == true)
        {
            shootcamera();
        }


    }
    void equip(int x)
    {
        if(currentweapon != null)
        {
            Object.Destroy(currentweapon);
        }
        GameObject t_euipwepon = Instantiate(loadout[x].prefab, weaponparent) as GameObject;
        t_euipwepon.name = "UMP9";
        /*
        t_euipwepon.transform.localEulerAngles = Vector3.zero;
        */
  
        currentweapon = t_euipwepon;
    }
 //武器动画相关
# region weaponanimator
    void setweaponanimator()
    {
        //调用冲刺、步行、idle动画
        weaponanimator.SetBool("IsSprinting", player.movemode == Motion.MoveMode.sprint);
        weaponanimator.SetBool("IsWalking", player.movemode == Motion.MoveMode.walk);
        weaponanimator.SetBool("IsIdling", player.movemode == Motion.MoveMode.idle);
        weaponanimator.SetBool("IsIdling_ads", player.movemode == Motion.MoveMode.adsidle);
        weaponanimator.SetBool("IsWalking_ads", player.movemode == Motion.MoveMode.adswalk);

        //调用
    }

    public void jumpanimator()
    {
        weaponanimator.SetTrigger("Isjumping");
        Debug.Log("It's time using jumping animation!");
    }
        
    public void landanimator()
    {
        weaponanimator.SetTrigger("Islanding");
        Debug.Log("its time using landing animation");
    }
    
    #endregion

    //ADS
    
    public void ads()
    {
        //按下鼠标右键后的判断
        if (Input.GetMouseButtonDown(1))
        {
            //正在从腰射转为ADS，重新开始计时
            if (t_adstime > 0)
            {
                //ads状态
                t_adstime = 0f;
                iscaladstime = true;

            }


            //目前状态为腰射，开始计时
            if (_aimmode == AimMode.hip)
            {
                iscaladstime = true;
                ani_iswitchtoads = true;
                weaponsway.transform.localEulerAngles = Vector3.zero;
                weaponanimator.SetTrigger("IsSwitchToAds");

                adscam_time_need = adstime;
                isfovchaging = true;
                targetfov = adsfov;
                startfov = fov_current;
            }
            
            //目前是ADS，转为hip
            if(_aimmode == AimMode.ads)
            {
                _aimmode = AimMode.hip;
                weaponanimator.SetTrigger("IsSwitchToHip");

                adscam_time_need = 0.12f;
                isfovchaging = true;
                targetfov = normalfov;
                startfov = adsfov;
            }
            
        }
        //ADS计时，hip转为ads
        if (iscaladstime == true)
        {
            if (t_adstime >= adstime)
            {
                _aimmode = AimMode.ads;
                iscaladstime = false;
                t_adstime = 0f;

            }
            else
            {
                t_adstime += Time.deltaTime;
            }
        }
 
    }
    
    private void ads_cam()
    {
        if (adscam_time_current >= adscam_time_need )
        {
            adscam_time_current = 0;
            isfovchaging = false;
            return;
        }
        adscam_time_current += Time.deltaTime;
        float time_ratio = curve_xmaxvalue / adscam_time_need;

        float t_camfov = startfov - adscurve.Evaluate(adscam_time_current * time_ratio) * (startfov - targetfov);
        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, t_camfov, smooth * Time.deltaTime);
               
    }


    #region shoot

    void Shoot()
    {
        float pre_randomvaule = t_randomValue;
        if(t_firerate < 0.5)
        {
            t_firerate += Time.deltaTime;
        }
  
        if (Input.GetMouseButton(0))
        {
            if(t_firerate < firerate)
            {
                return;
            }

            if (t_firerate > firerate_single)
            {
                do
                {
                    t_randomValue = Random.Range(1, 4);
                }
                while (t_randomValue == pre_randomvaule);

                if (t_randomValue == 1)
                {
                    weaponanimator.SetTrigger("SingleShoot1");

                }
                if (t_randomValue == 2)
                {
                    weaponanimator.SetTrigger("SingleShoot2");
                }
                if(t_randomValue == 3)
                {
                    weaponanimator.SetTrigger("SingleShoot3");
                }
                isCamera_shaking = true;
                t_firerate = 0;

                camera_animator.SetTrigger("ShootCamera");
                return;
            }          
        }      

    }
   
    void shootcamera()
    {
        if(shootCamera_currenttime >= shootcamera_time)
        {
            shootCamera_currenttime = 0;
            isCamera_shaking = false;
            return;
        }
        Debug.Log("operating");
        shootCamera_currenttime += Time.deltaTime;
        
     
        float time_ratio = curve_xmaxvalue_shoot / shootcamera_time;
        float t_camshake_x = ShootCameraCurve.Evaluate(shootCamera_currenttime * time_ratio) * X;
        float t_camshake_y = ShootCameraCurve.Evaluate(shootCamera_currenttime * time_ratio) * Y;
        Debug.Log(t_camshake_y);
        float t_camshake_z = ShootCameraCurve.Evaluate(shootCamera_currenttime * time_ratio) * Z;

        Quaternion t_xadj = Quaternion.AngleAxis(t_camshake_x, -Vector3.up);
        Quaternion t_yadj = Quaternion.AngleAxis(t_camshake_y, Vector3.right);
        Quaternion t_zadj = Quaternion.AngleAxis(t_camshake_z, -Vector3.forward);

        cameraparent.transform.localRotation = originrot * t_xadj * t_yadj * t_zadj;
    }
    #endregion


}