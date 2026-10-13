using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Item Data")]
public class ItemData : ScriptableObject
{
    public int ItemSize;
    public List<ItemDetail> itemDetails;


    public ItemDetail GetRandom()
    {
        int randomIndex = Random.Range(0, itemDetails.Count);
        return itemDetails[randomIndex];
    }
}

[System.Serializable]
public class ItemDetail
{
    public List<Vector2Int> shape;
}
