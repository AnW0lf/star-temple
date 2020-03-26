using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
[RequireComponent(typeof(Outline))]
public class SpellCtrl : MonoBehaviour
{
    public float destroyDuration;
    public LeanTweenType destroyEase;

    public Spell Spell
    {
        get => _spell;
        set
        {
            _spell = value;
            Reload();
        }
    }

    public int Count
    {
        get => _count;
        set
        {
            _count = value;
            counter.text = _count.ToString();
            if (_count == 1) counter.gameObject.SetActive(false);
            else counter.gameObject.SetActive(true);
            if (_count <= 0) DestroyThis();
        }
    }

    public Text counter;
    public Vector2 effectDistance;
    public Color effect, standart, common, rare, mythical;

    private Text txt;
    private Outline outline;
    private Spell _spell;
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
        print(string.Format("Используется \'{0}\'", Spell.name));
    }

    private void Reload()
    {
        txt = GetComponent<Text>();
        outline = GetComponent<Outline>();

        txt.text = Spell.name;

        switch (Spell.rarity)
        {
            case SpellRarity.STANDART:
                txt.color = standart;
                break;
            case SpellRarity.COMMON:
                txt.color = common;
                break;
            case SpellRarity.RARE:
                txt.color = rare;
                break;
            case SpellRarity.MYTHICAL:
                txt.color = mythical;
                break;
        }

        outline.effectColor = Color.clear;
        outline.effectDistance = Vector2.zero;

        counter.gameObject.SetActive(Spell.rarity != SpellRarity.STANDART);
    }

    public void DestroyThis()
    {
        gameObject.LeanScale(Vector3.zero, destroyDuration).setEase(destroyEase);
        Destroy(gameObject, destroyDuration);
    }
}

public enum SpellRarity { STANDART, COMMON, RARE, MYTHICAL }

public enum SpellType { ATTACK, DEFENCE, SUPPORT }

[Serializable]
public class Spell
{
    public string name;
    public SpellType type;
    public SpellRarity rarity;
    public int value;

    public Spell(string name, SpellType type, SpellRarity rarity, int value)
    {
        this.name = name;
        this.type = type;
        this.rarity = rarity;
        this.value = value;
    }

    public override bool Equals(object obj)
    {
        return obj is Spell spell &&
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

    public static bool operator ==(Spell left, Spell right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Spell left, Spell right)
    {
        return !(left == right);
    }
}
