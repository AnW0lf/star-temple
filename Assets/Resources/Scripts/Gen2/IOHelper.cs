using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

static class IOHelper
{
    private static bool initialized = false;
    private static string dirPath, fHero, fItem, fAction;

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
        fAction = "action.xml";

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
        hero.AddAction(GetAction("Удар"), 1);
        hero.AddAction(GetAction("Блок"), 1);

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

    public static Room LoadRoom(string name)
    {
        XElement root = ReadXml(string.Format("{0}.xml", name));

        if (!GetAttributeValue(root, "name", out string roomName))
            throw new ArgumentException(string.Format("Room {0} has incorrect value of attribute \'name\'", name));

        if (!GetAttributeValue(root, "type", out string roomType))
            throw new ArgumentException(string.Format("Room {0} has incorrect value of attribute \'type\'", name));

        List<StoryWord> story = new List<StoryWord>();
        List<Annotation> annotations = new List<Annotation>();
        List<CustomEvent> events = new List<CustomEvent>();
        Dictionary<string, int> triggers = new Dictionary<string, int>();

        // Story
        {
            XElement XStory = root.Element("story");
            if (XStory != null)
            {
                foreach (XElement XWord in XStory.Elements("word"))
                    story.Add(StoryWord.FromXml(XWord));
            }
        }

        // Annotations
        {
            XElement XAnnotations = root.Element("annotations");
            if (XAnnotations != null)
            {
                foreach (XElement XAnnotation in XAnnotations.Elements("annotation"))
                    annotations.Add(Annotation.FromXml(XAnnotation));
            }
        }

        // Events
        {
            XElement XEvents = root.Element("events");
            if (XEvents != null)
            {
                foreach (XElement XEvent in XEvents.Elements("event"))
                {
                    events.Add(CustomEvent.FromXml(XEvent));
                }
            }
        }

        // Triggers
        {
            XElement XTriggers = root.Element("triggers");
            if (XTriggers != null)
            {
                foreach (XElement XTrigger in XTriggers.Elements("trigger"))
                {
                    if (!GetAttributeValue(XTrigger, "name", out string triggerName))
                        throw new ArgumentException(string.Format("Room {0} Trigger has incorrect value of attribete \'name\' = \'{1}\'", roomName, XTrigger.Attribute("name").Value));
                    if (!GetAttributeValue(XTrigger, "value", out int triggerValue))
                        throw new ArgumentException(string.Format("Room {0} Trigger {2} has incorrect value of attribete \'value\' = \'{1}\'", roomName, XTrigger.Attribute("value").Value, triggerName));

                    triggers.Add(triggerName, triggerValue);
                }
            }
        }

        return new Room(roomName, roomType, story.ToArray(), annotations.ToArray(), events.ToArray(), triggers);
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

    public static Action GetAction(string actionName)
    {
        Initialize();
        XElement root = ReadXml(fAction);
        XElement xmlAction = root.Elements("action").ToList().Find(l =>
        {
            if (!GetAttributeValue(l, "name", out string name))
                throw new ArgumentException(string.Format("Action has incorrect value of attribute \'name\'."));
            return name == actionName;
        });

        if (xmlAction == null) return null;

        if (!GetAttributeValue(xmlAction, "type", out int type))
            throw new ArgumentException(string.Format("Action \'{0}\' has incorrect value of attribute \'type\'.", actionName));

        if (!GetAttributeValue(xmlAction, "rarity", out int rarity))
            throw new ArgumentException(string.Format("Action \'{0}\' has incorrect value of attribute \'rarity\'.", actionName));

        if (!GetAttributeValue(xmlAction, "value", out int value))
            throw new ArgumentException(string.Format("Action \'{0}\' has incorrect value of attribute \'value\'.", actionName));

        return new Action(actionName, (ActionType)type, (ActionRarity)rarity, value);
    }

    public static bool GetValue(XElement element, out int value)
    {
        if (element != null && !element.HasElements && int.TryParse(element.Value, out value))
            return true;
        value = -1;
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
        value = -1;
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

    public static bool GetAttributeValue(XAttribute attribute, out bool value)
    {
        if (attribute != null && bool.TryParse(attribute.Value, out value))
            return true;
        value = false;
        return false;
    }

    public static bool GetAttributeValue(XElement element, string attrName, out int value)
    {
        if (element != null && element.Attribute(attrName) != null &&
            int.TryParse(element.Attribute(attrName).Value, out value))
            return true;
        value = -1;
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

    public static bool GetAttributeValue(XElement element, string attrName, out bool value)
    {
        if (element != null && element.Attribute(attrName) != null &&
            bool.TryParse(element.Attribute(attrName).Value, out value))
            return true;
        value = false;
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
    public Dictionary<Action, int> actions;

    public Hero()
    {
        this.name = "Nameless";
        this.hp = 100;
        this.level = 1;
        this.money = 0;
        this.items = new Dictionary<Item, int>();
        this.actions = new Dictionary<Action, int>();
    }

    public Hero(string name)
    {
        this.name = name;
        this.hp = 100;
        this.level = 1;
        this.money = 0;
        this.items = new Dictionary<Item, int>();
        this.actions = new Dictionary<Action, int>();
    }

    public Hero(string name, int hp, int level, int money, Dictionary<Item, int> items, Dictionary<Action, int> actions)
    {
        this.name = name;
        this.hp = hp;
        this.level = level;
        this.money = money;
        this.items = new Dictionary<Item, int>(items);
        this.actions = new Dictionary<Action, int>(actions);
    }

    public Hero(Hero other) : this(other.name, other.hp, other.level, other.money, other.items, other.actions)
    { }

    public void AddItem(Item item, int count)
    {
        if (items.ContainsKey(item))
            items[item] += count;
        else
            items.Add(item, count);
    }

    public void AddAction(Action action, int count)
    {
        if (actions.ContainsKey(action))
            actions[action] += count;
        else
            actions.Add(action, count);
    }

    public void SubtractItem(Item item, int count)
    {
        if (items.ContainsKey(item))
            items[item] -= count;
    }

    public void SubtractAction(Action action, int count)
    {
        if (actions.ContainsKey(action))
            actions[action] -= count;
    }

    public override string ToString()
    {
        return string.Format("Name:{0} Items:[{1}] Actions:{2}", name, items.ToString(), actions.ToString());
    }

    public override bool Equals(object obj)
    {
        return obj is Hero hero &&
               name == hero.name &&
               EqualityComparer<Dictionary<Item, int>>.Default.Equals(items, hero.items) &&
               EqualityComparer<Dictionary<Action, int>>.Default.Equals(actions, hero.actions);
    }

    public override int GetHashCode()
    {
        var hashCode = -826629381;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
        hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<Item, int>>.Default.GetHashCode(items);
        hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<Action, int>>.Default.GetHashCode(actions);
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
        Dictionary<Action, int> actions = new Dictionary<Action, int>();

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

        foreach (XElement xmlAction in element.Elements("spell"))
        {
            if (!IOHelper.GetAttributeValue(xmlAction, "name", out string actionName))
                throw new ArgumentException(string.Format("Hero \'{0}\' has Action with incorrect value of attribute \'name\'", name));

            if (!IOHelper.GetAttributeValue(xmlAction, "count", out int count))
                throw new ArgumentException(string.Format("Hero \'{0}\' has Action \'{1}\' with incorrect value of attribute \'count\'", name, actionName));

            Action action = IOHelper.GetAction(actionName);
            if (action == null) continue;
            actions.Add(action, count);
        }

        return new Hero(name, hp, level, money, items, actions);
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

        foreach (var spell in hero.actions)
        {
            XElement xmlSpell = new XElement("action");
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
               Star == item.Star;
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
