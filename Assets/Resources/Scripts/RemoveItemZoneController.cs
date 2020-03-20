using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemoveItemZoneController : MonoBehaviour
{
    public RectTransform panel;
    public Text txt;
    public float duration = 5f;
    public LeanTweenType ease;
    public static RemoveItemZoneController Instance { get; private set; } = null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Show()
    {
        Item item = DragHelper.Instance.item.item;
        txt.text = string.Format("Распылить \'{0}\'\nи получить {1} *", item.Name, item.Star);
        panel.LeanMoveLocalY(0f, duration).setEase(ease);
    }

    public void Hide()
    {
        panel.LeanMoveLocalY(panel.sizeDelta.y, duration).setEase(ease);
    }

    public void Spray()
    {
        Item item = DragHelper.Instance.item.item;

        HeroController.Instance.AddItem("*", item.Star);
        HeroController.Instance.SubtractItem(item.Name);

        Hide();
    }
}
