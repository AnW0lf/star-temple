using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HeroCtrl : MonoBehaviour
{
    public InventoryCtrl inventory;
    public Text heroName, heroLevel, heroMoney, heroHPState;
    public string levelPattern, moneyPattern;

    public static HeroCtrl current { get; private set; } = null;

    private Hero hero;

    public string Name
    {
        get => hero.name;
    }

    public int HP
    {
        get => hero.hp;
        set
        {
            hero.hp = value;
            UpdateInfo();
        }
    }

    public int Money
    {
        get => hero.money;
        set
        {
            hero.money = value;
            UpdateInfo();
        }
    }

    public int Level
    {
        get => hero.level;
        set
        {
            hero.level = value;
            UpdateInfo();
        }
    }

    public void Add(Item item, int count)
    {
        if (count <= 0) return;
        hero.AddItem(item, count);
        inventory.AddItem(item, count);
    }

    public void Add(CustomAction action, int count)
    {
        if (count <= 0) return;
        hero.AddAction(action, count);
        inventory.AddAction(action, count);
    }

    public void Subtract(Item item, int count)
    {
        if (count <= 0) return;
        hero.SubtractItem(item, count);
        inventory.SubtractItem(item, count);
    }

    public void Subtract(CustomAction action, int count)
    {
        if (count <= 0) return;
        hero.SubtractAction(action, count);
        inventory.SubtractAction(action, count);
    }

    public int Count(Item item)
    {
        if (hero.items.ContainsKey(item)) return hero.items[item];
        else return 0;
    }

    public int Count(CustomAction action)
    {
        if (hero.actions.ContainsKey(action)) return hero.actions[action];
        else return 0;
    }

    private void Awake()
    {
        if (current == null) current = this;
        else if (current != this) Destroy(gameObject);
    }

    private void Start()
    {
        hero = IOHelper.LoadHero();
        if (hero == null) hero = IOHelper.CreateHero("Nameless Monk");
        hero.items.ToList().ForEach(pair => inventory.AddItem(pair.Key, pair.Value));
        hero.actions.ToList().ForEach(pair => inventory.AddAction(pair.Key, pair.Value));

        UpdateInfo();
    }

    private void UpdateInfo()
    {
        heroName.text = hero.name;
        heroLevel.text = string.Format(levelPattern, hero.level);
        heroMoney.text = string.Format(moneyPattern, hero.money);
        heroHPState.text = hero.hp.ToString();
    }
}
