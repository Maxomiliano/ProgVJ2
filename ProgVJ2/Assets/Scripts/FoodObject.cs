using UnityEngine;

public class FoodObject : CellObject
{
    [SerializeField] private AudioClip _foodSound;

    private Vector3 _soundClipPos;
    public int AmountGranted = 10;

    private void Start()
    {
        _soundClipPos = new Vector3(4, 4, -10);
    }

    public override void PlayerEntered()
    {
        AudioSource.PlayClipAtPoint(_foodSound, _soundClipPos);
        Destroy(gameObject);
        GameManager.Instance.ChangeFood(AmountGranted);
    }
}
