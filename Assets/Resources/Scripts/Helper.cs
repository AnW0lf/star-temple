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


    //public Hero CreateHero(string heroName)
    //{
    //    XElement root;
    //    if (!File.Exists(heroPath))
    //        root = new XElement("root");
    //    else
    //        root = XDocument.Parse(File.ReadAllText(heroPath)).Element("root");

    //    XElement hero = new XElement("hero");

    //    hero.Add(new XAttribute("name", heroName));
    //    hero.Add(new XAttribute("level", 1));
    //    hero.Add(new XAttribute("money", 0));
    //    hero.Add(new XAttribute("strength", 10));
    //    hero.Add(new XAttribute("persistence", 10));
    //    hero.Add(new XAttribute("agility", 10));
    //    hero.Add(new XAttribute("attention", 10));

    //    root.Add(hero);

    //    XDocument doc = new XDocument(root);
    //    File.WriteAllText(heroPath, doc.ToString());

    //    return LoadHero(heroName);
    //}

    //public void SaveHero(Hero hero)
    //{
    //    XElement root = XDocument.Parse(File.ReadAllText(heroPath)).Element("root");
    //    XElement XHero = root.Elements("hero").ToList().Find(h => h.Attribute("name").Value.Equals(hero.name));
    //    if (XHero == null) return;
    //    XHero.Attribute("hp").SetValue(hero.hp);
    //    XHero.Attribute("level").SetValue(hero.level);
    //    XHero.Attribute("money").SetValue(hero.money);
    //    XHero.Attribute("strength").SetValue(hero.strength);
    //    XHero.Attribute("persistence").SetValue(hero.persistence);
    //    XHero.Attribute("agility").SetValue(hero.agility);
    //    XHero.Attribute("attention").SetValue(hero.attention);

    //    var XItems = XHero.Elements("item");

    //    hero.items.ForEach(item =>
    //    {
    //        XElement XItem = null;
    //        if ((XItem = XItems.ToList().Find(xi => xi.Attribute("name").Value.Equals(item.Name))) != null)
    //        {
    //            XItem.Attribute("count").SetValue(item.Count);
    //        }
    //        else
    //        {
    //            XItem = new XElement("item");
    //            XItem.Add(new XAttribute("name", item.Name));
    //            XItem.Add(new XAttribute("count", item.Count));
    //            XHero.Add(XItem);
    //        }
    //    });

    //    XDocument doc = new XDocument(root);
    //    File.WriteAllText(heroPath, doc.ToString());
    //}

    //public Hero LoadHero(string heroName)
    //{
    //    XElement root = XDocument.Parse(File.ReadAllText(heroPath)).Element("root");

    //    foreach (XElement hero in root.Elements("hero").Where(l => l.Attribute("name").Value.Equals(heroName)))
    //    {
    //        int hp = int.Parse(hero.Attribute("hp").Value);
    //        int level = int.Parse(hero.Attribute("level").Value);
    //        int money = int.Parse(hero.Attribute("money").Value);
    //        int strength = int.Parse(hero.Attribute("strength").Value);
    //        int persistence = int.Parse(hero.Attribute("persistence").Value);
    //        int agility = int.Parse(hero.Attribute("agility").Value);
    //        int attention = int.Parse(hero.Attribute("attention").Value);
    //        List<Item> items = new List<Item>();

    //        foreach (XElement XItem in hero.Elements("item"))
    //        {
    //            Item item = GetItem(XItem.Attribute("name").Value);
    //            if (!int.TryParse(XItem.Attribute("count").Value, out int count))
    //                throw new ArgumentException(string.Format("Item \'{0}\' has incorrect value \'{1}\' of star count."
    //                    , item.Name, XItem.Attribute("count").Value));
    //            item.SetCount(count);

    //            items.Add(item);
    //        }

    //        return new Hero(heroName, hp, level, money, strength, persistence, agility, attention, items);
    //    }

    //    return Hero.Empty(heroName);
    //}

    //public Hero[] LoadHeroes()
    //{
    //    XElement root = XDocument.Parse(File.ReadAllText(heroPath)).Element("root");
    //    List<Hero> heroes = new List<Hero>();
    //    foreach (XElement hero in root.Elements("hero"))
    //    {
    //        string name = hero.Attribute("name").Value;
    //        int hp = int.Parse(hero.Attribute("hp").Value);
    //        int level = int.Parse(hero.Attribute("level").Value);
    //        int money = int.Parse(hero.Attribute("money").Value);
    //        int strength = int.Parse(hero.Attribute("strength").Value);
    //        int persistence = int.Parse(hero.Attribute("persistence").Value);
    //        int agility = int.Parse(hero.Attribute("agility").Value);
    //        int attention = int.Parse(hero.Attribute("attention").Value);
    //        List<Item> items = new List<Item>();

    //        foreach (XElement XItem in hero.Elements("item"))
    //        {
    //            Item item = GetItem(XItem.Attribute("name").Value);
    //            if (!int.TryParse(XItem.Attribute("count").Value, out int count))
    //                throw new ArgumentException(string.Format("Item \'{0}\' has incorrect value \'{1}\' of star count."
    //                    , item.Name, XItem.Attribute("count").Value));
    //            item.SetCount(count);

    //            items.Add(item);
    //        }

    //        heroes.Add(new Hero(name, hp, level, money, strength, persistence, agility, attention, items));
    //    }

    //    return heroes.ToArray();
    //}

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

    //public Room LoadRoom(string roomName)
    //{
    //    XElement room = null;

    //    string path = string.Format(roomPath, roomName);

    //    if (!File.Exists(path))
    //        throw new FileNotFoundException(string.Format("File {0} not found. Check it and try again.", path));

    //    room = XDocument.Parse(File.ReadAllText(path)).Root;

    //    string room_name = "";

    //    if (room.Attribute("name") == null)
    //        throw new ArgumentException(string.Format("Room does not contains attribute \'{0}\'.", "name"));

    //    room_name = room.Attribute("name").Value;

    //    RoomType room_type = RoomType.EMPTY;

    //    if (room.Attribute("type") == null)
    //        throw new ArgumentException(string.Format("Room does not contains attribute \'{0}\'.", "type"));

    //    switch (room.Attribute("type").Value)
    //    {
    //        case "common":
    //            room_type = RoomType.COMMON;
    //            break;
    //        case "before_battle":
    //            room_type = RoomType.BEFORE_BATTLE;
    //            break;
    //        case "battle":
    //            room_type = RoomType.BATTLE;
    //            break;
    //        default:
    //            room_type = RoomType.EMPTY;
    //            break;
    //    }

    //    // TODO
    //    /*
    //    if(room_type == RoomType.EMPTY)
    //        throw new ArgumentException(string.Format("Room has incorrect value of attribute \'{0}\' = \'{1}\'.", "type", room.Attribute("type").Value));
    //    */

    //    List<StoryWord> story = new List<StoryWord>();
    //    List<Annotation> annotations = new List<Annotation>();
    //    List<XmlEvent> events = new List<XmlEvent>();

    //    //load story
    //    {
    //        if (room.Element("story") != null)
    //        {
    //            foreach (XElement XWord in room.Element("story").Elements("word"))
    //            {
    //                StoryWord word = new StoryWord((XWord.Value.Length == 0 ? " " : XWord.Value));

    //                if (XWord.Attribute("annotation") != null && !int.TryParse(XWord.Attribute("annotation").Value, out word.annotation_id))
    //                    throw new ArgumentException(string.Format("Story word \"{0}\" attribute \'{1}\' contains incorrect value : {2}.",
    //                        XWord.Value, "annotation", XWord.Attribute("annotation").Value));

    //                if (XWord.Attribute("event") != null && !int.TryParse(XWord.Attribute("event").Value, out word.event_id))
    //                    throw new ArgumentException(string.Format("Story word \"{0}\" attribute \'{1}\' contains incorrect value : {2}.",
    //                        XWord.Value, "event", XWord.Attribute("event").Value));

    //                if (XWord.Attribute("event_reusable") != null && !bool.TryParse(XWord.Attribute("event_reusable").Value, out word.event_reusable))
    //                    throw new ArgumentException(string.Format("Story word \"{0}\" attribute \'{1}\' contains incorrect value : {2}.",
    //                        XWord.Value, "event_reusable", XWord.Attribute("event_reusable").Value));

    //                if (XWord.Attribute("drop_type") != null && XWord.Attribute("drop") != null)
    //                {
    //                    word.drop_type = XWord.Attribute("drop_type").Value;
    //                    if (!int.TryParse(XWord.Attribute("drop").Value, out word.drop_id))
    //                        throw new ArgumentException(string.Format("Story word \"{0}\" attribute \'{1}\' contains incorrect value : {2}.",
    //                        XWord.Value, "drop", XWord.Attribute("drop").Value));
    //                }

    //                if (XWord.Attribute("drop_reusable") != null && !bool.TryParse(XWord.Attribute("drop_reusable").Value, out word.drop_reusable))
    //                    throw new ArgumentException(string.Format("Story word \"{0}\" attribute \'{1}\' contains incorrect value : {2}.",
    //                        XWord.Value, "drop_reusable", XWord.Attribute("drop_reusable").Value));

    //                story.Add(word);
    //            }
    //        }
    //    }

    //    //load annotations
    //    {
    //        if (room.Element("annotations") != null)
    //        {
    //            foreach (XElement XAnnotation in room.Element("annotations").Elements("annotation"))
    //            {
    //                int id = -1;

    //                if (XAnnotation.Attribute("id") == null)
    //                    throw new ArgumentException(string.Format("Annotation does not contains attribute \'{0}\'.", "id"));

    //                if (!int.TryParse(XAnnotation.Attribute("id").Value, out id))
    //                    throw new ArgumentException(string.Format("Annotation has incorrect value of attribute \'{0}\' = \'{1}\'.",
    //                        "id", XAnnotation.Attribute("id").Value));

    //                if (XAnnotation.Attribute("chance") == null)
    //                    throw new ArgumentException(string.Format("Annotation does not contains attribute \'{0}\'.", "chance"));

    //                if (!int.TryParse(XAnnotation.Attribute("chance").Value, out int chance))
    //                    throw new ArgumentException(string.Format("Annotation has incorrect value of attribute \'{0}\' = \'{1}\'.",
    //                        "chance", XAnnotation.Attribute("chance").Value));

    //                Annotation annotation = new Annotation(id, chance);

    //                foreach (XElement XWord in XAnnotation.Elements("word"))
    //                {
    //                    AnnotationWord word = new AnnotationWord((XWord.Value.Length == 0 ? " " : XWord.Value));

    //                    if (XWord.Attribute("event") != null && !int.TryParse(XWord.Attribute("event").Value, out word.event_id))
    //                        throw new ArgumentException(string.Format("Annotation word \"{0}\" attribute \'{1}\' contains incorrect value : {2}.",
    //                            XWord.Value, "event", XWord.Attribute("event").Value));

    //                    if (XWord.Attribute("event_reusable") != null && !bool.TryParse(XWord.Attribute("event_reusable").Value, out word.event_reusable))
    //                        throw new ArgumentException(string.Format("Annotation word \"{0}\" attribute \'{1}\' contains incorrect value : {2}.",
    //                            XWord.Value, "event_reusable", XWord.Attribute("event_reusable").Value));

    //                    if (XWord.Attribute("drop_type") != null && XWord.Attribute("drop") != null)
    //                    {
    //                        word.drop_type = XWord.Attribute("drop_type").Value;
    //                        if (!int.TryParse(XWord.Attribute("drop").Value, out word.drop_id))
    //                            throw new ArgumentException(string.Format("Story word \"{0}\" attribute \'{1}\' contains incorrect value : {2}.",
    //                            XWord.Value, "drop", XWord.Attribute("drop").Value));
    //                    }

    //                    if (XWord.Attribute("drop_reusable") != null && !bool.TryParse(XWord.Attribute("drop_reusable").Value, out word.drop_reusable))
    //                        throw new ArgumentException(string.Format("Annotation word \"{0}\" attribute \'{1}\' contains incorrect value : {2}.",
    //                            XWord.Value, "drop_reusable", XWord.Attribute("drop_reusable").Value));

    //                    annotation.words.Add(word);
    //                }
    //                annotations.Add(annotation);
    //            }
    //        }
    //    }

    //    //load events
    //    {
    //        if (room.Element("events") != null)
    //        {
    //            foreach (XElement XEvent in room.Element("events").Elements("event"))
    //            {
    //                int event_id = -1;
    //                EventType event_type = EventType.EMPTY;

    //                if (XEvent.Attribute("event_id") == null)
    //                    throw new ArgumentException(string.Format("Event does not contains attribute \'{0}\'.", "event_id"));

    //                if (!int.TryParse(XEvent.Attribute("event_id").Value, out event_id))
    //                    throw new ArgumentException(string.Format("Event has incorrect value of attribute \'{0}\' = \'{1}\'.",
    //                        "event_id", XEvent.Attribute("event_id").Value));

    //                if (XEvent.Attribute("event_type") == null)
    //                    throw new ArgumentException(string.Format("Event does not contains attribute \'{0}\'. Event id = {1}", "event_type", event_id));

    //                if ((event_type = XmlEvent.ParseEventType(XEvent.Attribute("event_type").Value)) == EventType.EMPTY)
    //                    throw new ArgumentException(string.Format("Event has incorrect value of attribute \'{0}\' = \'{1}\'. Event id = {2}.",
    //                        "event_type", XEvent.Attribute("event_type").Value, event_id));

    //                XmlEvent @event = new XmlEvent(event_id, event_type);

    //                foreach (XAttribute attr in XEvent.Attributes().ToList().Where(a => a.Name != "event_id" && a.Name != "event_type"))
    //                {
    //                    if (@event.attributes.ContainsKey(attr.Name.ToString()))
    //                        throw new ArgumentException(string.Format("Event has too much attributes \'{0}\'. Event id = {1} type = {2}",
    //                        attr.Name, @event.event_id, @event.event_type));

    //                    @event.attributes.Add(attr.Name.ToString(), attr.Value);
    //                }

    //                events.Add(@event);
    //            }
    //        }
    //    }

    //    return new Room(room_name, room_type, story.ToArray(), annotations.ToArray(), events.ToArray());
    //}

    public string GetItemType(string name)
    {
        if (!File.Exists(itemPath))
            throw new FileNotFoundException(string.Format("File {0} not found. Check it and try again.", itemPath));

        XElement root = XDocument.Parse(File.ReadAllText(itemPath)).Root;
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
            if (!int.TryParse(XItem.Attribute("star").Value, out int count))
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
            foreach (XAttribute XAttr in XItem.Attributes())
            {
                switch (XAttr.Name.ToString())
                {
                    case "name":
                        item.SetName(XAttr.Value);
                        break;
                    case "type":
                        item.SetType(XAttr.Value);
                        break;
                    case "star":
                        int star;
                        if (!int.TryParse(XAttr.Value, out star))
                            throw new ArgumentException(string.Format("Item \'{0}\' has incorrect value \'{1}\' of star count."
                                , name, XAttr.Value));
                        item.SetStar(star);
                        break;
                    default:
                        item.AddOption(XAttr.Name.ToString(), XAttr.Value);
                        break;
                }
            }
            return item;
        }

        return new Item();
    }
}
