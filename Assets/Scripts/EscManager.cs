using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EscManager : MonoBehaviour
{
    UnityEngine.InputSystem.Keyboard keyboard;
    UnityEngine.InputSystem.Mouse mouse;
    // Start is called before the first frame update
    void Start()
    {
        keyboard = Keyboard.current;
        mouse = Mouse.current;
    }

    // Update is called once per frame
    void Update()
    {
        if (keyboard != null)
        {
            if (keyboard.spaceKey.isPressed)
            {
                SceneManager.LoadScene(1);
            }
        }
        if (mouse != null)
        {
            if (mouse.leftButton.isPressed || mouse.rightButton.isPressed)
            {
                SceneManager.LoadScene(1);
            }
        }
    }
}
