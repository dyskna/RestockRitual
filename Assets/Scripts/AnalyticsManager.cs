using UnityEngine;
public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void LogLevelComplete(int level, int score, int stars)
    {
        // Tu będą Unity Analytics lub Firebase
        Debug.Log($"Level {level} completed with {score} points, {stars} stars");

        // Track retention metrics
        PlayerPrefs.SetInt($"Level_{level}_BestScore",
            Mathf.Max(PlayerPrefs.GetInt($"Level_{level}_BestScore", 0), score));
    }

    public void LogLevelStart(int level)
    {
        PlayerPrefs.SetString("LastPlayDate", System.DateTime.Now.ToBinary().ToString());
    }
}