using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShipControlBase))]
public class AIControl : MonoBehaviour
{
    public bool isFiringTorpedoes = false; // Start is called before the first frame update
    public GameObject m_Target = null;
    private ShipTeam m_Team;
    private GameObject KM = null;
    private ShipControlBase baseControl;
    public bool isAggressive = true;
    public bool isHealer = false;
    [SerializeField] float healingRate = 0.1f;
    [SerializeField] float aggroRange = 30.0f;
    void Start()
    {
        baseControl = GetComponent<ShipControlBase>();
        m_Team = GetComponent<ShipTeam>();
        KM = GameManager.Instance.GetKM();
    }

    // Update is called once per frame
    void FixedUpdate()
    {   if (isAggressive)
        {
            //Choose a target if we dont have one
            //First, always check if a ship besides the KM is within 20 of us. If so, change to that target
            if (m_Target == null)
            {
                List<GameObject> ships = GameManager.Instance.GetShipsInRangeExcept(transform.position, aggroRange, KM, m_Team.team);
                if (ships.Count > 0)
                {
                    m_Target = ships[Random.Range(0, ships.Count)];
                    Debug.Log("Ship " + m_Team.shipName + " is now targetting " + m_Target.GetComponent<ShipTeam>().shipName);
                }
            }
            GameObject attackThis = KM;
            if (m_Target != null)
            {
                attackThis = m_Target;
            }
            if (attackThis != null)
            {
                ManeuverToTarget(attackThis);
                AttackIfInRange(attackThis);
            }
            else
            {
                baseControl.StopFiringAllLasers();
            }
        }
    if (isHealer)
        {
            baseControl.StopFiringAllLasers();
            List<GameObject> ships = GameManager.Instance.GetShipsInRangeFromTeamExcept(transform.position, 10f, this.gameObject, m_Team.team);
            bool healed = false;
            foreach (GameObject ship in ships)
            {
                if (ship.GetComponent<ShipDamage>().hitPoints < ship.GetComponent<ShipDamage>().maxHitPoints)
                {
                    Vector3 localPosition = transform.InverseTransformPoint(ship.transform.position);
                    float angle = Mathf.Rad2Deg * Mathf.Atan2(localPosition.x, localPosition.z);
                    baseControl.HealInDirection(angle);
                    healed = true;
                    break;
                }
            }
           
            if(!healed){
                baseControl.StopHealing();
            }
        }
    }

    private void AttackIfInRange(GameObject attackThis)
    {
        Vector3 localPosition = transform.InverseTransformPoint(attackThis.transform.position);
        float distance = localPosition.magnitude;
        float bearing = Mathf.Abs(Mathf.Rad2Deg * Mathf.Atan2(localPosition.x, localPosition.z));
        if (bearing < 10.0f && distance < 50f)
        {
            baseControl.FireAllLasers();
        }
        else
        {
            baseControl.StopFiringAllLasers();
        }
    }

    private void ManeuverToTarget(GameObject attackThis)
    {
        Vector3 worldPos = attackThis.transform.position - transform.position;
        baseControl.goalHeading = Mathf.Rad2Deg * Mathf.Atan2(worldPos.x, worldPos.z);
        if (worldPos.magnitude > 5f)
        {
            baseControl.goalSpeed = 70.0f;
        } else if (worldPos.magnitude < 3)
        {
            baseControl.goalSpeed = -25.0f;
        } else
        {
            baseControl.goalSpeed = 0.0f;
        }
    }
}
