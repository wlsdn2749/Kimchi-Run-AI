
using System.Collections.Generic;

using UnityEngine;

public class EnvControllerVSAI : MonoBehaviour
{
    [Header("References")]
    public GameObject EnemySpawner;
    public GameObject FoodSpawner;
    public GameObject GoldSpawner;
    public InferenceAgent InferenceAgent;
    
    public float PlayStartTime;
    
    [Header("Settings")]
    public int Lives = 3;
    public float gameSpeed = 8;
    public float minSpeed = 8;
    public float maxSpeed = 20;
    public bool isAlive = true;
    
    [Header("MLAgents")] public float totalReward = 0f;

    public void Reset()
    {
        Lives = 3;
        totalReward = 0f;
        PlayStartTime = Time.time;
    }

    // public float CalculateScore()
    // {
    //     return Time.time - PlayStartTime; 
    // }
    //
    //
    // public float CalculateGameSpeed()
    // {  
    //     gameSpeed = minSpeed + (0.75f * Mathf.Floor(CalculateScore() / 10f));
    //     return Mathf.Min(gameSpeed, maxSpeed);
    // }
    
    // Update is called once per frame
    void Update()
    {
        if (Lives == 0 || GameManagerVSAI.Instance.CalculateScore() >= 1000)
        {
            if (isAlive)
            {
                isAlive = false;
                InferenceAgent.KillPlayer();
                GameManagerVSAI.Instance.isPlayerWIN = true;
                GameManagerVSAI.Instance.PlayerWIN.text = "Player Win!! AI is sad :(";
            }
        }

    }
}
