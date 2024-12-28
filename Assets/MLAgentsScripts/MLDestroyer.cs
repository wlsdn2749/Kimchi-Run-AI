using UnityEngine;

public class MLDestroyer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // localPosition은 Prefab의 자식이기 때문에 -10으로만 해도 됨
        if (transform.localPosition.x < -10)
        {
            Destroy(gameObject);
        }
    }
}
