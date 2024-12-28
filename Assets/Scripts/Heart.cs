using UnityEngine;

public class Heart : MonoBehaviour
{
    public Sprite OnHeart;
    public Sprite OffHeart;
    public SpriteRenderer SpriteRenderer;
    
    public int LiveNumber;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManagerVSAI.Instance.Lives >= LiveNumber)
        {
            SpriteRenderer.sprite = OnHeart;
        }
        else
        {
            SpriteRenderer.sprite = OffHeart;
        }

    }
}
