﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventController : MonoBehaviour
{
    public GameController game;

    private Event[] events;
    private Dictionary<int, List<IDo>> executable = new Dictionary<int, List<IDo>>();
    public static EventController Instance { get; private set; } = null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public Event[] Events
    {
        get
        {
            return events;
        }
        set
        {
            events = value;
            CompyleEvents();
        }
    }

    private void CompyleEvents()
    {
        executable.Clear();
        int min = events.ToList().Min(e => e.event_id);
        int max = events.ToList().Max(e => e.event_id);
        for (int i = min; i <= max; i++)
        {
            List<Event> indexed = events.ToList().FindAll(e => e.event_id == i);
            List<Event> next = indexed.FindAll(e => e.event_type == EventType.NEXT);
            List<Event> stat = indexed.FindAll(e => e.event_type == EventType.STAT);
            List<Event> window = indexed.FindAll(e => e.event_type == EventType.WINDOW);

            executable.Add(i, new List<IDo>());
            if (next.Count > 0)
                executable[i].Add(new DoNext(i, next.First().room_name, game));

            {
                List<Event> item = indexed.FindAll(e => e.event_type == EventType.ITEM);
                if (item.Count > 0)
                {
                    item.Sort(delegate (Event x, Event y) { return x.item_name.CompareTo(y.item_name); });
                    List<ItemWithChance> itemWithChances = new List<ItemWithChance>();
                    Dictionary<int, int> dict = new Dictionary<int, int>();
                    string itemName = item.First().item_name;
                    for (int j = 0; j < item.Count; j++)
                    {
                        int itemCount = item[j].item_count;
                        int itemChance = item[j].item_chance;
                        if (itemName.Equals(item[j].item_name))
                        {
                            if (dict.ContainsKey(itemCount))
                            {
                                dict[itemCount] += itemChance;
                            }
                            else
                            {
                                dict.Add(itemCount, itemChance);
                            }

                            if (j == item.Count - 1)
                            {
                                itemWithChances.Add(new ItemWithChance(itemName, dict));
                            }
                        }
                        else
                        {
                            itemWithChances.Add(new ItemWithChance(itemName, dict));
                            dict.Clear();
                            itemName = item[j].item_name;
                            dict.Add(itemCount, itemChance);
                        }
                    }

                    itemWithChances.ForEach(itemWithChance => executable[i].Add(new DoItem(i, itemWithChance)));
                }
            }
        }
    }

    public void Execute(int id)
    {
        if (executable.ContainsKey(id))
        {
            executable[id].ForEach(e => e.Do());
            print(string.Format("Execute id = {0}", id));
        }
        else
        {
            Debug.LogWarning(string.Format("Not found event with id = {0}", id));
        }
    }
}

public enum EventType { EMPTY, NEXT, ITEM, WINDOW, STAT }
public enum StatType { EMPTY, STRENGTH, PERSISTENCE, AGILITY, ATTENTION }

public struct WindowWord
{
    public string word;

    public WindowWord(string word)
    {
        this.word = word;
    }
}

public struct Event
{
    public int event_id;
    public EventType event_type;
    public string room_name;
    public string item_name;
    public int item_count;
    public int item_chance;
    public StatType stat_type;
    public int stat_value;
    public int stat_id;
    public WindowWord[] window_words;

    public Event(int event_id, EventType event_type, string room_name, string item_name, int item_count, int item_chance, StatType stat_type, int stat_value, int stat_id, WindowWord[] window_words)
    {
        this.event_id = event_id;
        this.event_type = event_type;
        this.room_name = room_name;
        this.item_name = item_name;
        this.item_count = item_count;
        this.item_chance = item_chance;
        this.stat_type = stat_type;
        this.stat_value = stat_value;
        this.stat_id = stat_id;
        this.window_words = window_words;
    }

    public static Event Empty()
    {
        return new Event(-1, EventType.EMPTY, "", "", -1, -1, StatType.EMPTY, -1, -1, new WindowWord[0]);
    }
}

public struct ItemWithChance
{
    public string name;
    public Dictionary<int, int> dict;

    public ItemWithChance(string name, Dictionary<int, int> dict)
    {
        this.name = name;
        this.dict = dict;
    }
}

public struct StatWithChance
{
    public StatType type;
    public Dictionary<int, int> dict;

    public StatWithChance(StatType type, Dictionary<int, int> dict)
    {
        this.type = type;
        this.dict = dict;
    }
}

interface IDo
{
    int Id { get; set; }

    EventType event_type { get; }

    void Do();
}

public class DoNext : IDo
{
    private string roomName;
    private int id;
    private GameController game;

    public int Id { get => id; set => id = value; }

    public EventType event_type => EventType.NEXT;

    public DoNext(int id, string roomName, GameController game)
    {
        Id = id;
        this.roomName = roomName;
        this.game = game;
    }

    public void Do()
    {
        game.NextRoom(roomName);
    }
}

public class DoItem : IDo
{
    private int id;
    private ItemWithChance item;
    public int Id { get => id; set => id = value; }

    public EventType event_type => EventType.ITEM;

    public DoItem(int id, ItemWithChance item)
    {
        Id = id;
        this.item = item;
    }

    public void Do()
    {
        int ceiling = item.dict.Values.Sum() + 1;
        int chance = UnityEngine.Random.Range(0, ceiling);
        foreach (var pair in item.dict)
        {
            chance -= pair.Value;
            if (chance <= 0)
            {
                Inventory.Instance.AddItem(new Item(item.name, pair.Key));
                return;
            }
        }
        Inventory.Instance.AddItem(new Item(item.name, item.dict.Keys.Last()));
    }
}

public class DoStat : IDo
{
    private int id;
    private HeroController hero;
    private EventController eventController;
    private StatWithChance stats;

    public DoStat(int id, HeroController hero, EventController eventController, StatWithChance stats)
    {
        this.id = id;
        this.hero = hero;
        this.eventController = eventController;
        this.stats = stats;
    }

    public int Id { get => id; set => id = value; }

    public EventType event_type => EventType.STAT;

    public void Do()
    {
        int stat = 0, old_stat = 0;
        int event_id = -1;
        switch (stats.type)
        {
            case StatType.STRENGTH:
                stat = hero.Strength;
                break;
            case StatType.PERSISTENCE:
                stat = hero.Persistence;
                break;
            case StatType.AGILITY:
                stat = hero.Agility;
                break;
            case StatType.ATTENTION:
                stat = hero.Attention;
                break;
        }
        foreach (var pair in stats.dict)
        {
            if (stat <= pair.Key && old_stat <= pair.Key)
            {
                old_stat = pair.Key;
                event_id = pair.Value;
            }
        }
        eventController.Execute(event_id);
    }
}
