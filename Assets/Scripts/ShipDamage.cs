using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipDamage : MonoBehaviour
{
    [SerializeField] public float maxHitPoints = 3f;
    [SerializeField] private bool isImmortal = false;
    [SerializeField] private ParticleSystem explosionPS = null;
    [SerializeField] private List<AudioClip> explosionSounds= new List<AudioClip>();
    public float hitPoints;
    // Start is called before the first frame update
    void Awake()
    {
        hitPoints = maxHitPoints; 
    }
    public void Damage(float damage, GameObject origin = null)
    {
        hitPoints -= damage;
        if (hitPoints <= 0)
        {
            if (!isImmortal)
            {
                if (explosionPS != null)
                {
                    ParticleSystem explosion = Instantiate(explosionPS, transform.position, Quaternion.identity);
                    if (explosionSounds.Count > 0) {
                        var AS = explosion.gameObject.AddComponent<AudioSource>();
                        AS.clip = explosionSounds[Random.Range(0, explosionSounds.Count)];
                        AS.Play();
                    }

                }
                GameManager.Instance.NotifyShipDestroyed(gameObject, origin);
                if (gameObject.CompareTag("Player"))
                    Camera.main.transform.SetParent(null, true);
                Destroy(gameObject);
            }
        }
    }
    public void Heal(float damage)
    {
        hitPoints += damage;
        if (hitPoints > maxHitPoints)
        {
            hitPoints = maxHitPoints;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Ship hit by " + collision.gameObject.name);
    }
}
