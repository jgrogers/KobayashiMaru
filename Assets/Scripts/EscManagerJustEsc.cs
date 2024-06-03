using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EscManagerJustEsc : MonoBehaviour
{
    UnityEngine.InputSystem.Keyboard keyboard;
    UnityEngine.InputSystem.Mouse mouse;
    // Start is called before the first frame update
    void Start()
    {
        keyboard = Keyboard.current;
    }

    // Update is called once per frame
    void Update()
    {
        if (keyboard != null)
        {
            if (keyboard.escapeKey.isPressed)
            {
                Application.Quit();
            }
        }
    }
}
