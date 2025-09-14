using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    public float levelTime = 60f;
    public GameObject completeButton;
    public bool useTimer = true;
    
    [Header("UI References")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI levelText;
    public GameObject levelCompletePanel;
    public TextMeshProUGUI finalScoreText;
    public Button nextLevelButton;
    public Button restartButton;
    public Button watchAdButton;
    
    [Header("STARS")]
    public Image star1, star2, star3;
    
    [Header("Star Sprites")]
    public Sprite starFilledSprite, starEmptySprite;
    
    private float currentTime;
    private bool levelActive = true;
    private int currentLevel = 1;
    private Image[] stars;
    private int totalItemsCache;
    private Coroutine starsAnimationCoroutine;

    void Start()
    {   
        InitializeLevel();
        SetupUI();
        DOTween.Init(true, true, LogBehaviour.Default); // Initialize DOTween
        
    }

    void InitializeLevel()
    {
        currentTime = levelTime;
        stars = new Image[] { star1, star2, star3 };
        totalItemsCache = GameManager.Instance.GetTotalItems();
    }

    void SetupUI()
    {
        if (levelText) levelText.text = $"Level {currentLevel}";
        if (levelCompletePanel) levelCompletePanel.SetActive(false);

        // Setup button listeners
        if (completeButton) completeButton.GetComponent<Button>().onClick.AddListener(CompleteLevel);
        if (nextLevelButton) nextLevelButton.GetComponent<Button>().onClick.AddListener(NextLevel);
        if (restartButton) restartButton.GetComponent<Button>().onClick.AddListener(RestartLevel);
        if (watchAdButton) watchAdButton.GetComponent<Button>().onClick.AddListener(WatchAdForDoubleReward);
        
        
    }

    void Update()
    {
        if (!levelActive || !useTimer) return;
        
        currentTime -= Time.deltaTime;
        
        // Update timer every 3 frames for performance
        if (Time.frameCount % 3 == 0) UpdateTimerUI();
        
        if (currentTime <= 0) CompleteLevel();
    }
    
    void UpdateTimerUI()
    {
        if (!timerText) return;
        
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void CompleteLevel()
    {
        if (!levelActive) return;
        
        levelActive = false;
        int finalScore = GameManager.Instance.GetCurrentScore();
        int starsEarned = CalculateStars(finalScore);
        
        ShowLevelComplete(finalScore, starsEarned);
    }
    
    int CalculateStars(int score)
    {
        float percentage = (float)score / totalItemsCache;
        if (percentage >= 0.9f) return 3;
        if (percentage >= 0.7f) return 2;
        if (percentage >= 0.4f) return 1;
        return 0;
    }
    
    void ShowLevelComplete(int score, int starsEarned)
    {
        if (!levelCompletePanel) return;
        
        levelCompletePanel.SetActive(true);
        if (finalScoreText) finalScoreText.text = $"Score: {score}/{totalItemsCache}";
        
        // Stop previous animation and start new one
        if (starsAnimationCoroutine != null) StopCoroutine(starsAnimationCoroutine);
        starsAnimationCoroutine = StartCoroutine(AnimateStars(starsEarned));
        
        if (watchAdButton) watchAdButton.gameObject.SetActive(starsEarned < 3);
    }
    
    IEnumerator AnimateStars(int earnedStars)
    {
        // Reset all stars to empty state
        foreach (Image star in stars)
        {
            if (star) star.sprite = starEmptySprite;
        }
        
        yield return new WaitForSeconds(0.3f);
        
        // Animate earned stars
        for (int i = 0; i < earnedStars && i < stars.Length; i++)
        {
            if (stars[i])
            {
                stars[i].sprite = starFilledSprite;
                
                // Combined animation sequence
                Sequence starAnim = DOTween.Sequence();
                starAnim.Append(stars[i].DOColor(Color.yellow, 0.3f));
                starAnim.Join(stars[i].transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 2, 0.5f));
                starAnim.Join(stars[i].DOFade(1f, 0.1f));
                
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
    
    public void NextLevel()
    {
        Debug.Log("Coming Soon :D");
        ReloadScene();
    }
    
    public void RestartLevel() => ReloadScene();
    
    public void ReloadScene()
    {
        DOTween.KillAll();
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    
    void WatchAdForDoubleReward()
    {
        int doubledScore = GameManager.Instance.GetCurrentScore() * 2;
        GameManager.Instance.SetScore(doubledScore);
        
        if (finalScoreText) finalScoreText.text = $"Score: {doubledScore}/{totalItemsCache} (DOUBLED!)";
        if (starsAnimationCoroutine != null) StopCoroutine(starsAnimationCoroutine);
        
        starsAnimationCoroutine = StartCoroutine(AnimateStars(3));
        if (watchAdButton) watchAdButton.gameObject.SetActive(false);
    }
    
    void OnDestroy()
    {
        DOTween.KillAll();
        if (starsAnimationCoroutine != null) StopCoroutine(starsAnimationCoroutine);
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) PlayerPrefs.Save(); // Auto-save on pause
    }
}