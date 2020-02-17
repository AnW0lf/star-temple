﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private GameObject itemPrefab;
    public RectTransform container;

    [Header("Moving")]
    public float hidePos = -650f;
    public float showPos = 0f;
    [Range(0.1f, 10f)]
    public float delay = 0.5f;

    public List<ItemController> items { get; private set; }
    public static Inventory Instance { get; private set; } = null;
    private RectTransform self;

    private Coroutine moveTo = null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        items = new List<ItemController>();
        self = GetComponent<RectTransform>();
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

    public void Show()
    {
        if (moveTo != null) StopCoroutine(moveTo);
        moveTo = StartCoroutine(MoveTo(showPos));
    }

    public void Hide()
    {
        if (moveTo != null) StopCoroutine(moveTo);
        moveTo = StartCoroutine(MoveTo(hidePos));
    }

    private IEnumerator MoveTo(float pos)
    {
        float speed = Mathf.Abs(hidePos - showPos) / delay;
        while(Mathf.Abs(self.anchoredPosition.y - pos) > 2f * speed * Time.deltaTime)
        {
            self.anchoredPosition += Vector2.up * speed * Time.deltaTime * Mathf.Sign(pos - self.anchoredPosition.y);
            yield return null;
        }
        self.anchoredPosition = new Vector2(self.anchoredPosition.x, pos);
        moveTo = null;
    }
}
