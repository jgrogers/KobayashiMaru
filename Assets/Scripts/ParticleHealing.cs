using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHealing : MonoBehaviour
{
    [SerializeField] private float healing = 0.1f;
    private ParticleSystem mHealerPS;
    private FireworksParticleSoundSystem mFireSounds;
    private void Awake()
    {
        mHealerPS = GetComponent<ParticleSystem>();
    }

    private void OnParticleCollision(GameObject other)
    {
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
        int numCollisionEvents = mHealerPS.GetCollisionEvents(other, collisionEvents);
        foreach (ParticleCollisionEvent collisionEvent in collisionEvents) // for each collision, do the following:
        {
            Vector3 pos = collisionEvent.intersection;
            other.GetComponent<ShipDamage>()?.Heal(healing);
        }
    }
}
