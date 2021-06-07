using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motion : MonoBehaviour
{
    [Header("motion")]
    public float walkmovespeed;
    public float sprintmovespeed;
    public float idlespeedlimit;
    public float adsmovespeed;
    private float realmovespeed;
    private float _pramovespeed;
    public float Pramovespeed
    {
        get => this._pramovespeed;
    }

    private Rigidbody rb;
    
    [Header("Camera")]
    public Camera cam;
    public float fovmodifier;
    public float fovtranslator;
    private float basefov;
    private float realfov;

    [Header("Jump")]
    public float jumpforce;
    public LayerMask ground;
    public Transform grounddetector;
    public int jumpMaxNums;
    private int jumpnums;
    private bool isgrounded;
    private bool isfalling;
    private bool islanding;
    public float jumpinterval;
    public float t_jumpinterval;


    [Header("Weapon")]
    public Weapon weapon;

    public enum MoveMode
    {
        walk,sprint,idle,fall,ads
    }
    public MoveMode movemode;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        realmovespeed = walkmovespeed;
        movemode = MoveMode.walk;
        basefov = cam.fieldOfView;
        realfov = basefov;
        t_jumpinterval = 0f;
  
    }
    private void Update()
    {
        getspeed();
    }

    void FixedUpdate()
    {
        move();
        jump();

    }

    void move()
    {
        //移动方向
        float f_movedir = Input.GetAxis("Horizontal");
        float h_movedir = Input.GetAxis("Vertical");
        Vector3 movedir = new Vector3(f_movedir, 0, h_movedir).normalized;
        //移动模式
        if (weapon.T_adstime>= 0 && weapon._Aimmode == Weapon.AimMode.ads)
        {
            movemode = MoveMode.ads;
        }
        else if (Input.GetKey("q"))
        {
            if (h_movedir > 0)
            {
                movemode = MoveMode.sprint;
            }
        }
        else if (Pramovespeed <= idlespeedlimit)
        {
            movemode = MoveMode.idle;
        }
        else
        {
            movemode = MoveMode.walk;
        }
       
        //移动
        switch (movemode)
        {
            case MoveMode.walk:
                realmovespeed = walkmovespeed;
                realfov = Mathf.Lerp(cam.fieldOfView, basefov, Time.deltaTime * fovtranslator);
                break;
            case MoveMode.sprint:
                realmovespeed = sprintmovespeed;
                realfov = Mathf.Lerp(cam.fieldOfView, basefov * fovmodifier, Time.deltaTime * fovtranslator);
                break;
            case MoveMode.ads:
                realmovespeed = adsmovespeed;
                break;
        }



        Vector3 t_velocity = transform.TransformDirection(movedir * realmovespeed * Time.deltaTime);
        t_velocity.y = rb.velocity.y;
        rb.velocity = t_velocity;
        cam.fieldOfView = realfov;
    }
    void jump()
    {
        //跳跃
        if (Input.GetKeyDown("space"))
        {
            if (jumpnums < jumpMaxNums && t_jumpinterval <= 0)
            {
                rb.AddForce(0, jumpforce, 0, ForceMode.Acceleration);
                jumpnums++;
                t_jumpinterval = jumpinterval;
                weapon.jumpanimator();
            }
        }

        if (t_jumpinterval >= 0)
        {
            t_jumpinterval -= Time.deltaTime;
        }

        //下落
        isfalling = !isgrounded && rb.velocity.y < 0;
        if (isfalling)
        {
        }

        isgrounded = Physics.Raycast(grounddetector.position, Vector3.down, 0.1f, ground);
        if (isgrounded)
        {
            jumpnums = 0;
        }
        //接触地面的一瞬间
        islanding = isfalling && isgrounded;
        if (islanding)
        {
            Debug.Log("landing!");
            weapon.landanimator();
        }
    }
    
    void getspeed()
    {
        _pramovespeed = rb.velocity.magnitude;

    }

   
    


}
