using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

static class IOHelper
{
    private static bool initialized = false;
    private static string dirPath, fHero, fItem, fSpell;

    private static void Initialize()
    {
        if (initialized) return;

#if UNITY_ANDROID && !UNITY_EDITOR
        dirPath = Path.Combine(Application.persistentDataPath, "Resources/XML/");
#else
        dirPath = Path.Combine(Application.dataPath, "Resources/XML/");
#endif

        fHero = "hero.xml";
        fItem = "item.xml";
        fSpell = "spell.xml";

        initialized = true;
    }

    public static Hero LoadHero()
    {
        Initialize();

        XElement root = ReadXml(fHero);
        if (root == null) return null;

        return Hero.FromXml(root);
    }

    public static Hero CreateHero(string name)
    {
        Initialize();

        Hero hero = new Hero("Nameless Monk");
        hero.AddItem(GetItem("*"), 3);
        hero.AddSpell(GetSpell("Удар"), 1);
        hero.AddSpell(GetSpell("Блок"), 1);

        XElement xmlHero = Hero.ToXml(hero);
        WriteXml(fHero, xmlHero);

        return LoadHero();
    }

    public static void SaveHero(Hero hero)
    {
        Initialize();

        XElement xmlHero = Hero.ToXml(hero);
        WriteXml(fHero, xmlHero);
    }

    public static Item GetItem(string itemName)
    {
        Initialize();
        XElement root = ReadXml(fItem);
        XElement xmlItem = root.Elements("item").ToList().Find(l =>
        {
            if (!GetAttributeValue(l, "name", out string name))
                throw new ArgumentException(string.Format("Item has incorrect value of attribute \'name\'."));
            return name == itemName;
        });

        if (xmlItem == null) return null;

        int star = 0;
        string type = "";
        Dictionary<string, string> options = new Dictionary<string, string>();

        foreach (XAttribute attr in xmlItem.Attributes())
        {
            switch (attr.Name.ToString())
            {
                case "type":
                    if (!GetAttributeValue(attr, out type))
                        throw new ArgumentException(string.Format("Item \'{0}\' has incorrect value of attribute \'type\'.", itemName));
                    break;
                case "star":
                    if (!GetAttributeValue(attr, out star))
                        throw new ArgumentException(string.Format("Item \'{0}\' has incorrect value of attribute \'star\'.", itemName));
                    break;
                default:
                    if (!GetAttributeValue(attr, out string value))
                        throw new ArgumentException(string.Format("Item \'{0}\' has incorrect value of attribute \'{1}\'.", itemName, attr.Name));
                    options.Add(attr.Name.ToString(), value);
                    break;
            }
        }

        return new Item(itemName, type, star, options);
    }

    public static Spell GetSpell(string spellName)
    {
        Initialize();
        XElement root = ReadXml(fSpell);
        XElement xmlSpell = root.Elements("spell").ToList().Find(l =>
        {
            if (!GetAttributeValue(l, "name", out string name))
                throw new ArgumentException(string.Format("Spell has incorrect value of attribute \'name\'."));
            return name == spellName;
        });

        if (xmlSpell == null) return null;

        if (!GetAttributeValue(xmlSpell, "type", out int type))
            throw new ArgumentException(string.Format("Spell \'{0}\' has incorrect value of attribute \'type\'.", spellName));

        if (!GetAttributeValue(xmlSpell, "rarity", out int rarity))
            throw new ArgumentException(string.Format("Spell \'{0}\' has incorrect value of attribute \'rarity\'.", spellName));

        if (!GetAttributeValue(xmlSpell, "value", out int value))
            throw new ArgumentException(string.Format("Spell \'{0}\' has incorrect value of attribute \'value\'.", spellName));

        return new Spell(spellName, (SpellType)type, (SpellRarity)rarity, value);
    }

    public static bool GetValue(XElement element, out int value)
    {
        if (element != null && !element.HasElements && int.TryParse(element.Value, out value))
            return true;
        value = 0;
        return false;
    }

    public static bool GetValue(XElement element, out string value)
    {
        if (element != null && !element.HasElements)
        {
            value = element.Value;
            return true;
        }
        value = "";
        return false;
    }

    public static bool GetAttributeValue(XAttribute attribute, out int value)
    {
        if (attribute != null && int.TryParse(attribute.Value, out value))
            return true;
        value = 0;
        return false;
    }

    public static bool GetAttributeValue(XAttribute attribute, out string value)
    {
        if (attribute != null)
        {
            value = attribute.Value;
            return true;
        }
        value = "";
        return false;
    }

    public static bool GetAttributeValue(XElement element, string attrName, out int value)
    {
        if (element != null && element.Attribute(attrName) != null &&
            int.TryParse(element.Attribute(attrName).Value, out value))
            return true;
        value = 0;
        return false;
    }

    public static bool GetAttributeValue(XElement element, string attrName, out string value)
    {
        if (element != null && element.Attribute(attrName) != null)
        {
            value = element.Attribute(attrName).Value;
            return true;
        }
        value = "";
        return false;
    }

    private static XElement ReadXml(string fileName)
    {
        if (!File.Exists(dirPath + fileName)) return null;
        return XDocument.Parse(File.ReadAllText(dirPath + fileName)).Root;
    }

    private static void WriteXml(string fileName, XElement root)
    {
        XDocument doc = new XDocument();
        doc.Add(root);
        File.WriteAllText(dirPath + fileName, doc.ToString());
    }


}

[Serializable]
public class Hero
{
    public string name;
    public int money, level, hp;
    public Dictionary<Item, int> items;
    public Dictionary<Spell, int> spells;

    public Hero()
    {
        this.name = "Nameless";
        this.hp = 100;
        this.level = 1;
        this.money = 0;
        this.items = new Dictionary<Item, int>();
        this.spells = new Dictionary<Spell, int>();
    }

    public Hero(string name)
    {
        this.name = name;
        this.hp = 100;
        this.level = 1;
        this.money = 0;
        this.items = new Dictionary<Item, int>();
        this.spells = new Dictionary<Spell, int>();
    }

    public Hero(string name, int hp, int level, int money, Dictionary<Item, int> items, Dictionary<Spell, int> spells)
    {
        this.name = name;
        this.hp = hp;
        this.level = level;
        this.money = money;
        this.items = new Dictionary<Item, int>(items);
        this.spells = new Dictionary<Spell, int>(spells);
    }

    public Hero(Hero other) : this(other.name, other.hp, other.level, other.money, other.items, other.spells)
    { }

    public void AddItem(Item item, int count)
    {
        if (items.ContainsKey(item))
            items[item] += count;
        else
            items.Add(item, count);
    }

    public void AddSpell(Spell spell, int count)
    {
        if (spells.ContainsKey(spell))
            spells[spell] += count;
        else
            spells.Add(spell, count);
    }

    public void SubtractItem(Item item, int count)
    {
        if (items.ContainsKey(item))
            items[item] -= count;
    }

    public void SubtractSpell(Spell spell, int count)
    {
        if (spells.ContainsKey(spell))
            spells[spell] -= count;
    }

    public override string ToString()
    {
        return string.Format("Name:{0} Items:[{1}] Spells:{2}", name, items.ToString(), spells.ToString());
    }

    public override bool Equals(object obj)
    {
        return obj is Hero hero &&
               name == hero.name &&
               EqualityComparer<Dictionary<Item, int>>.Default.Equals(items, hero.items) &&
               EqualityComparer<Dictionary<Spell, int>>.Default.Equals(spells, hero.spells);
    }

    public override int GetHashCode()
    {
        var hashCode = -826629381;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
        hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<Item, int>>.Default.GetHashCode(items);
        hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<Spell, int>>.Default.GetHashCode(spells);
        return hashCode;
    }

    public static Hero FromXml(XElement element)
    {
        if (!IOHelper.GetAttributeValue(element, "name", out string name))
            throw new ArgumentException("Hero has incorrect value of attribute \'name\'");

        if (!IOHelper.GetAttributeValue(element, "hp", out int hp))
            throw new ArgumentException(string.Format("Hero \'{0}\' has incorrect value of attribute \'hp\'", name));

        if (!IOHelper.GetAttributeValue(element, "level", out int level))
            throw new ArgumentException(string.Format("Hero \'{0}\' has incorrect value of attribute \'level\'", name));

        if (!IOHelper.GetAttributeValue(element, "money", out int money))
            throw new ArgumentException(string.Format("Hero \'{0}\' has incorrect value of attribute \'money\'", name));

        Dictionary<Item, int> items = new Dictionary<Item, int>();
        Dictionary<Spell, int> spells = new Dictionary<Spell, int>();

        foreach (XElement xmlItem in element.Elements("item"))
        {
            if (!IOHelper.GetAttributeValue(xmlItem, "name", out string itemName))
                throw new ArgumentException(string.Format("Hero \'{0}\' has Item with incorrect value of attribute \'name\'", name));

            if (!IOHelper.GetAttributeValue(xmlItem, "count", out int count))
                throw new ArgumentException(string.Format("Hero \'{0}\' has Item \'{1}\' with incorrect value of attribute \'count\'", name, itemName));

            Item item = IOHelper.GetItem(itemName);
            if (item == null) continue;
            items.Add(item, count);
        }

        foreach (XElement xmlSpell in element.Elements("spell"))
        {
            if (!IOHelper.GetAttributeValue(xmlSpell, "name", out string spellName))
                throw new ArgumentException(string.Format("Hero \'{0}\' has Spell with incorrect value of attribute \'name\'", name));

            if (!IOHelper.GetAttributeValue(xmlSpell, "count", out int count))
                throw new ArgumentException(string.Format("Hero \'{0}\' has Spell \'{1}\' with incorrect value of attribute \'count\'", name, spellName));

            Spell spell = IOHelper.GetSpell(spellName);
            if (spell == null) continue;
            spells.Add(spell, count);
        }

        return new Hero(name, hp, level, money, items, spells);
    }

    public static XElement ToXml(Hero hero)
    {
        XElement xmlHero = new XElement("hero");
        xmlHero.Add(new XAttribute("name", hero.name));
        xmlHero.Add(new XAttribute("hp", hero.hp));
        xmlHero.Add(new XAttribute("level", hero.level));
        xmlHero.Add(new XAttribute("money", hero.money));

        foreach (var item in hero.items)
        {
            XElement xmlItem = new XElement("item");
            xmlItem.Add(new XAttribute("name", item.Key.Name));
            xmlItem.Add(new XAttribute("count", item.Value));
            xmlHero.Add(xmlItem);
        }

        foreach (var spell in hero.spells)
        {
            XElement xmlSpell = new XElement("spell");
            xmlSpell.Add(new XAttribute("name", spell.Key.name));
            xmlSpell.Add(new XAttribute("count", spell.Value));
            xmlHero.Add(xmlSpell);
        }

        return xmlHero;
    }
}

[Serializable]
public class Item
{
    public string Name { get; private set; } = "";
    public string Type { get; private set; } = "";
    public int Star { get; private set; } = 0;
    public Dictionary<string, string> Options { get; private set; }

    public Item() : this("", "", 0)
    {
        Options = new Dictionary<string, string>();
    }

    public Item(Item other) : this(other.Name, other.Type, other.Star, other.Options)
    { }

    public Item(string name, string type, int star)
    {
        Name = name;
        Type = type;
        Star = star;
        Options = new Dictionary<string, string>();
    }

    public Item(string name, string type, int star, Dictionary<string, string> options) : this(name, type, star)
    {
        Options = new Dictionary<string, string>(options);
    }

    public void SetName(string name)
    {
        Name = name;
    }

    public void SetType(string type)
    {
        Type = type;
    }

    public void SetStar(int star)
    {
        Star = star;
    }

    public void AddOption(string key, string value)
    {
        if (!Options.ContainsKey(key)) Options.Add(key, value);
        else Options[key] = value;
    }

    public void AddOption(KeyValuePair<string, string> pair)
    {
        if (!Options.ContainsKey(pair.Key)) Options.Add(pair.Key, pair.Value);
        else Options[pair.Key] = pair.Value;
    }

    public void RemoveOption(string key)
    {
        if (Options.ContainsKey(key)) Options.Remove(key);
    }

    public override bool Equals(object obj)
    {
        return obj is Item item &&
               Name == item.Name &&
               Type == item.Type &&
               Star == item.Star &&
               EqualityComparer<Dictionary<string, string>>.Default.Equals(Options, item.Options);
    }

    public override int GetHashCode()
    {
        var hashCode = 658799709;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Type);
        hashCode = hashCode * -1521134295 + Star.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<string, string>>.Default.GetHashCode(Options);
        return hashCode;
    }

    public override string ToString()
    {
        return Name;
    }
}
