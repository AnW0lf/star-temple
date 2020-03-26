using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StoryWordCtrl : MonoBehaviour, IWord
{
    public Text txt;
    public WordEffects effects;

    private int _a_id, _e_id, _d_id;
    private string _d_t;
    private bool _e_r, _d_r;

    public bool Annotated { get; private set; }

    public bool HasEvent { get; protected set; }

    public bool HasDrop { get; protected set; }

    public void SetWord(StoryWord word)
    {
        txt.text = word.word;
        _a_id = word.annotation_id;
        _e_id = word.event_id;
        _d_id = word.drop_id;
        _e_r = word.reusable_event;
        _d_r = word.reusable_drop;
        _d_t = word.drop_type;

        Annotated = _a_id > 0;
        HasEvent = _e_id > 0;
        HasDrop = _d_id > 0 && _d_t.Length > 0;

        /* Проверка на выход
        if (Helper.Instance.IsExitId(_e_id)) txt.fontStyle = FontStyle.Bold;
        else txt.fontStyle = FontStyle.Normal;
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
        print("Click on " + txt.text);
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
                InventoryCtrl.current.SubstractItem(droppedItem.Item, 1);
                // Вызвать аннотацию

                Annotated = false;
            }
            else if (HasDrop && _d_t == Helper.Instance.GetItemType(droppedItem.Item.Name))
            {
                InventoryCtrl.current.SubstractItem(droppedItem.Item, 1);
                // Вызвать событие по _d_id

                HasDrop = _d_r;
            }
        }
    }
}
