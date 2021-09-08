using UnityEngine;

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

    public AnimationCurve adscurve = new AnimationCurve(new Keyframe(curve_xminvalue, curve_yminvalue), new Keyframe(curve_xmaxvalue, curve_ymaxvalue));

    [Header("shoot")]
    public float firerate;
    public float firerate_single;
    private float t_firerate;
    private float t_randomValue;

    [Header("shootcamera")]
    public float X;
    public float Y;
    public float Z;
    public float Zmin;
    public float Zmax;
    public float shootcamera_time;
    public GameObject cameraparent;
    public CurveMotion curvemotion;
    public CurveMotion_1 curveMotion_1;

    private Quaternion originrot;
    static private float curve_xminvalue_shoot = 1;
    static private float curve_xmaxvalue_shoot = 1;
    static private float curve_yminvalue_shoot = 1;
    static private float curve_ymaxvalue_shoot = 1;

    public AnimationCurve ShootCameraCurve = new AnimationCurve(new Keyframe(curve_xminvalue_shoot, curve_xmaxvalue_shoot), new Keyframe(curve_yminvalue_shoot, curve_ymaxvalue_shoot));

    private float shootCamera_currenttime;

    [Header("recoil")]
    public float recoil_min_ver;
    public float recoil_max_ver;
    public float recoil_max_hor;
    public float recoil_min_hor;
    public GameObject recoilcamera;
    public float RecoilCam_smooth;
    private float[] recoilload = new float[2];
    private bool isRecoil;
    private float precious_y;
    private float precious_x;
    private float y_adj = 0;
    private float x_adj = 0;
    
    public float Y_adj
    {
        get => this.y_adj;
    }
    public float X_adj
    {
        get => this.x_adj;
    }
    private float d_y;
    public float D_y
    {
        get => this.d_y;
    }
    private float d_x;
    public float D_x
    {
        get => this.d_x;
    }
    private float t_y;
    public float T_y
    {
        get => this.t_y;
    }
    private float t_x;
    public float T_x
    {
        get => this.t_x;
    }

    public GameObject weapon;
   
    
    // Start is called before the first frame updateisfovchaging
    void Start()
    {
        //默认装备武器
        //equip(0);
        _aimmode = AimMode.hip;

        //ads初始化
        iscaladstime = false;
        targetfov = adsfov;
        fov_current = camera.fieldOfView;
        isfovchaging = false;

        //射击初始化
        t_firerate = 0;
        ani_iswitchtoads = false;
        /*
        originrot = cameraparent.transform.localRotation;
        */
        isRecoil = false;
        t_y = 0;
        t_x = 0;
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

        if (isfovchaging == true)
        {
            ads_cam();
        }

        //shoot
        if (_aimmode == AimMode.ads)
        {
            Shoot();
        }

       

        if(isRecoil == false)
        {
            return;
        }
        else
        {
            recoil();
        }



    }
    void equip(int x)
    {
        if (currentweapon != null)
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
    #region weaponanimator
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
            if (_aimmode == AimMode.ads)
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
        if (adscam_time_current >= adscam_time_need)
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
        if (t_firerate < 0.5)
        {
            t_firerate += Time.deltaTime;
        }

        if (Input.GetMouseButton(0))
        {
            if (t_firerate < firerate)
            {
                return;
            }

            if (t_firerate > firerate)
            {
                /*
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
                if (t_randomValue == 3)
                {
                    weaponanimator.SetTrigger("SingleShoot3");
                }
                */
                weaponanimator.SetTrigger("SingleShoot3");
                curvemotion.isopreate = true;
                curveMotion_1.isopreate = true;
        
                isRecoil = true;
                float[] recoil_load = recoil_cal();
                d_y = recoil_load[0];
                d_x = recoil_load[1];

                t_firerate = 0;
                camera_animator.SetTrigger("ShootCamera");
                return;
            }
        }

    }
    

    float[] recoil_cal()
    {

        float t_recoil_ver = Random.Range(recoil_min_ver, recoil_max_ver);
        float t_recoil_hor = Random.Range(recoil_min_hor, recoil_max_hor);
        float[] recoilload = new float[2] { t_recoil_ver, t_recoil_hor };
        return recoilload;
    }

    void recoil()
    {  if(d_y - t_y <= 0.0001)
        {
            isRecoil = false;
            t_y = 0;
            precious_y = 0;
            t_x = 0;
            precious_x = 0;
            y_adj = 0;
            x_adj = 0;
            return;
        }     
        t_y = Mathf.Lerp(t_y,d_y, RecoilCam_smooth*Time.deltaTime);
        t_x = Mathf.Lerp(t_x, d_x, RecoilCam_smooth*Time.deltaTime);
        y_adj = t_y - precious_y;
        precious_y = t_y;
        x_adj = t_x - precious_x;
        precious_x = t_x;
        Debug.Log($"t_y = {t_y}");
        Debug.Log($"d_y = {d_y}");
        Debug.Log(y_adj);
        
    }

}
    #endregion



