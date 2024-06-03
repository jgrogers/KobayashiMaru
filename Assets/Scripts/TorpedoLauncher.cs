using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorpedoLauncher : MonoBehaviour
{
    [SerializeField] private GameObject mTorpedoPrefab;
    [SerializeField] private float mTorpedoReloadRate = 0.3f;
    [SerializeField] private float m_ArmingTime = 1.0f;
    [SerializeField] private float m_MaxTorpedoes = 4.0f;
    public float mTorpedoLoaded;
    private void Awake()
    {
        mTorpedoLoaded = m_MaxTorpedoes;
    }
    public void FixedUpdate()
    {
        mTorpedoLoaded += Time.deltaTime * mTorpedoReloadRate;
        if (mTorpedoLoaded > m_MaxTorpedoes) { mTorpedoLoaded = m_MaxTorpedoes; }
    }
    public void SetMaxTorpedoes(float value)
    {
        m_MaxTorpedoes = value;
    }
    public void SetTorpedoReloadRate(float value)
    {
        mTorpedoReloadRate = value;

    }
    public void LaunchTorpedo()
    {
        if (mTorpedoLoaded >= 1.0f)
        {
            mTorpedoLoaded -= 1.0f;
            GameObject torpedo = Instantiate(mTorpedoPrefab, transform.position, transform.rotation);
            torpedo.GetComponent<Rigidbody>().velocity = GetComponentInParent<Rigidbody>().velocity + 1.0f * transform.forward;
            torpedo.GetComponent<TorpedoHoming>().team = GetComponentInParent<ShipTeam>().team;
            torpedo.GetComponent<TorpedoWarhead>().origin = transform.parent.gameObject;
        }
    }
}
