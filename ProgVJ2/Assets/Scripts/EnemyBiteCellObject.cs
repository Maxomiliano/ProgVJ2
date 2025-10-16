using System.Collections;
using UnityEngine;

public class EnemyBiteCellObject : CellObject
{
    [SerializeField] private AudioClip _biteSFX;

    public int Damage = 3;
    public float lifeTime = 3f;

    private bool _hasDamagedPlayer = false;
    private Vector3 _soundClipPos;

    public override void Init(Vector2Int cell)
    {
        base.Init(cell);
        _soundClipPos = new Vector3(4, 4, -10);

        PlayerController player = GameManager.Instance.PlayerController;
        if (player.Cell == cell && !_hasDamagedPlayer)
        {
            _hasDamagedPlayer = true;
            GameManager.Instance.ChangeFood(-Damage);
            //AudioSource.PlayClipAtPoint(_biteSFX, _soundClipPos);
        }

        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(lifeTime);

        GameManager.Instance.BoardManager.GetCellData(_cell).ContainedObject = null;
        Destroy(gameObject);
    }

    public override void PlayerEntered()
    {
        PlayerController player = GameManager.Instance.PlayerController;
        if (!_hasDamagedPlayer)
        {
            _hasDamagedPlayer = true;
            GameManager.Instance.ChangeFood(-Damage);
            player.PlayerHitAnimation();
            AudioSource.PlayClipAtPoint(_biteSFX, _soundClipPos);
        }

        GameManager.Instance.BoardManager.GetCellData(_cell).ContainedObject = null;
        Destroy(gameObject);
    }
}
