using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UITextUpdater : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI LevelText;
    [SerializeField] private TMPro.TextMeshProUGUI ScoreText;
    [SerializeField] private TMPro.TextMeshProUGUI ReinforcementTimeText;

    static public UITextUpdater Instance { get; private set; }
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Debug.LogError("Only one UITextUpdater instance at a time please");
            Destroy(this.gameObject);
        }
        SetScore(0.0f);
        SetLevel(0.0f);
        SetReinforcementTime(0.0f);
        
    }

    public void SetScore(float score)
    {
        ScoreText.text = "Score: " + ((int)Mathf.Floor(score)).ToString();
    }
    public void SetLevel(float level)
    {
        LevelText.text = "Level: " + ((int)Mathf.Floor(level)).ToString();
    }
    public void SetReinforcementTime(float time)
    {
        ReinforcementTimeText.enabled = true;
        ReinforcementTimeText.text = "Enemy fleet inbound - arriving in " + (Mathf.FloorToInt(time)).ToString() + " seconds";
    }
    public void HideReinforcementText(bool hide)
    {
        ReinforcementTimeText.enabled = !hide;
    }
}
