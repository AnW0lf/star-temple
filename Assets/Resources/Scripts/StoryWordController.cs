using System;
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
    private int annotation_id, event_id, drop_id;
    private string drop_type = "";
    public StoryWord word { get; private set; }

    private bool annotated = false, dropped = false;

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
        while (txt.color.a < 1f)
        {
            txt.color += new Color(0f, 0f, 0f, increment * Time.deltaTime);
            yield return null;
        }
    }

    public void SetWord(StoryWord word)
    {
        this.word = word;
        txt.text = word.word;
        drop_type = word.drop_type;
        drop_id = word.drop_id;
        annotation_id = word.annotation_id;
        event_id = word.event_id;

        annotated = false;
        dropped = false;

        if (drop_id > 0 && event_id > 0)
        {
            txt.fontStyle = FontStyle.Bold;
            txt.raycastTarget = true;
            btn.interactable = true;

            InitButton(true);
        }
        else if (drop_id > 0 || annotation_id > 0)
        {
            txt.fontStyle = FontStyle.Normal;
            txt.raycastTarget = true;
            btn.interactable = false;

            InitButton(false);
        }
        else if (event_id > 0)
        {
            txt.fontStyle = FontStyle.Bold;
            txt.raycastTarget = true;

            InitButton(true);
        }
        else
        {
            txt.fontStyle = FontStyle.Normal;
            txt.raycastTarget = false;
            btn.interactable = false;

            InitButton(false);
        }
    }

    private void InitButton(bool active)
    {
        btn.interactable = active;

        if (active)
        {
            btn.onClick.AddListener(() => EventController.Instance.Execute(gameObject, event_id));
            if (!word.reusable_event)
                btn.onClick.AddListener(() => btn.interactable = false);
        }
        else btn.onClick.RemoveAllListeners();
    }

    public void OnDrop(PointerEventData eventData)
    {
        ItemController droppedItem = DragHelper.Instance.item;
        if (droppedItem != null)
        {
            if (!annotated && droppedItem.item.name == Helper.Star.name)
            {
                HeroController.Instance.SubtractItem(droppedItem.item.name);
                txt.text += droppedItem.item.name;
                annotated = true;
                story.AddAnnotation(annotation_id);
            }
            else if (!dropped && drop_type == Helper.Instance.GetItemType(droppedItem.item.name))
            {
                HeroController.Instance.SubtractItem(droppedItem.item.name);
                EventController.Instance.Execute(gameObject, drop_id);

                if (!word.reusable_drop) dropped = true;
            }
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
