using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]   
[RequireComponent(typeof(PDControl))]   
public class ShipControlBase : MonoBehaviour
{
    Rigidbody mRigidbody;
    PDControl mControl;
    public GameObject worldOrientedAimPoint;
    private List<ParticleSystem> mLasersPS;
    private List<ParticleSystem> mHealPS;
    private TorpedoLauncher[] mTorpedoLaunchers;
    public float goalHeading;
    public float goalSpeed;
    public float engineStrengthMultiplier = 1.0f;
    public float shipLevel = 1f;
    // Start is called before the first frame update
    void Start()
    {
        mRigidbody = GetComponent<Rigidbody>();
        mControl = GetComponent<PDControl>();
        var mLasers = GetComponentsInChildren<ParticleDamage>();
        var mHealers = GetComponentsInChildren<ParticleHealing>();
        mLasersPS = new List<ParticleSystem>();
        mHealPS = new List<ParticleSystem>();
        foreach (var laser in mLasers)
        {
            mLasersPS.Add(laser.GetComponent<ParticleSystem>());
        }
        foreach(var healer in mHealers)
        {
            mHealPS.Add(healer.GetComponent<ParticleSystem>());
        }
        
        mTorpedoLaunchers = GetComponentsInChildren<TorpedoLauncher>();
        goalHeading = 0.0f;
        goalSpeed = 0.0f;
        UpdateShipLevel(shipLevel);
        StopFiringAllLasers();
    }
    public void IncrementShipLevel()
    {
        UpdateShipLevel(shipLevel + 1);
    }
    public void UpdateShipLevel(float newShipLevel)
    {
        shipLevel = newShipLevel;
        float laserRange = 60.0f * (1.0f + shipLevel / 5f);
        float laserSpeed = 60.0f * (1.0f + shipLevel / 5f);
        float laserEmissionRateMultiplier = 1.0f + shipLevel / 3f;
        float maxTorpedoes = 3.0f + shipLevel / 20f;
        float torpedoReloadRate = 0.3f + 0.01f * shipLevel;
        engineStrengthMultiplier = shipLevel / 5f;
        foreach (ParticleSystem laser in mLasersPS)
        {
            var lmain = laser.main;
            lmain.startSpeed = laserSpeed;
            lmain.startLifetime = laserRange / laserSpeed;
            var emission = laser.emission;
            emission.rateOverTimeMultiplier = laserEmissionRateMultiplier;
        }
        foreach (TorpedoLauncher torp in mTorpedoLaunchers)
        {
            torp.SetMaxTorpedoes(maxTorpedoes);
            torp.SetTorpedoReloadRate(torpedoReloadRate);
        }
     }
    private void Update()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        worldOrientedAimPoint.transform.localRotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, -transform.rotation.w);
        Vector3 desiredAngles = new Vector3(0, goalHeading, 0);
        Vector3 controlTorque = mControl.GetControlTorquesWorld(desiredAngles, transform.rotation.eulerAngles);
        mRigidbody.AddTorque(controlTorque);
        Vector3 desiredLocalVelocity = new Vector3(0, 0, goalSpeed);
        Vector3 localControlForces = mControl.GetControlForcesLocal(desiredLocalVelocity);
        mRigidbody.AddRelativeForce(20f * engineStrengthMultiplier * localControlForces);
        Debug.DrawLine(transform.position, transform.position + 2.0f * new Vector3(Mathf.Sin(Mathf.Deg2Rad*goalHeading), 0.0f, Mathf.Cos(Mathf.Deg2Rad*goalHeading)),Color.green);
     }

    public void FireTorpedo()
    {
        foreach (TorpedoLauncher torp in mTorpedoLaunchers)
        {
            torp.LaunchTorpedo();
        }

    }
    public void HealInDirection(float angle)
    {
        foreach (var healer in mHealPS)
        {
            var shape = healer.shape;
            //shape.rotation = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), 0f, Mathf.Sin(Mathf.Deg2Rad*angle)); 
            shape.rotation = new Vector3(0, angle, 0);
            if (!healer.emission.enabled)
            {
                var emission = healer.emission;
                emission.enabled = true;
            }
        }
 
    }
    public void StopHealing()
    {
        foreach (var healer in mHealPS)
        {
            if (healer.emission.enabled)
            {
                var emission = healer.emission;
                emission.enabled = false;
            }
        }
    }
    public void FireAllLasers()
    {
        foreach (var laser in mLasersPS)
        {
            if (!laser.emission.enabled) {
                var emission = laser.emission;
                emission.enabled = true;
            }
        }
    }
    public void StopFiringAllLasers()
    {
        foreach (var laser in mLasersPS)
        {
            if (laser.emission.enabled) {
                var emission = laser.emission;
                emission.enabled = false;
            }
        }
 
    }
}
