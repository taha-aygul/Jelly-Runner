using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    [SerializeField] private GameObject tutorial , levelCompleteScreen;
    [SerializeField] private TextMeshProUGUI coinText, levelText;
    ScoreManager scoreManager;
    public static UIManager Instance;

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
    private void Awake()
    {
        MakeSingleton();
    }
    void Start()
    {
        scoreManager = ScoreManager.Instance;
        UpdateCoinText();
        UpdateLevelText();
    }

    public void CloseTutorial()
    {
        tutorial.SetActive(false);
    }
    public void UpdateCoinText()            
    {
        coinText.text = scoreManager.CurrentCoin.ToString();
        // DOTWEEN SHAKE KULLANABILIRSIN
    }

    public void UpdateLevelText()
    {
        levelText.text = SceneManager.GetActiveScene().name;
    }

    public void OpenLevelCompleteScreen()
    {
        levelCompleteScreen.SetActive(true);
    }





}
