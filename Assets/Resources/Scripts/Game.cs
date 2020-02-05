using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    [Header("Hero header")]
    public RectTransform heroHeader;
    public Text heroName, level, money, description;
    [Range(0.1f, 10f)]
    public float speed = 1f;
    public float error = 1f;

    [Header("Story")]
    public Story story;

    private bool moveHero;
    private Hero hero = Hero.Empty("");

    private void OnEnable()
    {
        heroHeader.anchoredPosition = new Vector2(0f, -1280f);
        moveHero = true;
    }

    private void Update()
    {
        if (moveHero) MoveHero();
    }

    public void Fill(Hero hero)
    {
        this.hero = hero;
        heroName.text = hero.name;
        level.text = string.Format("Level {0}", hero.level);
        money.text = string.Format("{0} Coins", hero.money);
        description.text = string.Format("{0} strength, {1} persistence, {2} agility, {3} attention.",
            Hero.Convert(hero.strength), Hero.Convert(hero.persistence), Hero.Convert(hero.agility), Hero.Convert(hero.attention));

        story.AddWords(new string[] { "Привет", ", Мама,", " я", " в", " телевизоре*.", " ЕЕЕЕЕЕЕ!" });
    }

    private void MoveHero()
    {
        float distance = -heroHeader.anchoredPosition.y * Time.deltaTime * speed;
        heroHeader.anchoredPosition += Vector2.up * distance;

        if (Mathf.Abs(heroHeader.anchoredPosition.y) < error)
        {
            heroHeader.anchoredPosition = Vector2.zero;
            moveHero = false;
        }
    }
}
