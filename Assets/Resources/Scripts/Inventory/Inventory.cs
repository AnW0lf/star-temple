using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private GameObject itemPrefab;
    public RectTransform container;
    public List<ItemController> items { get; private set; }

    public static Inventory Instance { get; private set; } = null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        items = new List<ItemController>();
    }

    public void AddItem(Item item)
    {
        ItemController existItem;
        if ((existItem = items.Find(i => i.Item.name.Equals(item.name))) != null)
        {
            existItem.ChangeCount(item.count);
        }
        else
        {
            ItemController newItem = Instantiate(itemPrefab, container.transform).GetComponent<ItemController>();
            newItem.Fill(item);
            items.Add(newItem);
        }
    }

    public void SubtractItem(Item item)
    {
        ItemController existItem;
        if ((existItem = items.Find(i => i.Item.name.Equals(item.name))) != null)
        {
            existItem.ChangeCount(item.count);
            if (existItem.Count <= 0)
            {
                items.Remove(existItem);
                Destroy(existItem.gameObject);
            }
        }
    }

    public void LoadItems(Item[] items)
    {
        foreach(Item item in items)
        {
            AddItem(item);
        }
    }
}
