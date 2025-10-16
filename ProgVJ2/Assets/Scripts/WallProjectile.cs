using UnityEngine;

public class WallProjectile : MonoBehaviour
{
    [SerializeField] private AudioClip _rockSFX;
    [SerializeField] private ParticleSystem _stoneParticles;

    public float Speed = 5f;
    public float Lifetime = 4f;
    public int Damage = 1;

    private Vector3 _soundClipPos;
    private Vector3 _direction;
    private float _timeAlive;
    private ObjectPooler _objectPooler;

    public void Init(Vector3 direction, float speed, int damage, float lifetime, ObjectPooler pooler)
    {
        _direction = direction.normalized;
        _soundClipPos = new Vector3(4, 4, -10);
        Speed = speed;
        Damage = damage;
        Lifetime = lifetime;
        _objectPooler = pooler;
        _timeAlive = 0;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        transform.position += _direction * Speed * Time.deltaTime;

        PlayerController player = GameManager.Instance?.PlayerController;
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < 0.5f)
            {
                player.PlayerHitAnimation();
                GameManager.Instance.ChangeLives(-Damage);
                AudioSource.PlayClipAtPoint(_rockSFX, _soundClipPos);
                SpawnStoneParticles();
                ReturnToPool();
                return;
            }
        }

        _timeAlive += Time.deltaTime;
        if (_timeAlive >= Lifetime)
        {
            ReturnToPool();
        }
    }

    public void ReturnToPool()
    {
        if (_objectPooler != null)
        {
            _objectPooler.ReturnPooledObject(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void SpawnStoneParticles()
    {
        if (_stoneParticles == null) return;
        ParticleSystem ps = Instantiate(_stoneParticles, transform.position, Quaternion.identity);
    }

}
