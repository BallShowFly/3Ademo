using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("equip")]
    public Transform weaponparent;
    public Weapondata[] loadout;
    private GameObject currentweapon;
    public Animator weaponanimator;
    public Motion player;

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
    private bool iscamerachanging;
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





    // Start is called before the first frame updateiscamerachanging
    void Start()
    {
        //默认装备武器
        equip(0);
        _aimmode = AimMode.hip;

        //ads初始化
        iscaladstime = false;
        targetfov = adsfov;
        fov_current = camera.fieldOfView;
        iscamerachanging = false;

        //射击初始化
        t_firerate = 0;
        ani_iswitchtoads = false;
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

        //测试ADScamera变化
        if(iscamerachanging == true)
        {
            ads_cam();
        }

        //shoot
        if(_aimmode == AimMode.ads)
        {
            Shoot();
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
                iscamerachanging = true;
                targetfov = adsfov;
                startfov = fov_current;
            }
            
            //目前是ADS，转为hip
            if(_aimmode == AimMode.ads)
            {
                _aimmode = AimMode.hip;
                weaponanimator.SetTrigger("IsSwitchToHip");

                adscam_time_need = 0.12f;
                iscamerachanging = true;
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
            iscamerachanging = false;
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
        Debug.Log(t_firerate);
 
        if(t_firerate > 0)
        {
            t_firerate -= Time.deltaTime;
        }
  
        if (Input.GetMouseButtonDown (0))
        {
            if(t_firerate > firerate)
            {
                return;
            }

            float randomValue = Random.Range(1, 3);
            if(randomValue == 1)
            {
              
                weaponanimator.SetTrigger("SingleShoot1");
              
            }
            if (randomValue == 2)
            {
                weaponanimator.SetTrigger("SingleShoot2");
            }
            t_firerate = firerate;
        }
    }
   

    #endregion


}