using System;
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

    private string directoryPath, heroPath;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance == this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        directoryPath = Application.dataPath + @"/Resources/XML";
        heroPath = directoryPath + @"/hero.xml";
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

    public Hero LoadHero(string heroName)
    {
        XElement root = XDocument.Parse(File.ReadAllText(heroPath)).Element("root");

        foreach (XElement hero in root.Elements("hero").Where(l => l.Attribute("name").Value.Equals(heroName)))
        {
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

            return new Hero(heroName, level, money, strength, persistence, agility, attention, items);
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

            heroes.Add(new Hero(name, level, money, strength, persistence, agility, attention, items));
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

        string path = directoryPath + @"/" + roomName + @".xml";

        if (!File.Exists(path))
            throw new FileNotFoundException(string.Format("File {1} not found. Check it and try again."));

        room = XDocument.Parse(File.ReadAllText(path)).Element("room");

        List<StoryWord> story = new List<StoryWord>();
        List<Annotation> annotations = new List<Annotation>();

        room.Element("story").Elements("word").ToList().ForEach(XWord =>
        {
            WordType storyWordType = WordType.EMPTY;
            int annotation_id = -1, event_id = -1;
            switch (XWord.Attribute("type").Value)
            {
                case "regular":
                    storyWordType = WordType.REGULAR;
                    annotation_id = int.Parse(XWord.Attribute("annotation").Value);
                    break;
                case "separator":
                    storyWordType = WordType.SEPARATOR;
                    break;
                case "button":
                    storyWordType = WordType.BUTTON;
                    annotation_id = int.Parse(XWord.Attribute("annotation").Value);
                    event_id = int.Parse(XWord.Attribute("event").Value);
                    break;
            }
            story.Add(new StoryWord(XWord.Value, storyWordType, annotation_id, event_id));
        });

        room.Element("annotations").Elements("annotation").ToList().ForEach(XAnnotation => {
            int id = int.Parse(XAnnotation.Attribute("id").Value);
            List<AnnotationWord> words = new List<AnnotationWord>();
            XAnnotation.Elements("word").ToList().ForEach(XWord => {
                WordType annotationWordType = WordType.EMPTY;
                int event_id = -1;
                switch (XWord.Attribute("type").Value)
                {
                    case "regular":
                        annotationWordType = WordType.REGULAR;
                        break;
                    case "separator":
                        annotationWordType = WordType.SEPARATOR;
                        break;
                    case "button":
                        annotationWordType = WordType.BUTTON;
                        event_id = int.Parse(XWord.Attribute("event").Value);
                        break;
                }
                words.Add(new AnnotationWord(XWord.Value, annotationWordType, event_id));
            });
            annotations.Add(new Annotation(id, words.ToArray()));
        });

        StoryType storyType = StoryType.EMPTY;
        switch (room.Attribute("type").Value)
        {
            case "common":
                storyType = StoryType.COMMON;
                break;
            case "before_battle":
                storyType = StoryType.BEFORE_BATTLE;
                break;
            case "battle":
                storyType = StoryType.BATTLE;
                break;
        }

        return new Room(room.Attribute("name").Value, storyType, story.ToArray(), annotations.ToArray());
    }


}

public struct Hero
{
    public string name;
    public int level;
    public int money;
    public int strength;
    public int persistence;
    public int agility;
    public int attention;
    public List<Item> items;

    public Hero(string name, int level, int money, int strength, int persistence, int agility, int attention, List<Item> items)
    {
        this.name = name;
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
        return new Hero(name, 1, 0, 10, 10, 10, 10, new List<Item>());
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

public struct Item
{
    public string name;
    public int count;

    public Item(string name, int count)
    {
        this.name = name;
        this.count = count;
    }
}

public enum WordType { EMPTY, REGULAR, SEPARATOR, BUTTON }

public enum StoryType { EMPTY, COMMON, BEFORE_BATTLE, BATTLE }

public struct StoryWord
{
    public string word;
    public WordType type;
    public int annotation_id, event_id;

    public StoryWord(string word, WordType type, int annotation_id, int event_id)
    {
        this.word = word;
        this.type = type;
        this.annotation_id = annotation_id;
        this.event_id = event_id;
    }

    public static StoryWord Empty
    {
        get
        {
            return new StoryWord("", WordType.EMPTY, -1, -1);
        }
    }
}

public struct AnnotationWord
{
    public string word;
    public WordType type;
    public int event_id;

    public AnnotationWord(string word, WordType type, int event_id)
    {
        this.word = word;
        this.type = type;
        this.event_id = event_id;
    }

    public static AnnotationWord Empty
    {
        get
        {
            return new AnnotationWord("", WordType.EMPTY, -1);
        }
    }
}

public struct Annotation
{
    public int id;
    public AnnotationWord[] words;

    public Annotation(int id, AnnotationWord[] words)
    {
        this.id = id;
        this.words = words;
    }

    public static Annotation Empty
    {
        get
        {
            return new Annotation(-1, new AnnotationWord[0]);
        }
    }
}

public struct Room
{
    public string name;
    public StoryType type;
    public StoryWord[] story;
    public Annotation[] annotations;

    public Room(string name, StoryType type, StoryWord[] story, Annotation[] annotations)
    {
        this.name = name;
        this.type = type;
        this.story = story;
        this.annotations = annotations;
    }

    public static Room Empty
    {
        get
        {
            return new Room("", StoryType.EMPTY, new StoryWord[0], new Annotation[0]);
        }
    }
}
