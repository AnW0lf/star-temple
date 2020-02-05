using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroBlank : MonoBehaviour, IBlank
{
    [Header("Текст")]
    public Text heroName;
    public Text level;
    public Text money;
    public Text description;
    [Header("Цвет")]
    public Color selectBackground = Color.yellow;
    public Color unselectBackground = Color.white;
    public Color selectHeaderText = Color.black;
    public Color unselectHeaderText = Color.grey;
    public Color selectDescriptionText = Color.black;
    public Color unselectDescriptionText = Color.grey;

    private Image background;

    private void Awake()
    {
        background = GetComponent<Image>();
    }

    private void Start()
    {
        Unselect();
    }

    public void Select()
    {
        background.color = new Color(selectBackground.r, selectBackground.g, selectBackground.b, Alpha);
        heroName.color = new Color(selectHeaderText.r, selectHeaderText.g, selectHeaderText.b, Alpha);
        level.color = new Color(selectHeaderText.r, selectHeaderText.g, selectHeaderText.b, Alpha);
        money.color = new Color(selectHeaderText.r, selectHeaderText.g, selectHeaderText.b, Alpha);
        description.color = new Color(selectDescriptionText.r, selectDescriptionText.g, selectDescriptionText.b, Alpha);
    }

    public void Unselect()
    {
        background.color = new Color(unselectBackground.r, unselectBackground.g, unselectBackground.b, Alpha);
        heroName.color = new Color(unselectHeaderText.r, unselectHeaderText.g, unselectHeaderText.b, Alpha);
        level.color = new Color(unselectHeaderText.r, unselectHeaderText.g, unselectHeaderText.b, Alpha);
        money.color = new Color(unselectHeaderText.r, unselectHeaderText.g, unselectHeaderText.b, Alpha);
        description.color = new Color(unselectDescriptionText.r, unselectDescriptionText.g, unselectDescriptionText.b, Alpha);
    }

    public void Fill(Hero hero)
    {
        heroName.text = hero.name;
        level.text = string.Format("Level {0}", hero.level);
        money.text = string.Format("{0} Coins", hero.money);
        description.text = string.Format("{0} strength, {1} persistence, {2} agility, {3} attention.",
            Hero.Convert(hero.strength), Hero.Convert(hero.persistence), Hero.Convert(hero.agility), Hero.Convert(hero.attention));
    }

    public float Alpha
    {
        get
        {
            return (background.color.a + heroName.color.a + level.color.a + money.color.a + description.color.a) / 5f;
        }
        set
        {
            float a = Mathf.Clamp01(value);
            background.color = new Color(background.color.r, background.color.g, background.color.b, a);
            heroName.color = new Color(heroName.color.r, heroName.color.g, heroName.color.b, a);
            level.color = new Color(level.color.r, level.color.g, level.color.b, a);
            money.color = new Color(money.color.r, money.color.g, money.color.b, a);
            description.color = new Color(description.color.r, description.color.g, description.color.b, a);
        }
    }

}
