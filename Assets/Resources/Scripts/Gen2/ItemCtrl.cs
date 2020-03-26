using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemCtrl : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public Text txt, counter;
    public float destroyDuration;
    public LeanTweenType destroyEase;
    public Color normal = Color.black, increase = Color.green, decrease = Color.red;
    public Item Item
    {
        get => _item;
        set
        {
            _item = value;
            Reload();
        }
    }
    public int Count
    {
        get => _count;
        set
        {
            int oldCount = _count;
            _count = value;

            if (oldCount != _count)
            {
                if (oldCount < _count) counter.color = increase;
                else if (oldCount > _count) counter.color = decrease;
                counter.gameObject.LeanScale(Vector3.one * 1.25f, 0.3f).setOnComplete(() =>
                {
                    counter.text = _count.ToString();
                    counter.gameObject.LeanScale(Vector3.one, 0.3f).setOnComplete(() =>
                    counter.color = normal);
                });
            }

            counter.text = _count.ToString();

            if (_count == 1) counter.gameObject.SetActive(false);
            else counter.gameObject.SetActive(true);
            if (_count <= 0) DestroyThis();
        }
    }

    private Item _item;
    private int _count;

    public void Reload()
    {
        counter.text = Count.ToString();
        txt.text = Item.Name;
    }

    public void OnDrag(PointerEventData eventData)
    {
        InventoryCtrl.current.draggedText.rectTransform.position = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Начать перемещение
        InventoryCtrl.current.draggedText.enabled = true;
        InventoryCtrl.current.draggedText.text = txt.text;
        InventoryCtrl.current.draggedText.font = txt.font;
        InventoryCtrl.current.draggedText.fontSize = txt.fontSize;
        InventoryCtrl.current.draggedText.fontStyle = txt.fontStyle;

        InventoryCtrl.current.draggedItem = this;

        int count = Count - 1;

        if (count <= 0)
        {
            txt.enabled = false;
            counter.enabled = false;
        }
        else
        {
            counter.gameObject.LeanScaleX(0f, 0.15f).setOnComplete(() =>
            {
                counter.text = count.ToString();
                counter.gameObject.LeanScaleX(1f, 0.15f);
            });
        }

        // Открыть окно распыления
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        InventoryCtrl.current.draggedText.enabled = false;
        InventoryCtrl.current.draggedItem = null;

        if (Count > 0 && !txt.enabled)
        {
            txt.enabled = true;
            counter.enabled = true;
        }
        else
        {
            counter.gameObject.LeanScaleX(0f, 0.15f).setOnComplete(() =>
            {
                counter.text = Count.ToString();
                counter.gameObject.LeanScaleX(1f, 0.15f);
            });
        }
    }

    public void OnPointerClick()
    {
        // Показать окно с описание предмета
    }

    public void DestroyThis()
    {
        gameObject.LeanScale(Vector3.zero, destroyDuration).setEase(destroyEase);
        Destroy(gameObject, destroyDuration);
    }
}
