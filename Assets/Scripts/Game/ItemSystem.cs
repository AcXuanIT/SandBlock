using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class ItemSystem : Singleton<ItemSystem>
{
    public ItemData itemData;
    public ColorData colorData;

    [Header("Item Spawn Settings")]
    public int spawnCount = 3;
    public List<Transform> spawnPoints = new List<Transform>();

    [Header("Item Prefab")]
    public Item itemPrefab;
    public Transform itemParent;

    private readonly List<Item> spawnedItems = new List<Item>();
    private readonly Stack<Item> pool = new Stack<Item>();

    public void SpawnItems()
    {
        int n = Mathf.Min(spawnCount, spawnPoints.Count);
        for (int i = 0; i < n; i++)
            SpawnItem(spawnPoints[i].position, colorData.GetRandom());
    }

    private void SpawnItem(Vector2 center, ColorDetail color)
    {
        Item item = pool.Count > 0 ? pool.Pop() : Instantiate(itemPrefab, itemParent);
        item.gameObject.SetActive(true);
        item.SetupBlock(itemData.GetRandom(), color, itemData.ItemSize, center);
        spawnedItems.Add(item);
    }
    public void SpawnItemByColor(ColorDetail color)
    {
        OnClearAll();
        SpawnItem(spawnPoints[0].position, color);
        int n = Mathf.Min(spawnCount, spawnPoints.Count);
        for (int i = 1; i < n; i++)
            SpawnItem(spawnPoints[i].position, colorData.GetRandom());
    }

    public void OnItemPlaced(Item item)
    {
        item.gameObject.SetActive(false);
        pool.Push(item);
        spawnedItems.Remove(item);
        if (spawnedItems.Count == 0) SpawnItems();
    }

    public void OnRestartItem()
    {
        if(spawnedItems.Count == 0)
        {
            pool.Clear();
            SpawnItems();
        }
        else
        {
            foreach (Item item in spawnedItems)
            {
                item.gameObject.SetActive(false);
            }
            spawnedItems.Clear();
            pool.Clear();

            SpawnItems();
        }
    }


    public void OnClearAll()
    {
        pool.Clear();
        foreach(Item item in spawnedItems)
        {
            item.gameObject.SetActive(false);
        }
        spawnedItems.Clear();
    }
}
