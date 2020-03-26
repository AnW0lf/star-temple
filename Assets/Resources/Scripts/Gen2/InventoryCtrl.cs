using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class InventoryCtrl : MonoBehaviour
{
    public Text backpackBtn, spellsBtn;
    public Image btnsBackground, backpackArea, spellsArea;
    public Transform backpackAreaContent, spellsAreaContent;
    public RectTransform container;
    public float showDuration = 0.7f, swapDuration = 0.7f;
    public LeanTweenType showEase, swapEase;
    public Color btnSelected, btnUnselected, backpackAreaColor, spellsAreaColor;

    [Header("Backpack && Spells")]
    public GameObject itemPref;
    public GameObject spellPref;

    [Header("Dragged Text")]
    public Text draggedText;

    private Dictionary<Item, ItemCtrl> items;
    private Dictionary<Spell, SpellCtrl> spells;

    public static InventoryCtrl current { get; private set; } = null;
    public ItemCtrl draggedItem { get; set; } = null;

    public bool Visible
    {
        get
        {
            return isShowed;
        }

        set
        {
            if (value) Show();
            else Hide();
        }
    }

    private bool isShowed = false;
    private RectTransform rect;

    private void Awake()
    {
        if (current == null) current = this;
        else if (current != this) Destroy(gameObject);

        rect = GetComponent<RectTransform>();
        backpackArea.color = backpackAreaColor;
        spellsArea.color = spellsAreaColor;
        items = new Dictionary<Item, ItemCtrl>();
        spells = new Dictionary<Spell, SpellCtrl>();
    }

    private void Start()
    {
        Show();
        OpenBackpack();
    }

    private void Show()
    {
        rect.LeanMoveY(0f, showDuration).setEase(showEase).setOnComplete(() => isShowed = true);
    }

    private void Hide()
    {
        rect.LeanMoveY(-rect.sizeDelta.y, showDuration).setEase(showEase).setOnComplete(() => isShowed = false);
    }

    private void Swap(int index)
    {
        if (index == 0)
        {
            container.LeanMoveX(-container.sizeDelta.x / 2f * 0f, swapDuration).setEase(swapEase);
            btnsBackground.color = backpackAreaColor;
            backpackBtn.color = btnSelected;
            spellsBtn.color = btnUnselected;
        }
        else if (index == 1)
        {
            container.LeanMoveX(-container.sizeDelta.x / 2f * 1f, swapDuration).setEase(swapEase);
            btnsBackground.color = spellsAreaColor;
            spellsBtn.color = btnSelected;
            backpackBtn.color = btnUnselected;
        }
    }

    public void OpenBackpack()
    {
        Swap(0);
    }

    public void OpenSpells()
    {
        Swap(1);
    }

    public void AddItem(Item item, int count)
    {
        if (items.ContainsKey(item))
        {
            items[item].Count += count;
        }
        else
        {
            ItemCtrl ic = Instantiate(itemPref, backpackAreaContent).GetComponent<ItemCtrl>();
            ic.Item = item;
            ic.Count = count;
            items.Add(item, ic);
        }
    }
    public void AddSpell(Spell spell, int count)
    {
        if (spells.ContainsKey(spell) && spell.rarity != SpellRarity.STANDART)
        {
            spells[spell].Count += count;
        }
        else
        {
            SpellCtrl sc = Instantiate(spellPref, spellsAreaContent).GetComponent<SpellCtrl>();
            sc.Spell = spell;
            sc.Count = count;
            spells.Add(spell, sc);
        }
    }

    public void SubstractItem(Item item, int count)
    {
        if (items.ContainsKey(item))
        {
            items[item].Count -= count;

            if (items[item].Count <= 0)
                items.Remove(item);
        }
    }
    public void SubstractSpell(Spell spell, int count)
    {
        if (spells.ContainsKey(spell) && spell.rarity != SpellRarity.STANDART)
        {
            spells[spell].Count -= count;

            if (spells[spell].Count <= 0)
                spells.Remove(spell);
        }
    }

}
