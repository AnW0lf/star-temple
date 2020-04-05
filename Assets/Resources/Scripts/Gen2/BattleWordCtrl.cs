using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleWordCtrl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDropHandler
{
    public Text txt;
    public Outline outline;

    public bool Actioned { get; private set; } = false;
    public int Chance { get; set; } = -1;

    private bool Highlighted
    {
        get => outline.enabled;
        set => outline.enabled = value;
    }

    public void SetWord(string word, bool actioned)
    {
        txt.text = word;
        Actioned = actioned;
        Highlighted = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(Actioned && InventoryCtrl.current.draggedAction != null)
        {
            Highlighted = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Highlighted = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(Actioned && InventoryCtrl.current.draggedAction != null)
        {
            CustomAction action = InventoryCtrl.current.draggedAction.Action;
            if (action == null) return;

            GameCtrl.current.BattleStep(action);
        }

        Highlighted = false;
    }
}
