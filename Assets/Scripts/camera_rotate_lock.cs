using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_rotate_lock : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera mainCamera;
    [SerializeField] float turnSpeed = 1.0f;
    [SerializeField] float radius = 10.0f;
    void Start()
    {
       mainCamera = Camera.main; 
    }

    // Update is called once per frame
    void Update()
    {
       mainCamera.transform.position = new Vector3(radius * Mathf.Cos(Time.time * turnSpeed), 0f, radius* Mathf.Sin(Time.time*turnSpeed)); 
       mainCamera.transform.rotation = Quaternion.LookRotation(-mainCamera.transform.position, Vector3.up);
    }
}
