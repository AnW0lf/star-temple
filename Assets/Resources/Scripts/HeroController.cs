using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroController : MonoBehaviour
{
    [Header("Hero components")]
    public RectTransform rect;
    public Text heroNameTxt, levelTxt, moneyTxt, descriptionTxt;

    private Hero hero = Hero.Empty("");

    public Hero Hero
    {
        get
        {
            return hero;
        }
        set
        {
            hero = value;
            UpdateInfo();
        }
    }

    public int Money
    {
        get
        {
            return hero.money;
        }
        set
        {
            hero.money = Mathf.Max(value, 0);
            Helper.Instance.SaveHero(hero);
            UpdateInfo();
        }
    }

    public int Level
    {
        get
        {
            return hero.level;
        }
        set
        {
            hero.level = Mathf.Max(value, 1);
            Helper.Instance.SaveHero(hero);
            UpdateInfo();
        }
    }

    public int Strength
    {
        get
        {
            return hero.strength;
        }
        set
        {
            hero.strength = Mathf.Clamp(value, 0, 100);
            Helper.Instance.SaveHero(hero);
            UpdateInfo();
        }
    }

    public int Persistence
    {
        get
        {
            return hero.persistence;
        }
        set
        {
            hero.persistence = Mathf.Clamp(value, 0, 100);
            Helper.Instance.SaveHero(hero);
            UpdateInfo();
        }
    }

    public int Agility
    {
        get
        {
            return hero.agility;
        }
        set
        {
            hero.agility = Mathf.Clamp(value, 0, 100);
            Helper.Instance.SaveHero(hero);
            UpdateInfo();
        }
    }

    public int Attention
    {
        get
        {
            return hero.attention;
        }
        set
        {
            hero.attention = Mathf.Clamp(value, 0, 100);
            Helper.Instance.SaveHero(hero);
            UpdateInfo();
        }
    }

    public List<Item> Items
    {
        get
        {
            return hero.items;
        }
        set
        {
            hero.items = value;
            Helper.Instance.SaveHero(hero);
            UpdateInfo();
        }
    }

    private void UpdateInfo()
    {
        heroNameTxt.text = hero.name;
        levelTxt.text = string.Format("Level {0}", hero.level);
        moneyTxt.text = string.Format("{0} Coins", hero.money);
        descriptionTxt.text = string.Format("{0} strength, {1} persistence, {2} agility, {3} attention.",
            Hero.Convert(hero.strength), Hero.Convert(hero.persistence), Hero.Convert(hero.agility), Hero.Convert(hero.attention));
    }

    public void SaveHero()
    {
        Helper.Instance.SaveHero(hero);
    }
}
