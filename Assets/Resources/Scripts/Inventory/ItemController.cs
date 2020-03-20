using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemController : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
{
    public Text itemNameTxt, countTxt;
    [HideInInspector]
    public Item item;
    public string ItemName { get { return item.Name; } }
    public int Count { get { return item.Count; } private set { item.SetCount(value); } }

    private RectTransform draggedItem;

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
        draggedItem.position = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        draggedItem = HeroController.Instance.inventory.draggedItem;
        draggedItem.gameObject.SetActive(true);
        draggedItem.GetComponent<Text>().text = item.Name;

        DragHelper.Instance.item = this;

        if (item.Count == 1)
        {
            itemNameTxt.enabled = false;
            countTxt.enabled = false;
        }
        else
        {
            countTxt.text = (item.Count - 1).ToString();
        }

        if (item.Name != "*")
            RemoveItemZoneController.Instance.Show();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        draggedItem.GetComponent<Text>().text = "";
        draggedItem.gameObject.SetActive(false);
        draggedItem = null;

        DragHelper.Instance.item = null;

        itemNameTxt.enabled = true;
        countTxt.enabled = true;
        countTxt.text = item.Count.ToString();

        RemoveItemZoneController.Instance.Hide();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ItemInfoWindowController.Instance.SetText(Helper.Instance.GetItemDescription(ItemName));
    }
}
