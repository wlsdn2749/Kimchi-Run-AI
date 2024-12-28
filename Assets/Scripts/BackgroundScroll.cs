using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("How fast should the text scroll?")]
    public float scrollSpeed;

    [Header("References")]
    [HideInInspector]
    public MeshRenderer meshRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        meshRenderer.material.mainTextureOffset += 
            new Vector2(scrollSpeed * GameManagerVSAI.Instance.CalculateGameSpeed() / 20 * Time.deltaTime, 0);
    }
}
