using System;
using UnityEngine;

public class PlayerProgression : MonoBehaviour
{
    public PlayerProgressionData ProgressionData;

    public void GainExperience(int exp)
    {
        ProgressionData.CurrentExperience += exp;

        GameManager.Instance.UpdateExpUI(ProgressionData.CurrentExperience, ProgressionData.ExperienceToNextLevel);

        if (ProgressionData.CurrentExperience >= ProgressionData.ExperienceToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        ProgressionData.CurrentLevel++;
        ProgressionData.CurrentExperience -= ProgressionData.ExperienceToNextLevel;
        ProgressionData.ExperienceToNextLevel += 10;

        Debug.Log("Current level: " + ProgressionData.CurrentLevel);

        GameManager.Instance.UpdateLevelUI(ProgressionData.CurrentLevel);
        GameManager.Instance.UpdateExpUI(ProgressionData.CurrentExperience, ProgressionData.ExperienceToNextLevel);
    }
}
