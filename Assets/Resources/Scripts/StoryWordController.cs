using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoryWordController : MonoBehaviour, IWord, IDropHandler
{
    public StoryController story { get; private set; }

    [SerializeField]
    private Text text;
    private Image img;
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

        img = GetComponent<Image>();
        btn = GetComponent<Button>();
    }

    public void Show(float delay)
    {
        StartCoroutine(Showing(1f / delay));
    }

    public void Hide(float delay)
    {
        StartCoroutine(Hiding(1f / delay));
    }

    private IEnumerator Showing(float increment)
    {
        while(text.color.a < 1f)
        {
            text.color += new Color(0f, 0f, 0f, increment * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator Hiding(float decrement)
    {
        while (text.color.a < 1f)
        {
            text.color -= new Color(0f, 0f, 0f, decrement * Time.deltaTime);
            yield return null;
        }
        Destroy(gameObject);
    }

    public void SetWord(StoryWord word)
    {
        this.word = word;
        text.text = word.word;
        annotation_id = word.annotation_id;
        event_id = word.event_id;
        switch (word.type)
        {
            case WordType.REGULAR:
                text.fontStyle = FontStyle.Normal;
                btn.interactable = false;
                img.raycastTarget = true;
                break;
            case WordType.SEPARATOR:
                text.fontStyle = FontStyle.Normal;
                btn.interactable = false;
                img.raycastTarget = false;
                break;
            case WordType.BUTTON:
                text.fontStyle = FontStyle.Bold;
                btn.interactable = true;
                img.raycastTarget = true;
                break;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(!annotated && DragHelper.Instance.item.Item.name == Helper.Star.name)
        {
            Inventory.Instance.SubtractItem(new Item(Helper.Star.name, -1));
            text.text += Helper.Star.name;
            annotated = true;
            story.AddAnnotation(word.annotation_id);
        }
    }
}
