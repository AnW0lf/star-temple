using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class InventoryCtrl : MonoBehaviour
{
    public Text backpackBtn, spellsBtn;
    public Image btnsBackground, backpackArea, spellsArea;
    public RectTransform container;
    public float showDuration = 0.7f, swapDuration = 0.7f;
    public LeanTweenType showEase, swapEase;
    public Color btnSelected, btnUnselected, backpackAreaColor, spellsAreaColor;

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
        rect = GetComponent<RectTransform>();
        backpackArea.color = backpackAreaColor;
        spellsArea.color = spellsAreaColor;
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
}
