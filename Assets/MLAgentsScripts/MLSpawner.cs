using System;
using UnityEngine;
using Random = UnityEngine.Random;


public class MLSpawner : MonoBehaviour
{
    [Header("Settings")] 
    public float minSpawnDelay;
    public float maxSpawnDelay;

    [Header("References")] 
    public EnvController EnvController;
    public GameObject[] gameObjects;
    void OnEnable()
    {
        Invoke(nameof(Spawn), minSpawnDelay);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Spawn()
    {
        var randomObject = gameObjects[Random.Range(0, gameObjects.Length)];

        Instantiate(randomObject, transform.position, Quaternion.identity, EnvController.gameObject.transform); 
        Invoke(nameof(Spawn), Random.Range(minSpawnDelay, maxSpawnDelay));
    }
}
