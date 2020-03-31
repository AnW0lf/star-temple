using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SprayCtrl : MonoBehaviour, IDropHandler
{
    public float showDuration = 0.3f;
    public LeanTweenType showEase;
    public string textPattern;
    public Text text;

    public static SprayCtrl current { get; private set; } = null;
    public bool Visible
    {
        get => visible;
        set
        {
            visible = value;
            if (visible) Show();
            else Hide();
        }
    }

    private bool visible;
    private RectTransform rect;

    private void Awake()
    {
        if (current == null) current = this;
        else if (current != this) Destroy(gameObject);

        rect = GetComponent<RectTransform>();
        Visible = false;
    }

    private void Show()
    {
        rect.LeanMoveY(0f, showDuration).setEase(showEase);

        {
            ItemCtrl itemCtrl = InventoryCtrl.current.draggedItem;
            Item item;

            if (itemCtrl != null && (item = itemCtrl.Item) != null)
            {
                text.text = string.Format(textPattern, item.Name, item.Star);
            }
            else
            {
                text.text = "It works only for items";
            }
        }
    }

    private void Hide()
    {
        rect.LeanMoveY(rect.sizeDelta.y, showDuration).setEase(showEase);
    }

    public void OnDrop(PointerEventData eventData)
    {
        ItemCtrl itemCtrl = InventoryCtrl.current.draggedItem;
        Item item;

        if(itemCtrl != null && (item = itemCtrl.Item) != null)
        {
            int star = item.Star;
            HeroCtrl.current.Subtract(item, 1);
            HeroCtrl.current.Add(IOHelper.GetItem("*"), star);
        }

        Visible = false;
    }
}
