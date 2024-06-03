using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody))]
public class PDControl : MonoBehaviour
{
    [SerializeField] float Kp = 1.0f;
    [SerializeField] float Kd = -4.0f;
    [SerializeField] float Ki = 0.0f;
    [SerializeField] float maxSignal = 0.0f;  // Signal abs vel threshold, set above zero to limit control output
    [SerializeField] float maxAngularSignal = 0.0f;  // Signal abs vel threshold, set above zero to limit control output
    [SerializeField] float maxAngularError = 10.0f;
    [SerializeField] float maxError = 10.0f;
    public Vector3 angularErrorIntegral = new Vector3(0, 0, 0);
    public Vector3 lastDesiredAngles = new Vector3(0, 0, 0);
    public Vector3 errorIntegral = new Vector3(0, 0, 0);
    public Vector3 lastDesired = new Vector3(0, 0, 0);
    // Start is called before the first frame update
    private Rigidbody rb;
    void Start()
    {
       rb = GetComponent<Rigidbody>();
    }

    public static Vector3 WrapAngles(Vector3 angles)
    {
        angles.x = WrapAngle(angles.x);
        angles.y = WrapAngle(angles.y);
        angles.z = WrapAngle(angles.z);
        return angles;
    }
    public static float WrapAngle(float angle)
    {
        while (angle > 180.0f) angle -= 360.0f;
        while (angle < -180.0f) angle += 360.0f;
        return angle;
    }
    public Vector3 GetControlTorquesWorld(Vector3 desiredAngles, Vector3 currentAngles)
    {
        if (lastDesiredAngles != desiredAngles) {
            //Reset the integral term
            angularErrorIntegral = new Vector3(0, 0, 0);
            lastDesiredAngles = desiredAngles;
        }
//        desiredAngles = WrapAngles(desiredAngles);
//        currentAngles = WrapAngles(currentAngles);
        Vector3 currentAngularVelocity = rb.angularVelocity;
//Vector3 angleDirection = new Vector3(
 //Mathf.MoveTowardsAngle(currentAngles.x, desiredAngles.x, maxAngularError),
  //Mathf.MoveTowardsAngle(currentAngles.y, desiredAngles.y, maxAngularError),
   //Mathf.MoveTowardsAngle(currentAngles.z, desiredAngles.z, maxAngularError));
        Vector3 error = WrapAngles(desiredAngles - currentAngles);
        Vector3 control = Kp * error + Kd * currentAngularVelocity + Ki * angularErrorIntegral;
        if (maxAngularSignal > 0.0f)
        {
            return new Vector3(
                Mathf.Clamp(control.x, -maxAngularSignal, maxAngularSignal),
                Mathf.Clamp(control.y, -maxAngularSignal, maxAngularSignal),
                Mathf.Clamp(control.z, -maxAngularSignal, maxAngularSignal));
        }
        else return control;
    }
    public Vector3 GetControlTorquesLocal(Vector3 desiredAngles, Vector3 currentAngles)
    {
        Vector3 currentAngularVelocity = transform.InverseTransformDirection(rb.angularVelocity);
        Vector3 error = WrapAngles(desiredAngles - currentAngles);
        Vector3 control = Kp * error + Kd * currentAngularVelocity; // + Ki * errorIntegral;
        if (maxSignal > 0.0f)
        {
            return new Vector3(
                Mathf.Clamp(control.x, -maxSignal, maxSignal),
                Mathf.Clamp(control.y, -maxSignal, maxSignal),
                Mathf.Clamp(control.z, -maxSignal, maxSignal));
        }
        else return control;
    }
     public Vector3 GetControlForcesWorld(Vector3 desiredVelocity) {
        if (lastDesired != desiredVelocity) {
            //Reset the integral term
            errorIntegral = new Vector3(0, 0, 0);
            lastDesired = desiredVelocity;
        }
        Vector3 currentVelocity = rb.velocity;
        Vector3 error = new Vector3(
            Mathf.MoveTowards(currentVelocity.x, desiredVelocity.x, maxError),
            Mathf.MoveTowardsAngle(currentVelocity.y, desiredVelocity.y, maxError),
            Mathf.MoveTowardsAngle(currentVelocity.z, desiredVelocity.z, maxError));
        Vector3 control = Kp * error + Kd * currentVelocity + Ki * errorIntegral;
        if (maxSignal > 0.0f)
        {
            return new Vector3(
                Mathf.Clamp(control.x, -maxSignal, maxSignal),
                Mathf.Clamp(control.y, -maxSignal, maxSignal),
                Mathf.Clamp(control.z, -maxSignal, maxSignal));
        }
        else return control;
    }
     public Vector3 GetControlForcesLocal(Vector3 desiredVelocity)
    {
        Vector3 currentVelocity = transform.InverseTransformDirection(rb.velocity);
        Vector3 error = new Vector3(
            Mathf.MoveTowards(currentVelocity.x, desiredVelocity.x, maxError),
            Mathf.MoveTowardsAngle(currentVelocity.y, desiredVelocity.y, maxError),
            Mathf.MoveTowardsAngle(currentVelocity.z, desiredVelocity.z, maxError));
        Vector3 control = Kp * error + Kd * currentVelocity;// + Ki * errorIntegral;
        if (maxSignal > 0.0f)
        {
            return new Vector3(
                Mathf.Clamp(control.x, -maxSignal, maxSignal),
                Mathf.Clamp(control.y, -maxSignal, maxSignal),
                Mathf.Clamp(control.z, -maxSignal, maxSignal));
        }
        else return control;
    }
}
