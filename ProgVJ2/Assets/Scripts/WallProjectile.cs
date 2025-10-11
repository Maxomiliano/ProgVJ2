using UnityEngine;

public class WallProjectile : MonoBehaviour
{
    public float Speed = 5f;
    public float Lifetime = 4f;
    public int Damage = 1;

    private Vector3 _direction;
    private float _timeAlive;
    private ObjectPooler _objectPooler;

    public void Init(Vector3 direction, float speed, int damage, float lifetime, ObjectPooler pooler)
    {
        _direction = direction.normalized;
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
                GameManager.Instance.ChangeLives(-Damage); //Eventualmente esto va a ser un hitpoint que debo agregarle al personaje.
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
}
