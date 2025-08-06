using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private int score = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddPoint()
    {
        score++;
        Debug.Log("Punkty: " + score);
    }

    public void SubstractPoint()
    {
        if (score > 0) score--;
        Debug.Log("Punkty: " + score);
    }
}

