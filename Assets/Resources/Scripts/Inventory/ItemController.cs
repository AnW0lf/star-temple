using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemController : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Text itemNameTxt, countTxt;
    public Item Item { get; private set; }
    public string ItemName { get; private set; }
    public int Count { get; private set; }

    private Transform container;
    private Transform grandparent;

    private void Awake()
    {
        grandparent = transform.parent.parent;
    }

    public void Fill(Item item)
    {
        this.Item = item;
        ItemName = this.Item.name;
        Count = this.Item.count;
        itemNameTxt.text = ItemName;
        countTxt.text = Count.ToString();
    }

    public void ChangeCount(int value)
    {
        Count += value;
        countTxt.text = Count.ToString();
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
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(container);
        if (Item.name.Equals(Helper.Star.name))
            transform.SetAsFirstSibling();
        countTxt.enabled = true;
        itemNameTxt.raycastTarget = true;
    }
}
