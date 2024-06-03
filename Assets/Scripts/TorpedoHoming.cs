using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(PDControl))]
[RequireComponent(typeof(Rigidbody))]
public class TorpedoHoming : MonoBehaviour
{
    public GameObject mTarget = null;
    private Rigidbody mRigidbody;
    public float seekerRange = 10.0f;
    public float seekerAngle = 1.0f;
    private PDControl mControl;
    [SerializeField] float Kp = 1.0f;
    [SerializeField] float Kd = 0.1f;
    [SerializeField] float rotationTorque = 10.0f;
    [SerializeField] float thrust = 0.00f;
    public int team = -1;

    private void Awake()
    {
        mRigidbody = GetComponent<Rigidbody>();
        mControl = GetComponent<PDControl>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (mTarget != null)
        {
            Vector3 targetPosition = transform.InverseTransformPoint(mTarget.transform.position);
            float yawError = Mathf.Rad2Deg * Mathf.Atan2(targetPosition.x, targetPosition.magnitude);
            float pitchError = Mathf.Rad2Deg * Mathf.Atan2(targetPosition.y, targetPosition.magnitude);
            Vector3 requiredTorque = mControl.GetControlTorquesLocal(new Vector3(0, yawError, pitchError), new Vector3(0, 0, 0));

            mRigidbody.AddRelativeTorque(requiredTorque);
        } else
        {
            // See if we can find a target
            mTarget = FindTarget();
            if (mTarget != null)
            {
                Debug.Log("Torpedo locked on target " +  mTarget.name);
            }
        }
        mRigidbody.AddRelativeForce(0.0f, 0.0f, thrust);
    }
    private GameObject FindTarget() {
        List<GameObject> ships = GameManager.Instance.GetShipsInRangeBearing(transform, seekerRange, seekerAngle, team);
        GameObject closest = null;
        float closest_distsq = 0f;
        foreach (GameObject go in ships)
        {
            float distsq = (transform.position - go.transform.position).sqrMagnitude;
            if (closest == null || distsq < closest_distsq)
            {
                closest_distsq = distsq;
                closest = go;
            }
        }
        return closest;
    }
}
