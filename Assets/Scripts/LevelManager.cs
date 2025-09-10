using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections;

// DEBUGOWANE I NAPRAWIONE: Stars System
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
    
    [Header("STARS DEBUG - Sprawdź te pola!")]
    public Image star1;  // Przeciągnij każdą gwiazdkę osobno
    public Image star2;  // Łatwiej debugować
    public Image star3;  // Nie array - może być null
    
    [Header("Star Sprites - WYMAGANE!")]
    public Sprite starFilledSprite;  // Sprite wypełnionej gwiazdki
    public Sprite starEmptySprite;   // Sprite pustej gwiazdki
    
    private float currentTime;
    private bool levelActive = true;
    private int currentLevel = 1;
    private bool timerWarningActive = false;
    private Tween timerPulseTween;
    
    void Start()
    {
        // DEBUG: Sprawdź czy wszystko jest podłączone
        DebugStarsSetup();
        
        currentTime = levelTime;
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        
        if (levelText) levelText.text = "Level " + currentLevel;
        
        // Setup buttons
        if (completeButton) completeButton.GetComponent<Button>().onClick.AddListener(CompleteLevel);
        if (nextLevelButton) nextLevelButton.onClick.AddListener(NextLevel);
        if (restartButton) restartButton.onClick.AddListener(RestartLevel);
        if (watchAdButton) watchAdButton.onClick.AddListener(WatchAdForDoubleReward);
        
        if (levelCompletePanel) levelCompletePanel.SetActive(false);
        
        // TEST: Pokaż panel od razu dla testowania gwiazdek
        if (Input.GetKey(KeyCode.T))
        {
            Invoke("TestStars", 2f);
        }
    }
    
    // DEBUG: Sprawdź setup
    void DebugStarsSetup()
    {
        Debug.Log("=== STARS DEBUG ===");
        Debug.Log($"Star1: {(star1 != null ? "OK" : "NULL!")}");
        Debug.Log($"Star2: {(star2 != null ? "OK" : "NULL!")}");
        Debug.Log($"Star3: {(star3 != null ? "OK" : "NULL!")}");
        Debug.Log($"Filled Sprite: {(starFilledSprite != null ? "OK" : "NULL!")}");
        Debug.Log($"Empty Sprite: {(starEmptySprite != null ? "OK" : "NULL!")}");
        
        // Jeśli brak sprite, użyj built-in
        if (starFilledSprite == null)
        {
            starFilledSprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
            Debug.LogWarning("Using built-in sprite for filled star");
        }
        
        if (starEmptySprite == null)
        {
            starEmptySprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/Background.psd");
            Debug.LogWarning("Using built-in sprite for empty star");
        }
    }
    
    // TEST: Funkcja testowa - call w Start() lub przez button
    [ContextMenu("Test Stars")]
    void TestStars()
    {
        Debug.Log("Testing stars animation...");
        levelCompletePanel?.SetActive(true);
        StartCoroutine(AnimateStarsFixed(2)); // Test z 2 gwiazdkami
    }

    void Update()
    {
        if (!levelActive) return;
        
        // DEBUG: Test gwiazdek klawiszem T
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestStars();
        }
        
        if (useTimer && currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerUI();
            
            if (currentTime <= 0)
            {
                CompleteLevel();
            }
        }
    }
    
    void UpdateTimerUI()
    {
        if (timerText == null) return;
        
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void CompleteLevel()
    {
        if (!levelActive) return;
        
        levelActive = false;
        int finalScore = GameManager.Instance.GetCurrentScore();
        int stars = CalculateStars(finalScore);
        
        Debug.Log($"Level complete! Score: {finalScore}, Stars: {stars}");
        ShowLevelComplete(finalScore, stars);
    }
    
    int CalculateStars(int score)
    {
        int totalItems = GameManager.Instance.GetTotalItems();
        if (totalItems == 0) return 1; // Default 1 star if no items
        
        float percentage = (float)score / totalItems;
        
        Debug.Log($"Score calculation: {score}/{totalItems} = {percentage:P}");
        
        if (percentage >= 0.9f) return 3;
        if (percentage >= 0.7f) return 2;
        if (percentage >= 0.4f) return 1;
        return 0;
    }
    
    void ShowLevelComplete(int score, int stars)
    {
        Debug.Log($"ShowLevelComplete called with {stars} stars");
        
        if (levelCompletePanel == null)
        {
            Debug.LogError("levelCompletePanel is NULL!");
            return;
        }
        
        levelCompletePanel.SetActive(true);
        
        if (finalScoreText != null)
        {
            finalScoreText.text = $"Score: {score}/{GameManager.Instance.GetTotalItems()}";
        }
        
        // Start stars animation
        StartCoroutine(AnimateStarsFixed(stars));
        
        if (watchAdButton != null) 
        {
            watchAdButton.gameObject.SetActive(stars < 3);
        }
    }
    
    // NAPRAWIONA: Stars animation bez array
    IEnumerator AnimateStarsFixed(int earnedStars)
    {
        Debug.Log($"AnimateStarsFixed started with {earnedStars} stars");
        
        // Get all star references in order
        Image[] stars = { star1, star2, star3 };
        
        // First, reset all stars to empty
        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] != null)
            {
                Debug.Log($"Setting star {i} to empty");
                stars[i].sprite = starEmptySprite;
                stars[i].color = Color.gray;
                stars[i].transform.localScale = Vector3.one * 0.8f;
            }
            else
            {
                Debug.LogError($"Star {i} is NULL! Check Inspector!");
            }
        }
        
        yield return new WaitForSeconds(0.5f);
        
        // Animate earned stars one by one
        for (int i = 0; i < earnedStars && i < stars.Length; i++)
        {
            if (stars[i] != null)
            {
                Debug.Log($"Animating star {i}");
                
                // Change to filled
                stars[i].sprite = starFilledSprite;
                stars[i].color = Color.yellow;
                
                // Pop animation
                Sequence starAnim = DOTween.Sequence();
                starAnim.Append(stars[i].transform.DOScale(Vector3.one * 1.3f, 0.2f));
                starAnim.Append(stars[i].transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBounce));
                
                // Color flash
                stars[i].DOColor(Color.white, 0.1f).SetLoops(2, LoopType.Yoyo);
                
                yield return new WaitForSeconds(0.3f);
            }
        }
        
        Debug.Log("Stars animation completed");
    }
    
    void NextLevel()
    {
        currentLevel++;
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    
    void RestartLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    
    void WatchAdForDoubleReward()
    {
        Debug.Log("Watch ad for double reward!");
        int doubledScore = GameManager.Instance.GetCurrentScore() * 2;
        GameManager.Instance.SetScore(doubledScore);
        
        if (finalScoreText != null)
        {
            finalScoreText.text = $"Score: {doubledScore}/{GameManager.Instance.GetTotalItems()} (DOUBLED!)";
        }
        
        StartCoroutine(AnimateStarsFixed(3));
        
        if (watchAdButton != null)
        {
            watchAdButton.gameObject.SetActive(false);
        }
    }
    
    void OnDestroy()
    {
        timerPulseTween?.Kill();
        DOTween.Kill(this);
    }
}

// HELPER: Simple Star Creator - jeśli nie chcesz ręcznie robić
[System.Serializable]
public class StarSystemHelper : MonoBehaviour
{
    [Header("Auto-Create Stars")]
    public Transform starsContainer;
    public GameObject starPrefab;
    
    [ContextMenu("Create 3 Stars")]
    void CreateStars()
    {
        if (starsContainer == null)
        {
            Debug.LogError("Stars container is null!");
            return;
        }
        
        // Clear existing
        for (int i = starsContainer.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(starsContainer.GetChild(i).gameObject);
        }
        
        // Create 3 new stars
        for (int i = 0; i < 3; i++)
        {
            GameObject star = new GameObject($"Star{i + 1}");
            star.transform.SetParent(starsContainer);
            star.transform.localScale = Vector3.one;
            
            Image starImage = star.AddComponent<Image>();
            starImage.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
            starImage.color = Color.gray;
            
            RectTransform rect = star.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(60, 60);
        }
        
        Debug.Log("Created 3 stars automatically!");
    }
}