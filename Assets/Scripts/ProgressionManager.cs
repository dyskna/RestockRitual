using UnityEngine;
public class ProgressionManager : MonoBehaviour
{
    [Header("Unlockables")]
    public Sprite[] unlockedSprites; // Nowe skiny za gwiazdki
    public GameObject[] decorativeElements; // Dekoracje za achievements

    void Start()
    {
        int totalStars = GetTotalStars();
        UnlockContent(totalStars);
    }

    int GetTotalStars()
    {
        int total = 0;
        for (int i = 1; i <= PlayerPrefs.GetInt("CurrentLevel", 1); i++)
        {
            // Simplified: assume 3 stars if level completed
            if (PlayerPrefs.GetInt($"Level_{i}_BestScore", 0) > 0)
            {
                total += 3; // Could be more sophisticated
            }
        }
        return total;
    }

    void UnlockContent(int stars)
    {
        // Unlock skins every 10 stars (monetization alternative)
        for (int i = 0; i < unlockedSprites.Length; i++)
        {
            if (stars >= (i + 1) * 10)
            {
                // Unlock skin i
                PlayerPrefs.SetInt($"Skin_{i}_Unlocked", 1);
            }
        }
    }
}