using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
        _e_r = word.reusable_event;
        _d_r = word.reusable_drop;
        _d_t = word.drop_type;
        HasEvent = _e_id > 0;
        HasDrop = _d_id > 0 && _d_t.Length > 0;

        /* Проверка на выход
        if (Helper.Instance.IsExitId(_e_id)) txt.fontStyle = FontStyle.BoldAndItalic;
        else txt.fontStyle = FontStyle.Italic;
        */

        effects.hasEvent = () => { return HasEvent; };
        effects.hasDrop = () => { return HasDrop; };

        effects.Begin();
    }

    public void OnClick()
    {
        if (!HasEvent) return;
        // Вызвать событие по _e_id
        HasEvent = _e_r;
    }

    public void OnDrop()
    {
        ItemController droppedItem = DragHelper.Instance.item;
        if (droppedItem != null)
        {
            if (HasDrop && _d_t == Helper.Instance.GetItemType(droppedItem.item.Name))
            {
                // Удалить предмет из инвентаря
                // Вызвать событие по _d_id

                HasDrop = _d_r;
            }
        }
    }
}
