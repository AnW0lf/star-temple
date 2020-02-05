using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HeroBlankList : MonoBehaviour
{
    public GameObject heroBlankPrefab;
    public float verticalCenter = 300f;
    public float speed = 5f;
    public List<HeroBlank> heroes { get; private set; } = new List<HeroBlank>();
    public HeroBlank selected { get; private set; }
    public bool dragged { get; set; } = false;

    private RectTransform self;

    private void Awake()
    {
        self = GetComponent<RectTransform>();
    }

    void Start()
    {
        if (heroes.Count > 0)
            selected = heroes[0];
    }

    void Update()
    {
        if (heroes.Count == 0) return;

        foreach (HeroBlank hero in heroes) SetAlpha(hero);
        UpdateSelected();
        if (!dragged)
        {
            MoveToSelect();
        }
    }

    private void MoveToSelect()
    {
        if (selected == null) return;

        float requaredPos = -(verticalCenter + selected.GetComponent<RectTransform>().anchoredPosition.y);
        float distance = (requaredPos - self.anchoredPosition.y) * Time.deltaTime * speed;
        self.anchoredPosition += Vector2.up * distance;

    }

    private void SetAlpha(HeroBlank hero)
    {
        if (hero == null) return;

        float anchoredPos = self.anchoredPosition.y + verticalCenter;
        float heroAnchoredPos = hero.GetComponent<RectTransform>().anchoredPosition.y;
        float a = (verticalCenter - Mathf.Abs(anchoredPos + heroAnchoredPos)) / Mathf.Abs(verticalCenter);
        hero.Alpha = a;
    }

    private void UpdateSelected()
    {
        float a = 0f;
        foreach (HeroBlank hero in heroes)
        {
            if (hero.Alpha >= a)
            {
                a = hero.Alpha;
                selected = hero;
            }
        }

        foreach (HeroBlank hero in heroes)
        {
            if (hero == selected) hero.Select();
            else hero.Unselect();
        }
    }

    public void AddHero(Hero hero)
    {
        HeroBlank heroBlank = Instantiate(heroBlankPrefab, transform).GetComponent<HeroBlank>();
        heroBlank.Fill(hero);

        heroes.Add(heroBlank);
    }

    public void RemoveSelectedHero()
    {
        if (selected == null) return;
        Helper.instance.RemoveHero(selected.heroName.text);
        int id = heroes.IndexOf(selected);
        heroes.Remove(selected);
        Destroy(selected.gameObject);

        if (heroes.Count == 0) selected = null;
        else selected = heroes[Mathf.Clamp(id, 0, heroes.Count)];
    }
}
