using UnityEngine;

public class MLMover : MonoBehaviour
{
    [Header("Settings")] 
    private float moveSpeed = 5f;

    [Header("References")] public EnvController EnvController;
    void Awake()
    {
        EnvController = GetComponentInParent<EnvController>();
    }

    // Update is called once per frame
    void Update()
    {
        moveSpeed = EnvController.CalculateGameSpeed();
        transform.position += Vector3.left * (moveSpeed * Time.deltaTime);
    }
}
