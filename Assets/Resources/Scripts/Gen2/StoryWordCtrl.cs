using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Xml.Linq;
using System;

public class StoryWordCtrl : MonoBehaviour, IWord, IPointerEnterHandler, IPointerExitHandler
{
    public Text txt;
    public WordEffects effects;
    public Outline outline;

    private int _a_id, _e_id, _d_id;
    private string _d_t;
    private bool _e_r, _d_r;

    public bool Annotated { get; private set; }

    public bool HasEvent { get; protected set; }

    public bool HasDrop { get; protected set; }

    private bool Highlighted
    {
        get => outline.enabled;
        set => outline.enabled = value;
    }

    public void SetWord(StoryWord word)
    {
        txt.text = word.word;
        _a_id = word.annotation_id;
        _e_id = word.event_id;
        _d_id = word.drop_id;
        _e_r = word.event_reusable;
        _d_r = word.drop_reusable;
        _d_t = word.drop_type;

        Annotated = _a_id > 0;
        HasEvent = _e_id > 0;
        HasDrop = _d_id > 0 && _d_t.Length > 0;

        if (CustomEventSystem.current.IsExitId(_e_id)) txt.fontStyle = FontStyle.Bold;
        else txt.fontStyle = FontStyle.Normal;

        effects.hasEvent = () => { return HasEvent; };
        effects.hasDrop = () => { return HasDrop; };

        effects.Begin();

        Highlighted = false;
    }

    public void OnClick()
    {
        if (!HasEvent) return;
        CustomEventSystem.current.Execute(_e_id);
        HasEvent = _e_r;
        print("Click on " + txt.text + " event id " + _e_id);
    }

    public void OnDrop()
    {
        print(string.Format("Drop to \'{0}\'", txt.text));
        ItemCtrl droppedItem = InventoryCtrl.current.draggedItem;
        print(string.Format("Dropped Item is \'{0}\'", droppedItem));
        if (droppedItem != null)
        {
            print(string.Format("Annotated = {0}", Annotated));
            print(string.Format("HasDrop = {0}", HasDrop));
            if (Annotated && droppedItem.Item.Type == "star")
            {
                txt.text += droppedItem.Item.Name;
                InventoryCtrl.current.SubtractItem(droppedItem.Item, 1);
                GameCtrl.current.CallAnnotation(_a_id);

                Annotated = false;
            }
            else if (HasDrop && _d_t == Helper.Instance.GetItemType(droppedItem.Item.Name))
            {
                InventoryCtrl.current.SubtractItem(droppedItem.Item, 1);
                CustomEventSystem.current.Execute(_d_id, droppedItem.Item);

                HasDrop = _d_r;
            }
        }

        Highlighted = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Annotated)
        {
            ItemCtrl itemCtrl = InventoryCtrl.current.draggedItem;
            Item item;

            if (itemCtrl != null && (item = itemCtrl.Item) != null && item.Type == "star")
            {
                Highlighted = true;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Highlighted = false;
    }
}
public class StoryWord
{
    public string word, drop_type;
    public int annotation_id, event_id, drop_id;
    public bool event_reusable, drop_reusable;

    public StoryWord(string word) : this(word, "", -1, -1, -1, false, false)
    { }

    public StoryWord(string word, string drop_type, int annotation_id, int event_id, int drop_id, bool event_reusable, bool drop_reusable)
    {
        this.word = word;
        this.drop_type = drop_type;
        this.annotation_id = annotation_id;
        this.event_id = event_id;
        this.drop_id = drop_id;
        this.event_reusable = event_reusable;
        this.drop_reusable = drop_reusable;
    }

    public static StoryWord FromXml(XElement element)
    {
        if (!IOHelper.GetValue(element, out string word))
            throw new ArgumentException(string.Format("Story Word has incorrect value."));

        StoryWord storyWord = new StoryWord(word);

        IOHelper.GetAttributeValue(element, "drop_type", out storyWord.drop_type);

        if (!IOHelper.GetAttributeValue(element, "drop_id", out storyWord.drop_id) && storyWord.drop_type != "")
            throw new ArgumentException(string.Format("Story Word \'{0}\' has incorrect value of attribute \'drop_id\'.", word));

        IOHelper.GetAttributeValue(element, "annotation_id", out storyWord.annotation_id);

        IOHelper.GetAttributeValue(element, "event_id", out storyWord.event_id);

        IOHelper.GetAttributeValue(element, "event_reusable", out storyWord.event_reusable);

        IOHelper.GetAttributeValue(element, "drop_reusable", out storyWord.drop_reusable);

        return storyWord;
    }
}

