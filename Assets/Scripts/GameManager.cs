using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int world { get; private set; }
    public int stage { get; private set; }
    private int _lives;
    private int _coins;

    public int lives
    {
        get { return _lives; }
        private set
        {
            _lives = value;
            OnLivesChanged?.Invoke(_lives);
        }
    }

    public int coins
    {
        get { return _coins; }
        private set
        {
            _coins = value;
            UpdateScore();
        }
    }

    public event Action<int> OnLivesChanged;
    private TextMeshProUGUI scoreText;
    private Image currentHealthBar;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        SceneManager.sceneLoaded += OnSceneLoaded;
        NewGame();
    }

    public void NewGame()
    {
        lives = 3;
        coins = 0;
        LoadLevel(1, 1);
    }

    public void GameOver()
    {
        NewGame();
    }

    public void LoadLevel(int world, int stage)
    {
        this.world = world;
        this.stage = stage;
        SceneManager.LoadScene($"{world}-{stage}");
    }

    public void NextLevel()
    {
        LoadLevel(world, stage + 1);
    }

    public void ResetLevel(float delay)
    {
        CancelInvoke(nameof(ResetLevel));
        Invoke(nameof(ResetLevel), delay);
    }

    public void ResetLevel()
    {
        lives--;

        if (lives <= 0)
        {
            GameOver();
        }
        else
        {
            LoadLevel(world, stage);
            UpdateHealthBar();
            UpdateScore();
        }
    }

    public void AddCoin()
    {
        coins++;
        if (coins == 100)
        {
            coins = 0;
            AddLife();
        }
        UpdateScore();
    }

    public void AddLife()
    {
        lives++;
        UpdateHealthBar();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject scoreTextObject = GameObject.Find("ScoreText");

        if (scoreTextObject != null)
        {
            scoreText = scoreTextObject.GetComponent<TextMeshProUGUI>();
            UpdateScore();
        }
        GameObject healthBarObject = GameObject.Find("HealthBar");

        if (healthBarObject != null)
        {
            currentHealthBar = healthBarObject.GetComponent<Image>();
            UpdateHealthBar();
        }
    }

    private void UpdateScore()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {coins}";
        }
    }

    private void UpdateHealthBar()
    {

        if (currentHealthBar != null)
        {
            currentHealthBar.fillAmount = (float)lives / 10;
        }
    }
}
