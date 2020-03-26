using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeroCtrl : MonoBehaviour
{
    public InventoryCtrl inventory;

    public static HeroCtrl current { get; private set; } = null;

    private Hero hero;

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
        hero.spells.ToList().ForEach(pair => inventory.AddSpell(pair.Key, pair.Value));
    }
}
