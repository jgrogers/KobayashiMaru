using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameSettings : MonoBehaviour
{
    public float score { get; private set; } = 0f;
    public float GameDifficultySetting { get; private set; } = 0;
    public float ProbabilityOfSpawn { get; private set; } = 0.0f;
    public int WaveSizeMin { get; private set; } = 0;
    public int WaveSizeMax { get; private set; } = 1;
    public float EnemyShipLevel { get; private set; } = 1f;
    public float SpawnInterval { get; private set; } = 30.0f;

    [SerializeField] private float initialDifficulty = 0;
    public static GameSettings Instance { get; private set;}

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Debug.LogError("Too many game settings alive at once!");
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);//These settings persist and are configured via the menu or something
        SetDifficulty(initialDifficulty);
        score = 0;
    }
    public void SetDifficulty(float difficulty)
    {
        GameDifficultySetting = difficulty;
        ProbabilityOfSpawn = 0.6f + 0.4f * (float)Math.Tanh(0.05 * difficulty);
        WaveSizeMin = (int)Mathf.Floor(1f + difficulty);
        WaveSizeMax = (int)Mathf.Floor(1f + difficulty * 1.5f);
        SpawnInterval = difficulty >= 1f ? Mathf.Clamp(60.0f / Mathf.Pow(difficulty, 1f / 6f), 0.0f, 60.0f) : 60f;
        EnemyShipLevel = 1f + difficulty / 2.0f;
        Debug.Log("The difficulty is now " + difficulty +
            " Prob of spawn : " + ProbabilityOfSpawn +
            "Wave size [" + WaveSizeMin + ", " + WaveSizeMax +
            "] Spawn interval : " + SpawnInterval + "s");
    }
    public void IncrementScore()
    {
        score += 1.0f;
        UITextUpdater.Instance?.SetScore(score);
    }
}
