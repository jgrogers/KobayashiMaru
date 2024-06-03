using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDamage : MonoBehaviour
{
    [SerializeField] private float weaponDamage = 1.0f;
    [SerializeField] private ParticleSystem mHitPS;
    private ParticleSystem mLaserPS;
    private FireworksParticleSoundSystem mFireSounds;
    private void Awake()
    {
        mLaserPS = GetComponent<ParticleSystem>();
    }

    private void OnParticleCollision(GameObject other)
    {
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
        int numCollisionEvents = mLaserPS.GetCollisionEvents(other, collisionEvents);
        foreach (ParticleCollisionEvent collisionEvent in collisionEvents) // for each collision, do the following:
        {
            Vector3 pos = collisionEvent.intersection;
            Instantiate(mHitPS, pos, Quaternion.identity);
            other.GetComponent<ShipDamage>()?.Damage(weaponDamage, transform.parent.gameObject);
        }
    }
}
