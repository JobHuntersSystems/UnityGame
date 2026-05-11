using UnityEngine;
using System;
using System.Collections;

public class PlayerXP : MonoBehaviour
{
    [Header("niveles")]
    public int baseXP = 10;
    public int currentLevel = 1;
    public int currentXP = 0;
    public int totalXP = 0;
    public int XPToNextLevel;
    
    [Range(1f, 2f)]
    public float formula = 1.5f;

    public static event Action<int> OnLevelUp;

    void Awake()
    {
        XPToNextLevel = Mathf.RoundToInt(baseXP * Mathf.Pow(currentLevel,  formula));
    }
    public void AddXP(int amount)
    {
        currentXP += amount;
        totalXP += amount;
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        if (currentXP >= XPToNextLevel)
        {
            currentXP -= XPToNextLevel;
            currentLevel++;
            XPToNextLevel = Mathf.RoundToInt(baseXP * Mathf.Pow(currentLevel, formula));
            Time.timeScale = 0f;
            OnLevelUp?.Invoke(currentLevel);
        }
    }

    public void ResumeAfterUpgrade()
    {
        StartCoroutine(ResumeDelay());
    }

    private IEnumerator ResumeDelay()
    {
        yield return new WaitForSecondsRealtime(2f);
        Time.timeScale = 1f;
        CheckLevelUp();
    }
}
