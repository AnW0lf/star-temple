using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
[RequireComponent(typeof(Outline))]
public class ActionCtrl : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public float destroyDuration;
    public LeanTweenType destroyEase;

    public Action Action
    {
        get => _action;
        set
        {
            _action = value;
            Reload();
        }
    }

    public int Count
    {
        get => _count;
        set
        {
            _count = value;
            if (_action.rarity == ActionRarity.STANDART)
            {
                counter.text = "∞";
                counter.fontSize = 60;
            }
            else
                counter.text = _count.ToString();
            if (_count <= 0) DestroyThis();
        }
    }

    public Text counter;
    public Vector2 effectDistance;
    public Color effect, standart, common, rare, mythical;

    private Text txt;
    private Outline outline;
    private Action _action;
    private int _count;

    private void Awake()
    {
        txt = GetComponent<Text>();
        outline = GetComponent<Outline>();
    }

    public void OnPointerEnter()
    {
        outline.effectColor = effect;
        outline.effectDistance = effectDistance;
    }

    public void OnPointerExit()
    {
        outline.effectColor = Color.clear;
        outline.effectDistance = Vector2.zero;
    }

    public void OnPointerClick()
    {
        // Показать окно с описанием способности
    }

    private void Reload()
    {
        txt = GetComponent<Text>();
        outline = GetComponent<Outline>();

        txt.text = Action.name;

        switch (Action.rarity)
        {
            case ActionRarity.STANDART:
                txt.color = standart;
                break;
            case ActionRarity.COMMON:
                txt.color = common;
                break;
            case ActionRarity.RARE:
                txt.color = rare;
                break;
            case ActionRarity.MYTHICAL:
                txt.color = mythical;
                break;
        }

        outline.effectColor = Color.clear;
        outline.effectDistance = Vector2.zero;
    }

    public void DestroyThis()
    {
        gameObject.LeanScale(Vector3.zero, destroyDuration).setEase(destroyEase);
        Destroy(gameObject, destroyDuration);
    }

    public void OnDrag(PointerEventData eventData)
    {
        InventoryCtrl.current.draggedText.rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        InventoryCtrl.current.draggedText.enabled = false;
        InventoryCtrl.current.draggedAction = null;

        if (Action.rarity != ActionRarity.STANDART)
        {
            if (Count > 0 && !txt.enabled)
            {
                txt.enabled = true;
                counter.enabled = true;
            }
            else
            {
                counter.gameObject.LeanScaleX(0f, 0.15f).setOnComplete(() =>
                {
                    counter.text = Count.ToString();
                    counter.gameObject.LeanScaleX(1f, 0.15f);
                });
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        InventoryCtrl.current.draggedText.enabled = true;
        InventoryCtrl.current.draggedText.text = txt.text;
        InventoryCtrl.current.draggedText.font = txt.font;
        InventoryCtrl.current.draggedText.fontSize = txt.fontSize;
        InventoryCtrl.current.draggedText.fontStyle = txt.fontStyle;

        InventoryCtrl.current.draggedAction = this;

        if (Action.rarity != ActionRarity.STANDART)
        {
            int count = Count - 1;

            if (count <= 0)
            {
                txt.enabled = false;
                counter.enabled = false;
            }
            else
            {
                counter.gameObject.LeanScaleX(0f, 0.15f).setOnComplete(() =>
                {
                    counter.text = count.ToString();
                    counter.gameObject.LeanScaleX(1f, 0.15f);
                });
            }
        }
    }
}

public enum ActionRarity { STANDART, COMMON, RARE, MYTHICAL }

public enum ActionType { ATTACK, DEFENCE, SUPPORT }

[Serializable]
public class Action
{
    public string name;
    public ActionType type;
    public ActionRarity rarity;
    public int value;

    public Action(string name, ActionType type, ActionRarity rarity, int value)
    {
        this.name = name;
        this.type = type;
        this.rarity = rarity;
        this.value = value;
    }

    public override bool Equals(object obj)
    {
        return obj is Action spell &&
               name == spell.name &&
               type == spell.type &&
               rarity == spell.rarity &&
               value == spell.value;
    }

    public override int GetHashCode()
    {
        var hashCode = -714267209;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
        hashCode = hashCode * -1521134295 + type.GetHashCode();
        hashCode = hashCode * -1521134295 + rarity.GetHashCode();
        hashCode = hashCode * -1521134295 + value.GetHashCode();
        return hashCode;
    }

    public override string ToString()
    {
        return name;
    }

    public static bool operator ==(Action left, Action right)
    {
        if (left is null && right is null) return true;
        return left.Equals(right);
    }

    public static bool operator !=(Action left, Action right)
    {
        return !(left == right);
    }
}
