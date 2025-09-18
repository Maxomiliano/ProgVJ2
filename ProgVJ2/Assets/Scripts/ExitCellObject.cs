using UnityEngine;
using UnityEngine.Tilemaps;

public class ExitCellObject : CellObject
{
    public Tile EndTile;
    public int experienceOnExit = 20;

    public override void Init(Vector2Int coord)
    {
        base.Init(coord);

        GameManager.Instance.BoardManager.SetCellTile(coord, EndTile);
    }

    public override void PlayerEntered()
    {
        PlayerController player = GameManager.Instance.PlayerController;
        PlayerProgression progression = player.GetComponent<PlayerProgression>();
        if (progression != null)
        {
            progression.GainExperience(experienceOnExit);
        }

        GameManager.Instance.NewLevel();
    }
}
