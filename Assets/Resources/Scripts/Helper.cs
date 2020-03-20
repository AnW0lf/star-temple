﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

public class Helper : MonoBehaviour
{
    public static Helper Instance = null;

    public static readonly Item Star = new Item("*", 0);

    private string directoryPath, heroPath, itemPath, roomPath;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance == this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

#if UNITY_EDITOR
        directoryPath = Application.dataPath + @"/Resources/XML";
#elif UNITY_ANDROID || UNITY_IOS
		directoryPath = Application.persistentDataPath;
#endif
        heroPath = directoryPath + @"/hero.xml";
        itemPath = directoryPath + @"/item.xml";
        roomPath = directoryPath + "/{0}.xml";
    }

    public Hero CreateHero(string heroName)
    {
        XElement root = null;

        if (!File.Exists(heroPath))
            root = new XElement("root");
        else
            root = XDocument.Parse(File.ReadAllText(heroPath)).Element("root");

        XElement hero = new XElement("hero");

        hero.Add(new XAttribute("name", heroName));
        hero.Add(new XAttribute("level", 1));
        hero.Add(new XAttribute("money", 0));
        hero.Add(new XAttribute("strength", 10));
        hero.Add(new XAttribute("persistence", 10));
        hero.Add(new XAttribute("agility", 10));
        hero.Add(new XAttribute("attention", 10));

        root.Add(hero);

        XDocument doc = new XDocument(root);
        File.WriteAllText(heroPath, doc.ToString());

        return LoadHero(heroName);
    }

    public void SaveHero(Hero hero)
    {
        XElement root = XDocument.Parse(File.ReadAllText(heroPath)).Element("root");
        XElement XHero = root.Elements("hero").ToList().Find(h => h.Attribute("name").Value.Equals(hero.name));
        if (XHero == null) return;
        XHero.Attribute("hp").SetValue(hero.hp);
        XHero.Attribute("level").SetValue(hero.level);
        XHero.Attribute("money").SetValue(hero.money);
        XHero.Attribute("strength").SetValue(hero.strength);
        XHero.Attribute("persistence").SetValue(hero.persistence);
        XHero.Attribute("agility").SetValue(hero.agility);
        XHero.Attribute("attention").SetValue(hero.attention);

        var XItems = XHero.Elements("item");

        hero.items.ForEach(item =>
        {
            XElement XItem = null;
            if ((XItem = XItems.ToList().Find(xi => xi.Attribute("name").Value.Equals(item.Name))) != null)
            {
                XItem.Attribute("count").SetValue(item.Count);
            }
            else
            {
                XItem = new XElement("item");
                XItem.Add(new XAttribute("name", item.Name));
                XItem.Add(new XAttribute("count", item.Count));
                XHero.Add(XItem);
            }
        });

        XDocument doc = new XDocument(root);
        File.WriteAllText(heroPath, doc.ToString());
    }

    public Hero LoadHero(string heroName)
    {
        XElement root = XDocument.Parse(File.ReadAllText(heroPath)).Element("root");

        foreach (XElement hero in root.Elements("hero").Where(l => l.Attribute("name").Value.Equals(heroName)))
        {
            int hp = int.Parse(hero.Attribute("hp").Value);
            int level = int.Parse(hero.Attribute("level").Value);
            int money = int.Parse(hero.Attribute("money").Value);
            int strength = int.Parse(hero.Attribute("strength").Value);
            int persistence = int.Parse(hero.Attribute("persistence").Value);
            int agility = int.Parse(hero.Attribute("agility").Value);
            int attention = int.Parse(hero.Attribute("attention").Value);
            List<Item> items = new List<Item>();

            foreach (XElement item in hero.Elements("item"))
            {
                items.Add(new Item(item.Attribute("name").Value, int.Parse(item.Attribute("count").Value)));
            }

            return new Hero(heroName, hp, level, money, strength, persistence, agility, attention, items);
        }

        return Hero.Empty(heroName);
    }

    public Hero[] LoadHeroes()
    {
        XElement root = XDocument.Parse(File.ReadAllText(heroPath)).Element("root");
        List<Hero> heroes = new List<Hero>();
        foreach (XElement hero in root.Elements("hero"))
        {
            string name = hero.Attribute("name").Value;
            int hp = int.Parse(hero.Attribute("hp").Value);
            int level = int.Parse(hero.Attribute("level").Value);
            int money = int.Parse(hero.Attribute("money").Value);
            int strength = int.Parse(hero.Attribute("strength").Value);
            int persistence = int.Parse(hero.Attribute("persistence").Value);
            int agility = int.Parse(hero.Attribute("agility").Value);
            int attention = int.Parse(hero.Attribute("attention").Value);
            List<Item> items = new List<Item>();

            foreach (XElement item in hero.Elements("item"))
            {
                items.Add(new Item(item.Attribute("name").Value, int.Parse(item.Attribute("count").Value)));
            }

            heroes.Add(new Hero(name, hp, level, money, strength, persistence, agility, attention, items));
        }

        return heroes.ToArray();
    }

    public bool ContainsHero(string heroName)
    {
        XElement root = XDocument.Parse(File.ReadAllText(heroPath)).Element("root");

        return root.Elements("hero").Where(l => l.Attribute("name").Value.Equals(heroName)).Count() != 0;
    }

    public void RemoveHero(string heroName)
    {
        XElement root = null;

        if (!File.Exists(heroPath))
            return;
        else
            root = XDocument.Parse(File.ReadAllText(heroPath)).Element("root");

        root.Elements("hero").Where(l => l.Attribute("name").Value.Equals(heroName)).Remove();

        XDocument doc = new XDocument(root);
        File.WriteAllText(heroPath, doc.ToString());
    }

    public Room LoadRoom(string roomName)
    {
        XElement room = null;

        string path = string.Format(roomPath, roomName);

        if (!File.Exists(path))
            throw new FileNotFoundException(string.Format("File {0} not found. Check it and try again.", path));

        room = XDocument.Parse(File.ReadAllText(path)).Root;

        string room_name = "";

        if (room.Attribute("name") == null)
            throw new ArgumentException(string.Format("Room does not contains attribute \'{0}\'.", "name"));

        room_name = room.Attribute("name").Value;

        RoomType room_type = RoomType.EMPTY;

        if (room.Attribute("type") == null)
            throw new ArgumentException(string.Format("Room does not contains attribute \'{0}\'.", "type"));

        switch (room.Attribute("type").Value)
        {
            case "common":
                room_type = RoomType.COMMON;
                break;
            case "before_battle":
                room_type = RoomType.BEFORE_BATTLE;
                break;
            case "battle":
                room_type = RoomType.BATTLE;
                break;
            default:
                room_type = RoomType.EMPTY;
                break;
        }

        // TODO
        /*
        if(room_type == RoomType.EMPTY)
            throw new ArgumentException(string.Format("Room has incorrect value of attribute \'{0}\' = \'{1}\'.", "type", room.Attribute("type").Value));
        */

        List<StoryWord> story = new List<StoryWord>();
        List<Annotation> annotations = new List<Annotation>();
        List<Event> events = new List<Event>();

        //load story
        {
            if (room.Element("story") != null)
            {
                foreach (XElement XWord in room.Element("story").Elements("word"))
                {
                    StoryWord word = new StoryWord((XWord.Value.Length == 0 ? " " : XWord.Value));

                    if (XWord.Attribute("annotation") != null && !int.TryParse(XWord.Attribute("annotation").Value, out word.annotation_id))
                        throw new ArgumentException(string.Format("Story word \"{0}\" attribute \'{1}\' contains incorrect value : {2}.",
                            XWord.Value, "annotation", XWord.Attribute("annotation").Value));

                    if (XWord.Attribute("event") != null && !int.TryParse(XWord.Attribute("event").Value, out word.event_id))
                        throw new ArgumentException(string.Format("Story word \"{0}\" attribute \'{1}\' contains incorrect value : {2}.",
                            XWord.Value, "event", XWord.Attribute("event").Value));

                    if (XWord.Attribute("event_reusable") != null && !bool.TryParse(XWord.Attribute("event_reusable").Value, out word.reusable_event))
                        throw new ArgumentException(string.Format("Story word \"{0}\" attribute \'{1}\' contains incorrect value : {2}.",
                            XWord.Value, "event_reusable", XWord.Attribute("event_reusable").Value));

                    if (XWord.Attribute("drop_type") != null && XWord.Attribute("drop") != null)
                    {
                        word.drop_type = XWord.Attribute("drop_type").Value;
                        if (!int.TryParse(XWord.Attribute("drop").Value, out word.drop_id))
                            throw new ArgumentException(string.Format("Story word \"{0}\" attribute \'{1}\' contains incorrect value : {2}.",
                            XWord.Value, "drop", XWord.Attribute("drop").Value));
                    }

                    if (XWord.Attribute("drop_reusable") != null && !bool.TryParse(XWord.Attribute("drop_reusable").Value, out word.reusable_drop))
                        throw new ArgumentException(string.Format("Story word \"{0}\" attribute \'{1}\' contains incorrect value : {2}.",
                            XWord.Value, "drop_reusable", XWord.Attribute("drop_reusable").Value));

                    story.Add(word);
                }
            }
        }

        //load annotations
        {
            if (room.Element("annotations") != null)
            {
                foreach (XElement XAnnotation in room.Element("annotations").Elements("annotation"))
                {
                    int id = -1, chance = 0;

                    if (XAnnotation.Attribute("id") == null)
                        throw new ArgumentException(string.Format("Annotation does not contains attribute \'{0}\'.", "id"));

                    if (!int.TryParse(XAnnotation.Attribute("id").Value, out id))
                        throw new ArgumentException(string.Format("Annotation has incorrect value of attribute \'{0}\' = \'{1}\'.",
                            "id", XAnnotation.Attribute("id").Value));

                    if (XAnnotation.Attribute("chance") == null)
                        throw new ArgumentException(string.Format("Annotation does not contains attribute \'{0}\'.", "chance"));

                    if (!int.TryParse(XAnnotation.Attribute("chance").Value, out chance))
                        throw new ArgumentException(string.Format("Annotation has incorrect value of attribute \'{0}\' = \'{1}\'.",
                            "chance", XAnnotation.Attribute("chance").Value));

                    Annotation annotation = new Annotation(id, chance);

                    foreach (XElement XWord in XAnnotation.Elements("word"))
                    {
                        AnnotationWord word = new AnnotationWord((XWord.Value.Length == 0 ? " " : XWord.Value));

                        if (XWord.Attribute("event") != null && !int.TryParse(XWord.Attribute("event").Value, out word.event_id))
                            throw new ArgumentException(string.Format("Annotation word \"{0}\" attribute \'{1}\' contains incorrect value : {2}.",
                                XWord.Value, "event", XWord.Attribute("event").Value));

                        if (XWord.Attribute("event_reusable") != null && !bool.TryParse(XWord.Attribute("event_reusable").Value, out word.reusable_event))
                            throw new ArgumentException(string.Format("Annotation word \"{0}\" attribute \'{1}\' contains incorrect value : {2}.",
                                XWord.Value, "event_reusable", XWord.Attribute("event_reusable").Value));

                        if (XWord.Attribute("drop_type") != null && XWord.Attribute("drop") != null)
                        {
                            word.drop_type = XWord.Attribute("drop_type").Value;
                            if (!int.TryParse(XWord.Attribute("drop").Value, out word.drop_id))
                                throw new ArgumentException(string.Format("Story word \"{0}\" attribute \'{1}\' contains incorrect value : {2}.",
                                XWord.Value, "drop", XWord.Attribute("drop").Value));
                        }

                        if (XWord.Attribute("drop_reusable") != null && !bool.TryParse(XWord.Attribute("drop_reusable").Value, out word.reusable_drop))
                            throw new ArgumentException(string.Format("Annotation word \"{0}\" attribute \'{1}\' contains incorrect value : {2}.",
                                XWord.Value, "drop_reusable", XWord.Attribute("drop_reusable").Value));

                        annotation.words.Add(word);
                    }
                    annotations.Add(annotation);
                }
            }
        }

        //load events
        {
            if (room.Element("events") != null)
            {
                foreach (XElement XEvent in room.Element("events").Elements("event"))
                {
                    int event_id = -1;
                    EventType event_type = EventType.EMPTY;

                    if (XEvent.Attribute("event_id") == null)
                        throw new ArgumentException(string.Format("Event does not contains attribute \'{0}\'.", "event_id"));

                    if (!int.TryParse(XEvent.Attribute("event_id").Value, out event_id))
                        throw new ArgumentException(string.Format("Event has incorrect value of attribute \'{0}\' = \'{1}\'.",
                            "event_id", XEvent.Attribute("event_id").Value));

                    if (XEvent.Attribute("event_type") == null)
                        throw new ArgumentException(string.Format("Event does not contains attribute \'{0}\'. Event id = {1}", "event_type", event_id));

                    if ((event_type = Event.ParseEventType(XEvent.Attribute("event_type").Value)) == EventType.EMPTY)
                        throw new ArgumentException(string.Format("Event has incorrect value of attribute \'{0}\' = \'{1}\'. Event id = {2}.",
                            "event_type", XEvent.Attribute("event_type").Value, event_id));

                    Event @event = new Event(event_id, event_type);

                    foreach (XAttribute attr in XEvent.Attributes().ToList().Where(a => a.Name != "event_id" && a.Name != "event_type"))
                    {
                        if (@event.attributes.ContainsKey(attr.Name.ToString()))
                            throw new ArgumentException(string.Format("Event has too much attributes \'{0}\'. Event id = {1} type = {2}",
                            attr.Name, @event.event_id, @event.event_type));

                        @event.attributes.Add(attr.Name.ToString(), attr.Value);
                    }

                    events.Add(@event);
                }
            }
        }

        return new Room(room_name, room_type, story.ToArray(), annotations.ToArray(), events.ToArray());
    }

    public string GetItemType(string name)
    {
        XElement root = null;

        if (!File.Exists(itemPath))
            throw new FileNotFoundException(string.Format("File {0} not found. Check it and try again.", itemPath));

        root = XDocument.Parse(File.ReadAllText(itemPath)).Root;

        string type = "";

        foreach (XElement item in root.Elements("item"))
        {
            if (item.Attribute("name") != null && item.Attribute("name").Value == name)
            {
                if (item.Attribute("type") != null) type = item.Attribute("type").Value;
                return type;
            }
        }

        return type;
    }

    public int GetItemValue(string name)
    {
        if (!File.Exists(itemPath))
            throw new FileNotFoundException(string.Format("File {0} not found. Check it and try again.", itemPath));

        XElement root = XDocument.Parse(File.ReadAllText(itemPath)).Root;

        int value = 0;

        XElement XItem = root.Elements("item").ToList().Find(item => item.Attribute("name").Value == name);
        if (XItem != null && XItem.Attribute("value") != null)
        {
            if (!int.TryParse(XItem.Attribute("value").Value, out value))
                throw new ArgumentException(string.Format("Item {0} has incorrect type of attribute \'{1}\' = {2}",
                    XItem.Attribute("name").Value, "value", XItem.Attribute("value").Value));
        }

        return value;
    }

    public string GetItemDescription(string name)
    {
        if (!File.Exists(itemPath))
            throw new FileNotFoundException(string.Format("File {0} not found. Check it and try again.", itemPath));

        XElement root = XDocument.Parse(File.ReadAllText(itemPath)).Root;
        XElement XItem = root.Elements("item").ToList().Find(item => item.Attribute("name").Value == name);
        if (XItem != null)
            return XItem.Value;

        return "Description not found";
    }

    public int GetItemStarCount(string name)
    {
        if (!File.Exists(itemPath))
            throw new FileNotFoundException(string.Format("File {0} not found. Check it and try again.", itemPath));

        XElement root = XDocument.Parse(File.ReadAllText(itemPath)).Root;
        XElement XItem = root.Elements("item").ToList().Find(item => item.Attribute("name").Value == name);
        if (XItem != null && XItem.Attribute("star") != null)
        {
            int count = 0;
            if (!int.TryParse(XItem.Attribute("star").Value, out count))
                throw new ArgumentException(string.Format("Item \'{0}\' has incorrect value \'{1}\' of star count."
                    , name, XItem.Attribute("star").Value));

            return count;
        }

        return 0;
    }

    public Item GetItem(string name)
    {
        if (!File.Exists(itemPath))
            throw new FileNotFoundException(string.Format("File {0} not found. Check it and try again.", itemPath));

        XElement root = XDocument.Parse(File.ReadAllText(itemPath)).Root;
        XElement XItem = root.Elements("item").ToList().Find(item => item.Attribute("name").Value == name);
        if (XItem != null)
        {
            Item item = new Item();
            if (!int.TryParse(XItem.Attribute("star").Value, out count))
                throw new ArgumentException(string.Format("Item \'{0}\' has incorrect value \'{1}\' of star count."
                    , name, XItem.Attribute("star").Value));

            return count;
        }

        return 0;
    }
}

public struct Hero
{
    public string name;
    public int hp;
    public int level;
    public int money;
    public int strength;
    public int persistence;
    public int agility;
    public int attention;
    public List<Item> items;

    public Hero(string name, int hp, int level, int money, int strength, int persistence, int agility, int attention, List<Item> items)
    {
        this.name = name;
        this.hp = hp;
        this.level = level;
        this.money = money;
        this.strength = strength;
        this.persistence = persistence;
        this.agility = agility;
        this.attention = attention;
        this.items = items;
    }

    public static Hero Empty(string name)
    {
        return new Hero(name, 0, 1, 0, 10, 10, 10, 10, new List<Item>());
    }

    public static string Convert(int value)
    {
        int d = Mathf.Clamp(value, 0, 100) / 25;
        switch (d)
        {
            case 0: return "no";
            case 1: return "almost no";
            case 2: return "have some";
            case 3: return "enough";
            case 4: return "a lot of";
            default: return "unknown";
        }
    }
}

public class Item
{
    public string Name { get; private set; } = "";
    public string Type { get; private set; } = "";
    public int Count { get; private set; } = 0;
    public int Star { get; private set; } = 0;
    public Dictionary<string, string> Options { get; private set; }

    public Item()
    {
        Options = new Dictionary<string, string>();
    }

    public Item(Item other)
    {
        Name = other.Name;
        Type = other.Type;
        Count = other.Count;
        Star = other.Star;
        Options = new Dictionary<string, string>(other.Options);
    }

    public Item(string name, string type, int count, int star)
    {
        Name = name;
        Type = type;
        Count = count;
        Star = star;
        Options = new Dictionary<string, string>();
    }

    public void SetName(string name)
    {
        Name = name;
    }

    public void SetType(string type)
    {
        Type = type;
    }

    public void SetCount(int count)
    {
        Count = count;
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
}

public enum RoomType { EMPTY, COMMON, BEFORE_BATTLE, BATTLE }

public struct StoryWord
{
    public string word, drop_type;
    public int annotation_id, event_id, drop_id;
    public bool reusable_event, reusable_drop;

    public StoryWord(string word)
    {
        this.word = word;
        drop_type = "";
        annotation_id = -1;
        event_id = -1;
        drop_id = -1;
        reusable_event = false;
        reusable_drop = false;
    }

    public static StoryWord Empty
    {
        get
        {
            return new StoryWord("");
        }
    }
}

public struct AnnotationWord
{
    public string word, drop_type;
    public int event_id, drop_id;
    public bool reusable_event, reusable_drop;

    public AnnotationWord(string word)
    {
        this.word = word;
        event_id = -1;
        drop_type = "";
        drop_id = -1;
        reusable_event = false;
        reusable_drop = false;
    }

    public static AnnotationWord Empty
    {
        get
        {
            return new AnnotationWord("");
        }
    }
}

public struct Annotation
{
    public int id, chance;
    public List<AnnotationWord> words;

    public Annotation(int id, int chance)
    {
        this.id = id;
        this.chance = chance;
        this.words = new List<AnnotationWord>();
    }

    public static Annotation Empty
    {
        get
        {
            return new Annotation(-1, 0);
        }
    }
}

public struct Room
{
    public string name;
    public RoomType type;
    public StoryWord[] story;
    public Annotation[] annotations;
    public Event[] events;

    public Room(string name, RoomType type, StoryWord[] story, Annotation[] annotations, Event[] events)
    {
        this.name = name;
        this.type = type;
        this.story = story;
        this.annotations = annotations;
        this.events = events;
    }

    public static Room Empty
    {
        get
        {
            return new Room("", RoomType.EMPTY, new StoryWord[0], new Annotation[0], new Event[0]);
        }
    }
}
