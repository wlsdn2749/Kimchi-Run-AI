using UnityEngine;

public class Mover : MonoBehaviour
{
    [Header("Settings")] 
    private float moveSpeed = 5f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moveSpeed = GameManager.Instance.CalculateGameSpeed();
        transform.position += Vector3.left * (moveSpeed * Time.deltaTime);
    }
}
