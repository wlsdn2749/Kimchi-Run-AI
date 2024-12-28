using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;

public class UIDebugOverlay : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public TMP_Text debugText;
    public TMP_Text rewardDebugText;
    public PlayerAgent PlayerAgent;
    public EnvController EnvController;
    public bool isDebugActive = false;
    
    private List<string> changeLog = new List<string>(); 
    private const int maxLogEntries = 5; // Limit the number of entries displayed
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            isDebugActive = !isDebugActive;
            if (debugText != null)
                debugText.gameObject.SetActive(isDebugActive);
            
            if (rewardDebugText != null)
                rewardDebugText.gameObject.SetActive(isDebugActive);
        }
        
        if (isDebugActive && debugText != null)
        {
            ReadOnlyCollection<float> observations = PlayerAgent.GetObservations();
            debugText.text = $"FPS: {1.0f / Time.deltaTime:F2}\n" +
                             $"Time: {Time.time:F2}s\n" +
                             $"Game Speed: {EnvController.CalculateGameSpeed()}\n" +
                             $"Game Score: {EnvController.CalculateScore()}\n" +
                             $"PlayerAgent::IsGrounded: {PlayerAgent.isGrounded}\n" +
                             $"PlayerAgent::hitCount: {PlayerAgent.hitCount}\n" +
                             $"Obs:";
            
            foreach (float value in observations)
            {
                debugText.text += $"{value}";
            }
        }

        if (isDebugActive && rewardDebugText != null)
        {
            UpdateRewardText();
        }

    }
    
    public void LogChange(string message)
    {
        changeLog.Add(message);

        // Limit the number of entries in the log
        if (changeLog.Count > maxLogEntries)
        {
            changeLog.RemoveAt(0); // Remove the oldest entry
        }
    }
    
    private void UpdateRewardText()
    {
        // Display total reward and change log
        rewardDebugText.text = $"Total Reward: {EnvController.totalReward:F2}\nChanges:\n";

        for (int i = 0; i < changeLog.Count; i++)
        {
            rewardDebugText.text += $"{changeLog[i]}";
        }
    }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     if (isDebugActive)
    //     {
    //         Debug.Log("Drawing Gizmos!");
    //         Gizmos.DrawCube(OverlapBox.transform.position, new Vector3(PlayerAgent.OverlapBoxRangeX, PlayerAgent.OverlapBoxRangeY, 5));
    //     }
    // }
}