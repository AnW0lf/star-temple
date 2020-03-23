using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollGamePanels : MonoBehaviour
{
    public RectTransform screens, bgImg;
    public RectTransform[] btns;
    public float swapDuration, bgImgXSizeOffset = 40f;
    public LeanTweenType swapEase;

    private RectTransform[] panels;

    private void Awake()
    {
        panels = new RectTransform[screens.childCount];
        for (int i = 0; i < panels.Length; i++)
            panels[i] = screens.GetChild(i).GetComponent<RectTransform>();
    }

    public void Swap(int index)
    {
        if(index < panels.Length)
        {
            screens.LeanMoveLocalX(500f - panels[index].anchoredPosition.x, swapDuration).setEase(swapEase);
            bgImg.LeanMoveLocalX(btns[index].anchoredPosition.x, swapDuration).setEase(LeanTweenType.easeInOutQuart);
            bgImg.LeanSize(new Vector2(btns[index].sizeDelta.x + bgImgXSizeOffset, bgImg.sizeDelta.y), swapDuration);
        }
    }
}
