
using System.Collections.Generic;

using UnityEngine;

public class EnvController : MonoBehaviour
{
    [Header("References")]
    public GameObject EnemySpawner;
    public GameObject FoodSpawner;
    public GameObject GoldSpawner;
    public PlayerAgent PlayerAgent;
    
    public float PlayStartTime;
    
    [Header("Settings")]
    public int Lives = 3;
    public float gameSpeed = 8;
    public float minSpeed = 8;
    public float maxSpeed = 20;


    [Header("MLAgents")] public float totalReward = 0f;
    private List<GameObject> GetChildObjects(Transform parentTransform)
    {
        List<GameObject> childObjects = new List<GameObject>();

        if (parentTransform != null)
        {
            foreach (Transform childTransform in parentTransform)
            {
                childObjects.Add(childTransform.gameObject);
            }
        }

        return childObjects;
    }

    private void RemoveAllChildrenEntities()
    {
        List<GameObject> entities = GetChildObjects(transform);
        
        foreach (var entity in entities)
        {
            Destroy(entity);
        }

    }
    void Start()
    {
    }

    public void Reset()
    {
        RemoveAllChildrenEntities();
        PlayerAgent.ResetPlayer();
        Lives = 3;
        totalReward = 0f;
        EnemySpawner.SetActive(true);
        FoodSpawner.SetActive(true);
        GoldSpawner.SetActive(true);
        PlayStartTime = Time.time;
    }

    public float CalculateScore()
    {
        return Time.time - PlayStartTime; 
    }
    

    public float CalculateGameSpeed()
    {  
        gameSpeed = minSpeed + (0.75f * Mathf.Floor(CalculateScore() / 10f));
        return Mathf.Min(gameSpeed, maxSpeed);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Lives == 0 || CalculateScore() >= 1000)
        {
            PlayerAgent.KillPlayer();
            EnemySpawner.SetActive(false);
            FoodSpawner.SetActive(false);
            GoldSpawner.SetActive(false);
            PlayerAgent.EndEpisode();
        }

    }
}
