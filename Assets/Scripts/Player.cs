using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Settings")] 
    public float JumpForce = 15f;
    private bool isGrounded = true;
    private string _animatorState = "state";
    
    private bool isInvincible = false;
    
    
    [Header("Reference")] 
    [HideInInspector]
    private Rigidbody2D rb2d;
    private Animator _animator;
    private Collider2D _playerCollider;
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _playerCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb2d.AddForceY(JumpForce, ForceMode2D.Impulse);
            isGrounded = false;
            _animator.SetInteger(_animatorState, 1);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Platform")
        {
            if (!isGrounded)
            {
                _animator.SetInteger(_animatorState, 2);
            }
            isGrounded = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Enemy")
        {
            if (!isInvincible)
            {
                Destroy(collider.gameObject);
                Hit();
            }

            
        }
        else if(collider.gameObject.tag == "Food")
        {
            Destroy(collider.gameObject);
            Heal();
        }
        else if (collider.gameObject.tag == "Golden")
        {
            Destroy(collider.gameObject);
            StartInvincible();
        }
    }

    public void KillPlayer()
    {
        _playerCollider.enabled = false;
        _animator.enabled = false;
        rb2d.AddForceY(JumpForce, ForceMode2D.Impulse);
    }
    void Hit()
    {
        GameManagerVSAI.Instance.Lives -= 1;

    }

    void Heal()
    {
        GameManagerVSAI.Instance.Lives = Mathf.Min(3, GameManagerVSAI.Instance.Lives + 1);
    }

    void StartInvincible()
    {
        isInvincible = true;
        Invoke(nameof(StopInvincible), 5f);
    }

    void StopInvincible()
    {
        isInvincible = false;
    }
}
