using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Intro,
    Playing,
    Dead 
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState State = GameState.Intro;
    [Header("References")] 
    public GameObject IntroUI;

    public GameObject DeadUI;

    public GameObject EnemySpawner;
    public GameObject FoodSpawner;
    public GameObject GoldSpawner;

    public float PlayStartTime;
    
    public int Lives = 3;

    public Player PlayerScript;
    public TMP_Text score;
    public TMP_Text highScore;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        IntroUI.SetActive(true);
        highScore.text = "High Score : " + GetHighScore();
    }

    float CalculateScore()
    {
        return Time.time - PlayStartTime; 
    }

    private void UpdateHighScore(int currentScore)
    {
        highScore.text = "High Score : " + currentScore;
        highScore.color = Color.green;
        PlayerPrefs.SetInt("highScore", currentScore);
    }

    public float CalculateGameSpeed()
    {
        if (State != GameState.Playing)
        {
            return 5f;
        }

        float speed = 8f + (0.75f * Mathf.Floor(CalculateScore() / 10f));
        return Mathf.Min(speed, 20f);
    }

    int GetHighScore()
    {
        return PlayerPrefs.GetInt("highScore");
    }
    // Update is called once per frame
    void Update()
    {
        if (State == GameState.Playing)
        {
            int currentScore = Mathf.FloorToInt(CalculateScore());
            score.text = "Score : " + currentScore;

            if (currentScore > GetHighScore())
            {
                UpdateHighScore(currentScore);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && State == GameState.Intro)
        {
            State = GameState.Playing;
            IntroUI.SetActive(false);
            EnemySpawner.SetActive(true);
            FoodSpawner.SetActive(true);
            GoldSpawner.SetActive(true);
            PlayStartTime = Time.time;
        }

        if (State == GameState.Playing && Lives == 0)
        {
            PlayerScript.KillPlayer();
            EnemySpawner.SetActive(false);
            FoodSpawner.SetActive(false);
            GoldSpawner.SetActive(false);
            DeadUI.SetActive(true);
            State = GameState.Dead;
        }

        if (State == GameState.Dead && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("Main");
        }
    }
}
