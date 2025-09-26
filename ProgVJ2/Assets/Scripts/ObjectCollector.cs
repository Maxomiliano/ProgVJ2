using UnityEngine;
using System.Collections.Generic;

public class ObjectCollector : MonoBehaviour
{
    public List<GameObject> Collectibles;
    //private int collectedCount;
    /*
     
    private void Start()
    {
        collectedCount = 0;
    }
    */

    public void CollectObject(CollectableCellObject obj)
    {
        GameManager.Instance.CollectedCount++;
        GameManager.Instance.UpdateCollectableUI(GameManager.Instance.CollectedCount);
        Debug.Log("Objetos recolectados: " + GameManager.Instance.CollectedCount);
    }
}
