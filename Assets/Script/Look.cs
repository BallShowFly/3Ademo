using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Look : MonoBehaviour
{
    public Transform player;
    public Transform cam;
    private Quaternion camoriginpo;
    public float Ysensivity;
    public float Xsensivity;
    public float maxsangle;
    static public bool iscursorlock;
    public GameObject weapon;
    // Start is called before the first frame update
    void Start()
    {
        camoriginpo = cam.localRotation;
        iscursorlock = false;
        setcursormode();
    }

    // Update is called once per frame
    void Update()
    {
        setY();
        setX();
        if (Input.GetKeyDown("`"))
        {
            changecursormode();
            setcursormode();
        }
        
    }
    void setY()
    {
        float t_camYmove = Input.GetAxis("Mouse Y") * Ysensivity * Time.deltaTime;
        Quaternion t_adj = Quaternion.AngleAxis(t_camYmove, -Vector3.right);
        Quaternion t_delata = cam.localRotation * t_adj;

        if (Quaternion.Angle(camoriginpo, t_delata) < maxsangle)
        {
            cam.localRotation = t_delata;
 
            weapon.transform.localRotation = cam.localRotation;
        }

    }
    void setX()
    {
        float t_camXmove = Input.GetAxis("Mouse X") * Xsensivity * Time.deltaTime;
        Quaternion t_adj = Quaternion.AngleAxis(t_camXmove, Vector3.up);
        Quaternion t_delata = player.localRotation * t_adj;
        player.localRotation = t_delata;
    }
    void changecursormode()
    {
        if (iscursorlock == true)
        {
            iscursorlock = false;
            return;
        }
        if (iscursorlock == false)
        {
            iscursorlock = true;
        }
    }


    void setcursormode()
    {
        if (iscursorlock == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
   
}
