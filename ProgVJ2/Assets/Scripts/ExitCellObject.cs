using UnityEngine;
using UnityEngine.Tilemaps;

public class ExitCellObject : CellObject
{
    [SerializeField] private AudioClip _exitClip;

    public Tile EndTile;
    public int experienceOnExit = 20;
    private Vector3 _soundClipPos;

    public override void Init(Vector2Int coord)
    {
        base.Init(coord);
        _soundClipPos = new Vector3(4, 4, -10);
        GameManager.Instance.BoardManager.SetCellTile(coord, EndTile);
    }

    public override void PlayerEntered()
    {
        AudioSource.PlayClipAtPoint(_exitClip, _soundClipPos);
        PlayerController player = GameManager.Instance.PlayerController;
        PlayerProgression progression = player.GetComponent<PlayerProgression>();
        if (progression != null)
        {
            progression.GainExperience(experienceOnExit);
        }

        GameManager.Instance.NewLevel();
    }
}
