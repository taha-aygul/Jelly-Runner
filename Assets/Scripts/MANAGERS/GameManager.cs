using UnityEngine;

public class GameManager : MonoBehaviour
{
    LevelManager _levelManager;
    ScoreManager _scoreManager;
    UIManager _UIManager;
    public bool IsGamePlaying { get; private set; }
    public bool IsGameFinished { get; private set; }

    public static GameManager Instance;
    private void MakeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Awake()
    {
        MakeSingleton();
    }


    void Start()
    {
        _UIManager = UIManager.Instance;
        _scoreManager = ScoreManager.Instance;
        _levelManager = LevelManager.Instance;
        StopGame();
    }
    public void StartGame()
    {
        IsGamePlaying = true;
        IsGameFinished = false;
    }
    public void StopGame()
    {
        IsGamePlaying = false;
    }
    public void RestartGame()
    {
        _levelManager.ReloadLevel();

    }
    public void LevelFailed()
    {
        _levelManager.ReloadLevel();
    }
    public void LevelSuccesful(int scoreMultiplier)
    {
        // UI Operations
       // scoreManager.GainCoin(scoreMultiplier * scoreManager.coinGainedThisRound);
        _scoreManager.GiveBonus(scoreMultiplier);
        IsGamePlaying = false;
        IsGameFinished = true;
        _UIManager.OpenLevelCompleteScreen();

    }

}
