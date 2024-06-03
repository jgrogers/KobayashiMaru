using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ShipControlBase))]   
public class PlayerController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera guiCamera;
    UnityEngine.InputSystem.Keyboard keyboard;
    UnityEngine.InputSystem.Mouse mouse;
    //private CinemachineFramingTransposer framingTransposer = null;
    public bool isFiringTorpedoes = false;
    private ShipControlBase baseControl;
    // Start is called before the first frame update
    void Start()
    {
        baseControl = GetComponent<ShipControlBase>();
        keyboard = Keyboard.current;
        mouse = Mouse.current;
     //   CinemachineComponentBase componentBase = guiCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
      //  if (componentBase is CinemachineFramingTransposer)
       // {
       //     framingTransposer = componentBase as CinemachineFramingTransposer;
        //}
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (mouse != null)
        {
            //Set the camera distance with scroll wheel
            //   if (framingTransposer)
            //  {
            //     framingTransposer.m_CameraDistance -= 0.05f * mouse.scroll.value.y;
            //    if (framingTransposer.m_CameraDistance < 10.0f) framingTransposer.m_CameraDistance = 10.0f;
            //}
            Camera.main.transform.position -= new Vector3(0f, 0.05f * mouse.scroll.value.y, 0f);
            if (Camera.main.transform.position.y < 5f) new Vector3(0f, 5f, 0f);
            if (mouse.rightButton.isPressed || mouse.leftButton.isPressed) {
                Vector2 screen_center = new Vector2(Screen.width / 2, Screen.height / 2);
                UnityEngine.InputSystem.Controls.Vector2Control mousePosition = mouse.position;
                Vector2 aim_direction = (mousePosition.value - screen_center).normalized;
                float mag = (mousePosition.value - screen_center).magnitude;
                float depth_of_player = (transform.position - Camera.main.transform.position).magnitude;
                Vector3 screenpos = mousePosition.value;
                screenpos.z = 0.0f;
                Ray ray = Camera.main.ScreenPointToRay(screenpos);
                Vector3 world_pos = ray.origin + ray.direction * depth_of_player / (-ray.direction.y);
                Vector3 control_direction = world_pos - transform.position;
                baseControl.goalHeading = Mathf.Rad2Deg * Mathf.Atan2(control_direction.x, control_direction.z);
                baseControl.goalSpeed = Mathf.Pow(control_direction.magnitude, 1.2f);
            }
            if (mouse.leftButton.isPressed)
            {
                baseControl.FireAllLasers();
            } 
            else
            {
                baseControl.StopFiringAllLasers();
            }
        }
        if (keyboard != null)
        {
            if (keyboard.sKey.isPressed)
            {
                baseControl.goalSpeed = -20.0f;

            }
            if (keyboard.spaceKey.isPressed)
            {
                if (!isFiringTorpedoes)
                {
                    baseControl.FireTorpedo();
                    isFiringTorpedoes = true;
                }
            } else
            {
                isFiringTorpedoes = false;
            }
        }
    }
}
