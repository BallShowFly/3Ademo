using System.Collections;
using System.Collections.Generic;
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


    private bool iscaladstime;
    public Camera camera;
    public double adsfov;
    private bool ani_iswitchtoads;

    public enum AimMode
    {
        hip,ads
    }

    // Start is called before the first frame update
    void Start()
    {
        equip(0);
        _aimmode = AimMode.hip;
        iscaladstime = false;
        adsfov = 39.15295f;
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
    #region ADS
    public void ads()
    {
        //按下鼠标右键后的判断
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("get mouse2 down!");
            //目前状态为腰射，开始计时
            if(_aimmode == AimMode.hip)
            {
                iscaladstime = true;
                ani_iswitchtoads = true;
                weaponsway.transform.localEulerAngles = Vector3.zero;
                weaponanimator.SetTrigger("IsSwitchToAds");
            }
            //正在从腰射转为ADS，重新开始计时
            if(t_adstime > 0)
            {
                t_adstime = 0f;
                iscaladstime = true;
                weaponanimator.SetTrigger("IsSwitchToAds");
            }
            //目前是ADS，转为hip
            if(_aimmode == AimMode.ads)
            {
                _aimmode = AimMode.hip;
                weaponanimator.SetTrigger("IsSwitchToHip");
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

  


    #endregion




}
