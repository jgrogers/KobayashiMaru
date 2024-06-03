using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Dictionary<int, List<GameObject>> ships = new Dictionary<int, List<GameObject>> ();
    public static GameManager Instance = null;
    private float lastSpawnTime;
    [SerializeField] private List<GameObject> shipPrefabs = new List<GameObject> ();
    [SerializeField] private GameObject playerShip;
    [SerializeField] private GameObject kobayashiMaru;
    [SerializeField] private float DifficultyIncreasePerMinute = 1.0f;
    private float lastDifficultyIncreaseTime;
    private Dictionary<int, int> contactNum = new Dictionary<int, int> ();
    private bool firstWave = true;
    private IEnumerator LoseGame()
    {
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene(4);

    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Debug.LogError("Too many game managers in the kitchen");
            Destroy(gameObject);
            return;
        }
        lastSpawnTime = Time.time;
        lastDifficultyIncreaseTime = Time.time;
        //Populate teams
        ShipTeam[] allshipteams = FindObjectsOfType<ShipTeam>();
        foreach (ShipTeam shipteam in allshipteams)
        {
            GameObject ship = shipteam.gameObject;
            NotifyShipCreated(ship);
        }
    }
    private void FixedUpdate()
    {
        if (playerShip == null || kobayashiMaru == null)
        {
            StartCoroutine(LoseGame());
        }
        if (Time.time > lastDifficultyIncreaseTime + 60.0f)
        {
            GameSettings.Instance.SetDifficulty(DifficultyIncreasePerMinute + GameSettings.Instance.GameDifficultySetting);
            lastDifficultyIncreaseTime = Time.time;
            playerShip?.GetComponent<ShipControlBase>().IncrementShipLevel();
            if (playerShip != null)
                UITextUpdater.Instance.SetLevel(playerShip.GetComponent<ShipControlBase>().shipLevel);
        }
        float spawnInterval = firstWave? GameSettings.Instance.SpawnInterval/2.0f : GameSettings.Instance.SpawnInterval;
        if (Time.time > lastSpawnTime + spawnInterval * 0.25f)
        {
            UITextUpdater.Instance.SetReinforcementTime(spawnInterval - (Time.time - lastSpawnTime));
        }else
        {
            UITextUpdater.Instance.HideReinforcementText(true);
        }
        if (Time.time > lastSpawnTime + spawnInterval)
        {
            lastSpawnTime = Time.time;
            float pval = Random.value;
            if (true || firstWave || pval < GameSettings.Instance.ProbabilityOfSpawn)
            {
                firstWave = false;
                Debug.Log("Spawning!");
                int spawn_count = Random.Range(GameSettings.Instance.WaveSizeMin, GameSettings.Instance.WaveSizeMax+1);
                if (shipPrefabs.Count != 0)
                {
                    for (int i = 0; i < spawn_count; i++)
                    {
                        GameObject prefab = shipPrefabs[Random.Range(0, shipPrefabs.Count)];
                        float angle = Random.Range(0f, Mathf.PI * 2.0f);
                        Vector2 instance_offset = 80.0f * new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                        instance_offset += new Vector2(Random.Range(-10f, 10f), Random.Range(-10f, 10f));
                        Vector3 newcent;
                        if (playerShip) newcent = playerShip.transform.position;
                        else newcent = new Vector3(0, 0, 0);
                        GameObject ship = Instantiate(prefab, newcent + new Vector3(instance_offset.y, 0.0f, instance_offset.x), Quaternion.identity);
                        ship.GetComponent<ShipTeam>().team = 1;
                        NotifyShipCreated(ship);
                    }
                }
            } else
            {
                Debug.Log("Didn't spawn reinforcements this time. Pval was " + pval + " prob level for spawn is " + GameSettings.Instance.ProbabilityOfSpawn);
            }

        }

        
    }
    public void NotifyShipCreated(GameObject ship)
    {
        if (ship != null) {
            ShipTeam shipTeam = ship.GetComponent<ShipTeam>();
            int team = shipTeam.team;
            if (!ships.ContainsKey(team))
            {
                ships[team] = new List<GameObject>();
                contactNum[team] = 1;
            }
            if (shipTeam.shipName == "UNKNOWN")
            {
                //Designate contact name
                if (team == 0)
                    shipTeam.shipName = "Allied ship " + contactNum[team]++;
                else if (team == 1)
                    shipTeam.shipName = "Enemy ship " + contactNum[team]++;
                else
                    shipTeam.shipName = "Faction " + team + " ship " + contactNum[team]++;
            }
            ships[team].Add(ship);
        }
    }
    public void NotifyShipDestroyed(GameObject ship, GameObject reason = null)
    {
        foreach (int team in ships.Keys)
        {
            List<int> to_remove = new List<int>();
            for (int i = ships[team].Count-1; i >= 0; i--) {
                if (ship == ships[team][i])
                {
                    to_remove.Add(i); 
                }
            }
            foreach (int removeind in to_remove)
            {
                ships[team].RemoveAt(removeind);
            }
        }
        if (reason != null && reason.CompareTag("Player"))
        {
            GameSettings.Instance.IncrementScore();
        }
    }
    public Dictionary<int, List<GameObject>> GetAllShips()
    {
        return ships;
    }
    public GameObject GetKM()
    {
        return kobayashiMaru;
    }
    public List<GameObject> GetShipsInRangeFromTeamExcept(Vector3 origin, float range, GameObject exceptThis, int team)
    {
        List<GameObject> in_range = new List<GameObject>();
        foreach (GameObject go in ships[team])
        {
            if (go == exceptThis) continue;
            double dist = (go.transform.position - origin).magnitude;
            if (dist < range)
            {
                in_range.Add(go);
            }
        }
        return in_range;
    }
     public List<GameObject> GetShipsInRangeExcept(Vector3 origin, float range, GameObject exceptThis, int team_mask = -1)
    {
        List<GameObject> in_range = new List<GameObject>();
        foreach (int team in ships.Keys)
        {
            if (team == team_mask) continue;
            foreach (GameObject go in ships[team])
            {
                if (go == exceptThis) continue;
                double dist = (go.transform.position - origin).magnitude;
                if (dist < range)
                {
                    in_range.Add(go);
                }
            }
        }
        return in_range;
    }
     public List<GameObject> GetShipsInRangeFromTeam(Vector3 origin, float range, int team)
    {
        List<GameObject> in_range = new List<GameObject>();
        foreach (GameObject go in ships[team])
        {
            double dist = (go.transform.position - origin).magnitude;
            if (dist < range)
            {
                in_range.Add(go);
            }
        }
        return in_range;
    }
      public List<GameObject> GetShipsInRange(Vector3 origin, float range, int team_mask = -1)
    {
        List<GameObject> in_range = new List<GameObject>();
        foreach (int team in ships.Keys)
        {
            if (team == team_mask) continue;
            foreach (GameObject go in ships[team])
            {
                double dist = (go.transform.position - origin).magnitude;
                if (dist < range)
                {
                    in_range.Add(go);
                }
            }
        }
        return in_range;
    }
    public List<GameObject> GetShipsInRangeBearing(Transform origin, float range, float bearingLimit, int team_mask = -1)
    {
        List<GameObject> in_range = new List<GameObject>();
        foreach (int team in ships.Keys)
        {
            foreach (GameObject go in ships[team])
            {
                if (team == team_mask) continue;
                double dist = (go.transform.position - origin.transform.position).magnitude;
                if (dist < range)
                {
                    Vector3 local_position = origin.InverseTransformPoint(go.transform.position);
                    float angle = Mathf.Atan2(local_position.x, local_position.z);
                    if (Mathf.Abs(angle) < bearingLimit)
                    {
                        in_range.Add(go);
                    }
                    else
                    {
                    }
                }
            }
        }
        return in_range;
    }
}
