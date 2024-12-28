using UnityEngine;

public class MLMoverVSAI : MonoBehaviour
{
    [Header("Settings")] 
    private float moveSpeed = 5f;

    [Header("References")] public EnvControllerVSAI EnvControllerVSAI;
    void Awake()
    {
        EnvControllerVSAI = GetComponentInParent<EnvControllerVSAI>();
    }

    // Update is called once per frame
    void Update()
    {
        moveSpeed = GameManagerVSAI.Instance.CalculateGameSpeed();
        transform.position += Vector3.left * (moveSpeed * Time.deltaTime);
    }
}
