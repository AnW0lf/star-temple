﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemController : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
{
    public Text itemNameTxt, countTxt;
    [HideInInspector]
    public Item item;
    public string ItemName { get { return item.name; } }
    public int Count { get { return item.count; } private set { item.count = value; } }

    private Transform container;
    private Transform grandparent;

    private void Awake()
    {
        grandparent = transform.parent.parent;
    }

    public void Fill(Item item)
    {
        this.item = item;
        itemNameTxt.text = ItemName;
        countTxt.text = Count.ToString();
    }

    public void ChangeCount(int value)
    {
        Color oldColor = countTxt.color;
        countTxt.color = (value > 0) ? Color.green : Color.red;
        LeanTween.scale(countTxt.gameObject, Vector3.one * 1.5f, 0.3f).setDelay(0.1f)
            .setOnComplete(() => LeanTween.scale(countTxt.gameObject, Vector3.one, 0.3f)
            .setOnComplete(() =>
            {
                Count += value;
                countTxt.text = Count.ToString();
                countTxt.color = oldColor;
            }));
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        container = transform.parent;
        transform.SetParent(grandparent);
        countTxt.enabled = false;
        DragHelper.Instance.item = this;
        itemNameTxt.raycastTarget = false;

        if (item.name != "*")
            RemoveItemZoneController.Instance.Show();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(container);
        if (item.name.Equals(Helper.Star.name))
            transform.SetAsFirstSibling();
        countTxt.enabled = true;
        itemNameTxt.raycastTarget = true;
        DragHelper.Instance.item = null;

        RemoveItemZoneController.Instance.Hide();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ItemInfoWindowController.Instance.SetText(Helper.Instance.GetItemDescription(ItemName));
    }
}
