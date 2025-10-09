using UnityEngine;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour
{
    public GameObject PooledPrefab;
    public int PoolSize = 10;

    private List<GameObject> _pool;

    private void Start()
    {
        _pool = new List<GameObject>(PoolSize);
        for (int i = 0; i < PoolSize; i++)
        {
            GameObject go = Instantiate(PooledPrefab, transform);
            go.SetActive(false);
            _pool.Add(go);
        }
    }

    public GameObject GetPooledObject()
    {
        foreach (GameObject go in _pool)
        {
            if(!go.activeInHierarchy) return go;
        }
        return null;
    }
}
