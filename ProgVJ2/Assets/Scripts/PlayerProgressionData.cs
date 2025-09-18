using UnityEngine;

[CreateAssetMenu(fileName = "PlayerProgressionData", menuName = "ScriptableObjects/PlayerProgressionData", order = 1)]
public class PlayerProgressionData : ScriptableObject
{
    public int CurrentLevel;
    public int CurrentExperience;
    public int ExperienceToNextLevel;
}
