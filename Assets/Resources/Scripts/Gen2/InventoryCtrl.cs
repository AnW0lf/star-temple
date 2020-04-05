using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private Dictionary<string, ItemCtrl> items;
    private Dictionary<string, ActionCtrl> spells;

    public static InventoryCtrl current { get; private set; } = null;
    public ItemCtrl draggedItem { get; set; } = null;
    public ActionCtrl draggedAction { get; set; } = null;

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
        items = new Dictionary<string, ItemCtrl>();
        spells = new Dictionary<string, ActionCtrl>();
    }

    private void Start()
    {
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
        if (items.ContainsKey(item.Name))
        {
            items[item.Name].Count += count;
        }
        else
        {
            ItemCtrl ic = Instantiate(itemPref, backpackAreaContent).GetComponent<ItemCtrl>();
            ic.Item = item;
            ic.Count = count;
            items.Add(item.Name, ic);
        }
    }
    public void AddAction(CustomAction action, int count)
    {
        if (spells.ContainsKey(action.name) && action.rarity != ActionRarity.STANDART)
        {
            spells[action.name].Count += count;
        }
        else
        {
            ActionCtrl sc = Instantiate(spellPref, spellsAreaContent).GetComponent<ActionCtrl>();
            sc.Action = action;
            sc.Count = count;
            spells.Add(action.name, sc);
        }
    }

    public void SubtractItem(Item item, int count)
    {
        if (items.ContainsKey(item.Name))
        {
            items[item.Name].Count -= count;

            if (items[item.Name].Count <= 0)
                items.Remove(item.Name);
        }
    }
    public void SubtractAction(CustomAction action, int count)
    {
        if (spells.ContainsKey(action.name) && action.rarity != ActionRarity.STANDART)
        {
            spells[action.name].Count -= count;

            if (spells[action.name].Count <= 0)
                spells.Remove(action.name);
        }
    }

}
