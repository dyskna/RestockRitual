using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    private int score = 0;
    private int totalItems = 0;

    void Awake()
    {
        Instance = this;
        totalItems = GameObject.FindGameObjectsWithTag("DraggableItem").Length;
    }

    public void AddPoint()
    {
        score++;
    }
    
    public void SubstractPoint()
    {
        score = Mathf.Max(0, score - 1);
    }
    
    // NOWE: Gettery dla LevelManager
    public int GetCurrentScore() { return score; }
    public int GetTotalItems() { return totalItems; }
    public void SetScore(int newScore) { score = newScore; }
    
    // NOWE: Progress tracking dla retention
    public float GetProgressPercentage()
    {
        return totalItems > 0 ? (float)score / totalItems : 0f;
    }
}

