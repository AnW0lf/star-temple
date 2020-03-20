using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroController : MonoBehaviour
{
    [Header("Hero components")]
    public RectTransform rect;
    public Text heroNameTxt, levelTxt, moneyTxt, descriptionTxt;

    [Header("Inventory")]
    public Inventory inventory;

    public static HeroController Instance { get; private set; } = null;

    private Hero hero = Hero.Empty("");

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddItem(string name, int count)
    {
        inventory.AddItem(new Item(name, Helper.Instance.GetItemType(name)
            , count, Helper.Instance.GetItemStarCount(name)));
    }

    public void SubtractItem(string name)
    {
        inventory.SubtractItem(new Item(name, Helper.Instance.GetItemType(name)
            , -1, Helper.Instance.GetItemStarCount(name)));
    }

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

    public int HP
    {
        get
        {
            return hero.hp;
        }
        set
        {
            hero.hp = Mathf.Max(value, 0);
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
            UpdateInfo();
        }
    }

    private void UpdateInfo()
    {
        heroNameTxt.text = hero.name;
        levelTxt.text = string.Format("Level {0}", Level);
        moneyTxt.text = string.Format("{0} Coins", Money);
        descriptionTxt.text = string.Format("{0} strength, {1} persistence, {2} agility, {3} attention. <color=#b45f06ff><b>Slightly Injured.</b></color>",
            Hero.Convert(Strength), Hero.Convert(Persistence), Hero.Convert(Agility), Hero.Convert(Attention));
    }

    public void SaveHero()
    {
        Helper.Instance.SaveHero(hero);
    }
}
