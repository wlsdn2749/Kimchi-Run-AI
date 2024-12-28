using System;
using UnityEngine;

public class MLHeart : MonoBehaviour
{
    public Sprite OnHeart;
    public Sprite OffHeart;
    public SpriteRenderer SpriteRenderer;
    
    public int LiveNumber;
    public EnvController EnvController;
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (EnvController.Lives >= LiveNumber)
        {
            SpriteRenderer.sprite = OnHeart;
        }
        else
        {
            SpriteRenderer.sprite = OffHeart;
        }

    }
}
