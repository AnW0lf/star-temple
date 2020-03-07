using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveItemZoneController : MonoBehaviour
{
    public RectTransform panel;
    public float speed = 5f, error = 1f;
    public static RemoveItemZoneController Instance { get; private set; } = null;

    private Coroutine moveTo = null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Show()
    {
        MoveTo(0f);
    }

    public void Hide()
    {
        MoveTo(panel.sizeDelta.y);
    }

    private void MoveTo(float ypos)
    {
        if (moveTo != null) StopCoroutine(moveTo);
        moveTo = StartCoroutine(MoveToCoroutine(ypos));
    }

    private IEnumerator MoveToCoroutine(float ypos)
    {
        while (Math.Abs(ypos - panel.anchoredPosition.y) > error)
        {
            yield return null;
            panel.anchoredPosition += Vector2.up * speed * Time.deltaTime * Math.Sign(ypos - panel.anchoredPosition.y);
        }
        panel.anchoredPosition = new Vector2(panel.anchoredPosition.x, ypos);
    }

    public void Remove()
    {
        if(DragHelper.Instance.item != null)
        {
            string itemName = DragHelper.Instance.item.item.name;
            HeroController.Instance.SubtractItem(itemName);
        }
        Hide();
    }
}
