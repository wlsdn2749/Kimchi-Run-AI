using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;


public enum Category
{
    Enemy = 0,
    FOod = 1,
    Golden = 2,
}

public class PlayerAgent : Agent
{

    private float maxOffsetX = 9f;
    private float minOffsetX = -9f;
    private float maxOffsetY = 0f;
    private float minOffsetY = -3.9f;
    private const int k_NoAction = 0;
    private const int k_Jump = 1;

    [Header("Settings")] 
    public float JumpForce = 10f;
    public bool isGrounded = true;
    private ContactFilter2D filter;
    private bool isInvincible = false;


    [Header("Reference")]
    private Rigidbody2D rb2d;
    private Collider2D _playerCollider;
    public UIDebugOverlay UIDebugOverlay;
    public EnvController EnvController;

    [Header("MLAgents")] private Collider2D[] results = new Collider2D[6];

    public float OverlapBoxRangeX;
    public float OverlapBoxRangeY;
    public int hitCount = 0;
    private Transform parent;
    private Vector3 playerInitPosition;
    private List<Vector2> obstaclePositions;
    private Transform nearestObstacleTransform;

    private BufferSensorComponent _bufferSensor;
    protected override void Awake()
    {
        base.Awake();
        rb2d = GetComponent<Rigidbody2D>();
        _playerCollider = GetComponent<Collider2D>();
        _bufferSensor = GetComponent<BufferSensorComponent>();
        filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Observable"));
        filter.useTriggers = true;
        filter.useLayerMask = true;
        
        
        // prefab의 절대 위치를 더해줄 필요가 있음, 그래야 Normalization이 정상적으로 동작
        parent = transform.parent;
        maxOffsetX += parent.position.x;
        minOffsetX += parent.position.x;
        maxOffsetY += parent.position.y;
        minOffsetY += parent.position.y;

        playerInitPosition = new Vector3(parent.position.x - 6.12f, parent.position.y - 3.08f, -10);
    }
    
    public override void OnEpisodeBegin()
    {
        EnvController.Reset();
    }

    public float NormalizeX(float value)
    {
        float normalizedValue = (value - minOffsetX) / (maxOffsetX - minOffsetX);
        return Mathf.Round(normalizedValue * 1000f) / 1000f; // Round to 2 decimal places
    }

    public float NormalizeY(float value)
    {
        float normalizedValue = (value - minOffsetY) / (maxOffsetY - minOffsetY);
        return Mathf.Round(normalizedValue * 1000f) / 1000f; // Round to 3 decimal places
    }
    
    public float Normalize(float value, float minValue, float maxValue)
    {
        float normalizedValue = (value - minValue) / (maxValue - minValue);
        return Mathf.Round(normalizedValue * 1000f) / 1000f; // Round to 3 decimal places
    }
    private float[] OneHotEncode(int categoryIndex, int totalCategories)
    {
        float[] oneHotVector = new float[totalCategories];
        oneHotVector[categoryIndex] = 1f;
        return oneHotVector;
        
    }
    
    
    // private List<List<float>> ProcessOverlapBoxResults(int hitCount, Collider2D[] results)
    // {
    //     if (hitCount == 0 || results == null || results.Length == 0) return null;
    //     
    //     List<Vector2> positions = new List<Vector2>();
    //     List<string> tags = new List<string>();
    //     String[] uniqueTags = {"Enemy", "Food", "Golden"};
    //     
    //     foreach(var result in results)
    //     {
    //         if (result == null) continue;
    //         positions.Add(result.transform.position);
    //         tags.Add(result.tag);
    //     }
    //
    //     List<Vector2> normalizedPositions = positions.Select(pos =>
    //         new Vector2(
    //             NormalizeX(pos.x), 
    //             NormalizeY(pos.y)
    //         )
    //     ).ToList();
    //
    //     var sortedIndices = normalizedPositions
    //         .Select((pos, index) => new { pos, index })
    //         .OrderBy(item => item.pos.x)
    //         .ThenBy(item => item.pos.y)
    //         .Select(item => item.index)
    //         .ToList();
    //
    //     // List<float> finalResult = new List<float>();
    //     List<List<float>> finalResult = new List<List<float>>();
    //     foreach (var index in sortedIndices)
    //     {
    //         List<float> tempResult = new List<float>();
    //         int tagIndex = Array.IndexOf(uniqueTags, tags[index]);
    //
    //         float[] oneHotVector = OneHotEncode(tagIndex, uniqueTags.Length);
    //         
    //         tempResult.AddRange(oneHotVector);
    //         tempResult.Add(normalizedPositions[index].x);
    //         tempResult.Add(normalizedPositions[index].y);
    //         finalResult.Add(tempResult);
    //     }
    //     
    //     while (finalResult.Count < 6)
    //     {
    //         finalResult.Add(new List<float> { 0f, 0f, 0f, 0f, 0f });
    //     }
    //
    //     return finalResult;
    // }
    // public override void CollectObservations(VectorSensor sensor)
    // {
    //     
    //     // Player 어차피 점프해있을때는 측정을 안하기에, 이게 필요한가? 계속 1임
    //     // sensor.AddObservation(isGrounded ? 1 : 0);
    //     
    //     // Game Speed is need max:20 min:8
    //     sensor.AddObservation(Normalize(EnvController.CalculateGameSpeed(), EnvController.minSpeed, EnvController.maxSpeed));
    //     
    //     // Enemy, Food, Gold
    //     hitCount = Physics2D.OverlapBox(
    //         (Vector2)OverlapBox.transform.position,
    //         new Vector2(OverlapBoxRangeX, OverlapBoxRangeY),
    //         0,
    //         filter,
    //         results
    //     );
    //     
    //     List<List<float>> finalResult = ProcessOverlapBoxResults(hitCount, results);
    //     if (finalResult == null)
    //     {
    //         finalResult = new List<List<float>>();
    //
    //         for (int i = 0; i < 6; i++)
    //         {
    //             // Create a new list with 5 default values (0.0f)
    //             List<float> emptyList = new List<float>(new float[5]);
    //             finalResult.Add(emptyList);
    //         }
    //     }
    //     foreach (var result in finalResult)
    //     {
    //         float[] resultObservations = result?.ToArray() ?? new float[5];
    //         // _bufferSensor.AppendObservation(resultObservations);
    //         sensor.AddObservation(resultObservations);
    //     }
    //
    //
    //
    //
    // }

    
    // private Dictionary<string, List<float>> ProcessOverlapBoxResults(int hitCount, Collider2D[] results)
    // {
    //     if (hitCount == 0 || results == null || results.Length == 0) 
    //         return new Dictionary<string, List<float>>(); // 빈 딕셔너리 반환
    //
    //     List<Vector2> positions = new List<Vector2>();
    //     List<string> tags = new List<string>();
    //     String[] uniqueTags = { "Enemy", "Food", "Golden" };
    //
    //     foreach (var result in results)
    //     {
    //         if (result == null) continue;
    //         positions.Add(result.transform.position);
    //         tags.Add(result.tag);
    //     }
    //
    //     // Normalize positions
    //     List<Vector2> normalizedPositions = positions.Select(pos =>
    //         new Vector2(
    //             NormalizeX(pos.x),
    //             NormalizeY(pos.y)
    //         )
    //     ).ToList();
    //
    //     // Group by category
    //     Dictionary<string, List<int>> categoryIndices = new Dictionary<string, List<int>>();
    //     foreach (string tag in uniqueTags)
    //     {
    //         categoryIndices[tag] = new List<int>();
    //     }
    //     for (int i = 0; i < tags.Count; i++)
    //     {
    //         if (categoryIndices.ContainsKey(tags[i]))
    //         {
    //             categoryIndices[tags[i]].Add(i);
    //         }
    //     }
    //
    //     // Prepare result: Extract the closest one for each category
    //     Dictionary<string, List<float>> finalResult = new Dictionary<string, List<float>>();
    //     foreach (string tag in uniqueTags)
    //     {
    //         if (categoryIndices[tag].Count > 0)
    //         {
    //             int closestIndex = categoryIndices[tag]
    //                 .OrderBy(index => normalizedPositions[index].x)
    //                 .First();
    //
    //             List<float> tempResult = new List<float>();
    //             int tagIndex = Array.IndexOf(uniqueTags, tag);
    //             float[] oneHotVector = OneHotEncode(tagIndex, uniqueTags.Length);
    //
    //             tempResult.AddRange(oneHotVector);
    //             tempResult.Add(1 - normalizedPositions[closestIndex].x); // 입력할때 1에 가까울 수록 큰값으로 입력
    //             tempResult.Add(normalizedPositions[closestIndex].y);
    //             finalResult[tag] = tempResult;
    //         }
    //         else
    //         {
    //             // Add padding if category is missing
    //             finalResult[tag] = new List<float> { 0f, 0f, 0f, 0f, 0f };
    //         }
    //     }
    //
    //     return finalResult;
    // }

    
    private Transform GetNearestObstacleTransform()
    {
        // EnvController에서 Enemy 태그를 가진 자식 객체들을 찾습니다.
        Transform env = EnvController.gameObject.transform;
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
        sensor.AddObservation(Normalize(EnvController.CalculateGameSpeed(), EnvController.minSpeed, EnvController.maxSpeed));
        
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
        

        // 모종의 이유로 Player가 바깥으로 나가면 HP를 0으로 만들어서 바로 재시작하도록

        if (Mathf.Abs(transform.localPosition.y) > 10 || Mathf.Abs(transform.localPosition.x) > 15)
        {
            EnvController.Lives = 0;
        }
        
        if (isGrounded)
        {
            RequestDecision();
        }


    }

    private void Update()
    {
        nearestObstacleTransform = GetNearestObstacleTransform();
        if (nearestObstacleTransform && !isGrounded && transform.position.y > nearestObstacleTransform.position.y + 2.2 ) // 점프 해있으면서 + 착지했을때 부딫히지 않도록 상수값
        {
            // Debug.Log("is jump");
            if (Mathf.Abs(transform.position.x - nearestObstacleTransform.position.x) < 0.5f) // 장애물을 뛰어넘었을 경우
            {
                MLObstacle mlObstacle = nearestObstacleTransform.GetComponent<MLObstacle>();
                if(!mlObstacle.isDodged)
                {
                    SafeAddReward("Dodge", 1.0f);
                    mlObstacle.isDodged = true;
                }
            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = Input.GetKey(KeyCode.Space) ? k_Jump : k_NoAction;
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
        
        EnvController.totalReward += arg;
        AddReward(arg);
        
        
        if(isDebug)
            UIDebugOverlay.LogChange($"{rewDesc}: {arg.ToString("n4") + "\n"}");
#endif
    }

    public void KillPlayer()
    {
        _playerCollider.enabled = false;
    }

    public void ResetPlayer()
    {
        _playerCollider.enabled = true;
        isGrounded = true;
        transform.rotation = quaternion.identity;
        transform.position = playerInitPosition;
    }

    void Hit()
    {
        EnvController.Lives -= 1;

    }
    void Heal()
    {
        EnvController.Lives = Mathf.Min(3, EnvController.Lives + 1);
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
