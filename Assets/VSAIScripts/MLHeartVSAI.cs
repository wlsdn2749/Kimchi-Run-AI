using System;
using UnityEngine;

public class MLHeartVSAI : MonoBehaviour
{
    public Sprite OnHeart;
    public Sprite OffHeart;
    public SpriteRenderer SpriteRenderer;
    
    public int LiveNumber;
    public EnvControllerVSAI EnvControllerVSAI;
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (EnvControllerVSAI.Lives >= LiveNumber)
        {
            SpriteRenderer.sprite = OnHeart;
        }
        else
        {
            SpriteRenderer.sprite = OffHeart;
        }

    }
}
