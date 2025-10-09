using System.Collections;
using UnityEngine;

public class EnemyBiteCellObject : CellObject
{
    public int Damage = 3;
    public float lifeTime = 3f;
    
    private bool _hasDamagedPlayer = false;

    public override void Init(Vector2Int cell)
    {
        base.Init(cell);

        PlayerController player = GameManager.Instance.PlayerController;
        if (player.Cell == cell && !_hasDamagedPlayer)
        {
            _hasDamagedPlayer = true;
            GameManager.Instance.ChangeFood(-Damage);
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
        if (!_hasDamagedPlayer)
        {
            _hasDamagedPlayer = true;
            GameManager.Instance.ChangeFood(-Damage);
        }

        GameManager.Instance.BoardManager.GetCellData(_cell).ContainedObject = null;
        Destroy(gameObject);
    }
}
