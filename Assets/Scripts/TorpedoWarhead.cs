using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorpedoWarhead : MonoBehaviour
{
    public float warheadDamage = 1.0f;
    [SerializeField] private ParticleSystem warheadExplosionPS;
    [SerializeField] private TorpedoHoming myGuidance; 
    [SerializeField] private List<AudioClip> explosionSounds = new List<AudioClip>();
    public GameObject origin;

    public float mLifespan = 5.0f;
    public float m_ArmedCount = 0.0f;
    public float m_ArmingTime = 0.5f;
    public bool m_Armed = false;
    void FixedUpdate()
    {
        m_ArmedCount += Time.deltaTime;
        if (!m_Armed)
        {
            if (m_ArmedCount >= m_ArmingTime)
            {
                m_Armed = true;
            }
        }
        mLifespan -= Time.deltaTime;
        if (mLifespan < 0)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (m_Armed == false) return;
        if (other.gameObject.CompareTag("Torpedo")) return;
        other.gameObject.GetComponent<ShipDamage>()?.Damage(warheadDamage, origin);
        ParticleSystem explosion = Instantiate(warheadExplosionPS, transform.position, Quaternion.identity);
        if (explosionSounds.Count > 0)
        {
            var AS = explosion.gameObject.AddComponent<AudioSource>();
            AS.clip = explosionSounds[Random.Range(0, explosionSounds.Count)];
            AS.Play();
        }
        Destroy(gameObject);
    }
}
