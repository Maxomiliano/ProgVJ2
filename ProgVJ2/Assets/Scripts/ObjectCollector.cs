using UnityEngine;
using System.Collections.Generic;

public class ObjectCollector : MonoBehaviour
{
    public List<GameObject> Collectibles;
    private int collectedCount;

    private PlayerProgression progression;

    private void Start()
    {
        collectedCount = 0;
        progression = GetComponent<PlayerProgression>();
    }

    public void CollectObject(CollectableCellObject obj)
    {
        collectedCount++;
        Debug.Log("Objetos recolectados: " + collectedCount);
    }
}
