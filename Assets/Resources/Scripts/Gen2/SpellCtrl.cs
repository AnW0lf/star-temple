using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
[RequireComponent(typeof(Outline))]
public class SpellCtrl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SpellRarity rarity;
    public Text counter;
    public Vector2 effectDistance;
    public Color effect, standart, common, rare, mythical;

    private Text txt;
    private Outline outline;

    public void OnPointerEnter(PointerEventData eventData)
    {
        outline.effectColor = effect;
        outline.effectDistance = effectDistance;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        outline.effectColor = Color.clear;
        outline.effectDistance = Vector2.zero;
    }

    private void Awake()
    {
        txt = GetComponent<Text>();
        outline = GetComponent<Outline>();

        switch (rarity)
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
    }


}

public enum SpellRarity { STANDART, COMMON, RARE, MYTHICAL }
