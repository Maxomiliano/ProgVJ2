using UnityEngine;

public class CollectableCellObject : CellObject
{
    public int experienceOnCollect = 2;

    public override void PlayerEntered()
    {
        PlayerController player = GameManager.Instance.PlayerController;
        ObjectCollector collector = player.GetComponent<ObjectCollector>();
        if (collector != null)
        {
            collector.CollectObject(this);
        }

        PlayerProgression progression = player.GetComponent<PlayerProgression>();
        if (progression != null)
        {
            progression.GainExperience(experienceOnCollect);
        }

        GameManager.Instance.BoardManager.GetCellData(_cell).ContainedObject = null;
        Destroy(gameObject);
    }
}
