using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSystem : MonoBehaviour
{
    [SerializeField] public ItemData itemData;
    [SerializeField] public ColorData colorData;

    [Header("Item Spawn Settings")]
    public int spawnCount = 3;

    public List<Transform> spawnPoints;

    [Header("Item Prefab")]
    public Item itemPrefab;
    public Transform itemParent;
    public List<Item> spawnedItems = new List<Item>();


    private void Start()
    {
        SpawnItems();
    }
    public void SpawnItems()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            GetRandomDataItem(spawnPoints[i].position);
        }
    }
    public void GetRandomDataItem(Vector2 pos)
    {
        ItemDetail itemDetail = itemData.GetRandom();
        ColorDetail colorDetail = colorData.GetRandom();

        Item newItem = PoolingManager.Spawn(itemPrefab, pos, Quaternion.identity, itemParent);
        newItem.SetupBlock(itemDetail, colorDetail, itemData.ItemSize , pos);
        newItem.gameObject.SetActive(true);
    }

}
