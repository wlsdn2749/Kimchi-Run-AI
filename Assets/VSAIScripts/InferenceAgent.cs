using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;


public class InferenceAgent : Agent
{
    
    private const int k_NoAction = 0;
    private const int k_Jump = 1;
    private string _animatorState = "state";

    [Header("Settings")] 
    public float JumpForce = 10f;
    public bool isGrounded = true;
    private ContactFilter2D filter;
    private bool isInvincible = false;


    [Header("Reference")]
    private Rigidbody2D rb2d;
    private Collider2D _playerCollider;
    private Animator _animator;
    public UIDebugOverlayVSAI UIDebugOverlayVSAI;
    public EnvControllerVSAI EnvControllerVSAI;
    
    [Header("MLAgents")]

    public int hitCount = 0;
    private Transform parent;
    private Vector3 playerInitPosition;
    private List<Vector2> obstaclePositions;
    private Transform nearestObstacleTransform;
    
    protected override void Awake()
    {
        base.Awake();
        rb2d = GetComponent<Rigidbody2D>();
        _playerCollider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
        filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Observable"));
        filter.useTriggers = true;
        filter.useLayerMask = true;
        
        
        // prefab의 절대 위치를 더해줄 필요가 있음, 그래야 Normalization이 정상적으로 동작
    }
    
    public override void OnEpisodeBegin()
    {
        EnvControllerVSAI.Reset();
    }
    
    
    public float Normalize(float value, float minValue, float maxValue)
    {
        float normalizedValue = (value - minValue) / (maxValue - minValue);
        return Mathf.Round(normalizedValue * 1000f) / 1000f; // Round to 3 decimal places
    }

    private Transform GetNearestObstacleTransform()
    {
        // EnvController에서 Enemy 태그를 가진 자식 객체들을 찾습니다.
        Transform env = EnvControllerVSAI.gameObject.transform;
        Vector2 playerPosition = transform.position;
        if (env == null) return null; // 부모가 없으면 기본값 반환

        // 모든 "Enemy" 태그를 가진 자식들의 위치를 가져옵니다.
        var obstaclePositions = env.GetComponentsInChildren<Transform>()
            .Where(child => child.CompareTag("Enemy"))
            .Select(child => child.transform)
            .ToList();

        // player보다 큰 x값을 가진 장애물들만 필터링
        var validObstacles = obstaclePositions
            .Where(obstacle => obstacle.position.x > playerPosition.x) // player보다 큰 x값
            .OrderBy(obstacle => obstacle.position.x) // x값 기준으로 정렬 (가장 작은 값)
            .ToList();

        // validObstacles가 비어있지 않으면 가장 작은 x값을 가진 장애물 반환
        if (validObstacles.Any())
        {
            return validObstacles.First(); // 가장 작은 x값을 가진 장애물 반환
        }

        // validObstacles가 없으면 기본값 반환
        return null;
    }
    
    private float GetClosestNormalizedDistance(Vector2 playerPosition, Transform nearestObstacleTransform, float maxDistance)
    {
        if (nearestObstacleTransform == null)
            return 0f;

        float minDistance = Vector2.Distance(playerPosition, nearestObstacleTransform.position);

        return 1f - Mathf.Clamp(minDistance / maxDistance, 0f, 1f);
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        // Add game speed observation (normalized)
        sensor.AddObservation(Normalize(GameManagerVSAI.Instance.CalculateGameSpeed(), EnvControllerVSAI.minSpeed, EnvControllerVSAI.maxSpeed));
        
        // 플레이어 위치
        Vector2 playerPosition = transform.position;

        // 정규화된 거리 계산
        float normalizedDistance = GetClosestNormalizedDistance(playerPosition, nearestObstacleTransform, 9f);
        // Debug.Log(normalizedDistance);
        // Observation에 추가
        sensor.AddObservation(normalizedDistance);
    }
    private void FixedUpdate()
    {
        // 가장 가까운 장애물 위치 갱신
        nearestObstacleTransform = GetNearestObstacleTransform();
        
        // 모종의 이유로 Player가 바깥으로 나가면 HP를 0으로 만들어서 바로 재시작하도록

        if (Mathf.Abs(transform.localPosition.y) > 10 || Mathf.Abs(transform.localPosition.x) > 15)
        {
            EnvControllerVSAI.Lives = 0;
        }
        
        if (isGrounded && GameManagerVSAI.Instance.State == GameStateVSAI.Playing)
        {
            RequestDecision();
        }


    }
    
    public override void OnActionReceived(ActionBuffers actions)
    {
        var action = actions.DiscreteActions[0];

        switch (action)
        {
            case k_NoAction:
                SafeAddReward("timestep", 0.01f, false);
                break;
            case k_Jump:
                rb2d.linearVelocity = Vector2.zero;
                rb2d.angularVelocity = 0f;
                rb2d.AddForceY(JumpForce, ForceMode2D.Impulse);
                isGrounded = false;
                _animator.SetInteger(_animatorState, 1);
                SafeAddReward("jump", -0.1f, false);
                break;
        }
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Debug.Log("collision : " +  collision.gameObject.name );
        if (collision.gameObject.CompareTag("Platform"))
        {
            // Debug.Log("isground "  + "true");
            isGrounded = true;
            _animator.SetInteger(_animatorState, 2);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Enemy")
        {
            if (!isInvincible)
            {
                Destroy(collider.gameObject);
                SafeAddReward("Hit", -3f);
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

    private void SafeAddReward(string rewDesc, float arg, bool isDebug = true)
    {
#if UNITY_EDITOR
        
        EnvControllerVSAI.totalReward += arg;
        AddReward(arg);
        
        
        if(isDebug)
            UIDebugOverlayVSAI.LogChange($"{rewDesc}: {arg.ToString("n4") + "\n"}");
#endif
    }

    public void KillPlayer()
    {
        _playerCollider.enabled = false;
        _animator.enabled = false;
        isGrounded = false;
        rb2d.AddForceY(JumpForce, ForceMode2D.Impulse);
    }
    
    void Hit()
    {
        EnvControllerVSAI.Lives -= 1;

    }
    void Heal()
    {
        EnvControllerVSAI.Lives = Mathf.Min(3, EnvControllerVSAI.Lives + 1);
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
