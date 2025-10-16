using UnityEngine;

public class CollectableCellObject : CellObject
{
    [SerializeField] private AudioClip _rescuedSFX;

    private Vector3 _soundClipPos;
    public int experienceOnCollect = 2;

    private void Start()
    {
        _soundClipPos = new Vector3( 4,4,-10);
    }

    public override void PlayerEntered()
    {
        PlayerController player = GameManager.Instance.PlayerController;
        ObjectCollector collector = player.GetComponent<ObjectCollector>();
        if (collector != null)
        {
            collector.CollectObject(this);
        }

        if (_rescuedSFX != null)
        {
            AudioSource.PlayClipAtPoint(_rescuedSFX, _soundClipPos);
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
