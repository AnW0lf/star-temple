using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Xml.Linq;
using System;

public class AnnotationWordCtrl : MonoBehaviour, IWord
{
    public Text txt;
    public WordEffects effects;

    private int _e_id, _d_id;
    private string _d_t;
    private bool _e_r, _d_r;

    public bool HasEvent { get; protected set; }

    public bool HasDrop { get; protected set; }

    public void SetWord(AnnotationWord word)
    {
        txt.text = word.word;
        _e_id = word.event_id;
        _d_id = word.drop_id;
        _e_r = word.event_reusable;
        _d_r = word.drop_reusable;
        _d_t = word.drop_type;
        HasEvent = _e_id > 0;
        HasDrop = _d_id > 0 && _d_t.Length > 0;

        if (CustomEventSystem.current.IsExitId(_e_id)) txt.fontStyle = FontStyle.BoldAndItalic;
        else txt.fontStyle = FontStyle.Italic;

        effects.hasEvent = () => { return HasEvent; };
        effects.hasDrop = () => { return HasDrop; };

        txt.raycastTarget = HasEvent || HasDrop;

        effects.Begin();
    }

    public void OnClick()
    {
        if (!HasEvent) return;
        CustomEventSystem.current.Execute(_e_id);
        HasEvent = _e_r;

        txt.raycastTarget = HasEvent || HasDrop;
    }

    public void OnDrop()
    {
        ItemCtrl droppedItem = InventoryCtrl.current.draggedItem;
        if (droppedItem != null)
        {
            if (HasDrop && _d_t == Helper.Instance.GetItemType(droppedItem.Item.Name))
            {
                InventoryCtrl.current.SubtractItem(droppedItem.Item, 1);
                CustomEventSystem.current.Execute(_d_id, droppedItem.Item);

                HasDrop = _d_r;

                txt.raycastTarget = HasEvent || HasDrop;
            }
        }
    }
}

public class AnnotationWord
{
    public string word, drop_type;
    public int event_id, drop_id;
    public bool event_reusable, drop_reusable;

    public AnnotationWord(string word) : this(word, "", -1, -1, false, false)
    { }

    public AnnotationWord(string word, string drop_type, int event_id, int drop_id, bool event_reusable, bool drop_reusable)
    {
        this.word = word;
        this.drop_type = drop_type;
        this.event_id = event_id;
        this.drop_id = drop_id;
        this.event_reusable = event_reusable;
        this.drop_reusable = drop_reusable;
    }

    public static AnnotationWord FromXml(XElement element)
    {
        if (!IOHelper.GetValue(element, out string word))
            throw new ArgumentException(string.Format("Story Word has incorrect value."));

        if (word == "") word = " ";

        AnnotationWord annotationWord = new AnnotationWord(word);

        IOHelper.GetAttributeValue(element, "drop_type", out annotationWord.drop_type);

        if (!IOHelper.GetAttributeValue(element, "drop_id", out annotationWord.drop_id) && annotationWord.drop_type != "")
            throw new ArgumentException(string.Format("Story Word \'{0}\' has incorrect value of attribute \'drop_id\'.", word));

        IOHelper.GetAttributeValue(element, "event_id", out annotationWord.event_id);

        IOHelper.GetAttributeValue(element, "event_reusable", out annotationWord.event_reusable);

        IOHelper.GetAttributeValue(element, "drop_reusable", out annotationWord.drop_reusable);

        return annotationWord;
    }
}
