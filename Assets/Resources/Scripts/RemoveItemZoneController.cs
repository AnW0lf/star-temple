using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveItemZoneController : MonoBehaviour
{
    public RectTransform panel;
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
        panel.LeanMoveLocalY(0f, duration).setEase(ease);
    }

    public void Hide()
    {
        panel.LeanMoveLocalY(panel.sizeDelta.y, duration).setEase(ease);
    }

    public void Spray()
    {
        if (DragHelper.Instance.item != null)
            RemoveItemWindowController.Instance.Show(DragHelper.Instance.item.item);
        Hide();
    }
}
