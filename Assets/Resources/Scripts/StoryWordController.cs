﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoryWordController : MonoBehaviour, IDropHandler
{
    public StoryController story { get; private set; }

    [SerializeField]
    private Text txt;
    private Button btn;
    private int annotation_id, event_id;
    public StoryWord word { get; private set; }

    private bool annotated = false;

    private void Awake()
    {
        if (transform.GetSiblingIndex() == 0)
            story = FindObjectOfType<StoryController>();
        else
            story = transform.parent.GetChild(0).GetComponent<StoryWordController>().story;

        btn = GetComponent<Button>();
    }

    public void Show(float delay)
    {
        StartCoroutine(Showing(1f / delay));
    }

    public void Hide()
    {
        Destroy(gameObject);
    }

    private IEnumerator Showing(float increment)
    {
        while(txt.color.a < 1f)
        {
            txt.color += new Color(0f, 0f, 0f, increment * Time.deltaTime);
            yield return null;
        }
    }

    public void SetWord(StoryWord word)
    {
        this.word = word;
        txt.text = word.word;
        annotation_id = word.annotation_id;
        event_id = word.event_id;
        switch (word.type)
        {
            case WordType.REGULAR:
                txt.fontStyle = FontStyle.Normal;
                txt.raycastTarget = true;
                btn.interactable = false;
                break;
            case WordType.SEPARATOR:
                txt.fontStyle = FontStyle.Normal;
                txt.raycastTarget = false;
                btn.interactable = false;
                break;
            case WordType.BUTTON:
                txt.fontStyle = FontStyle.Bold;
                txt.raycastTarget = true;
                btn.interactable = true;
                btn.onClick.AddListener(() => btn.interactable = false);
                btn.onClick.AddListener(() => EventController.Instance.Execute(event_id));
                break;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(!annotated && DragHelper.Instance.item.Item.name == Helper.Star.name)
        {
            Inventory.Instance.SubtractItem(new Item(Helper.Star.name, -1));
            txt.text += Helper.Star.name;
            annotated = true;
            story.AddAnnotation(annotation_id);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
