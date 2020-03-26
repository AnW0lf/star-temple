using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EventController : MonoBehaviour
{
    public GameController game;
    public HeroController hero;

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

            //Item Event
            {
                List<Event> item = indexed.FindAll(e => e.event_type == EventType.ITEM);
                if (item.Count > 0)
                {
                    if (!executable.ContainsKey(i)) executable.Add(i, new List<IDo>());

                    item.Sort(delegate (Event x, Event y) { return x.attributes["item_name"].CompareTo(y.attributes["item_name"]); });

                    Dictionary<string, Dictionary<int, int>> doItems = new Dictionary<string, Dictionary<int, int>>();
                    Dictionary<int, int> dict = new Dictionary<int, int>();
                    string item_name = item.First().attributes["item_name"];

                    for (int j = 0; j < item.Count; j++)
                    {
                        int item_count = 0, item_chance = 0;
                        TryParse(item[j], "item_count", out item_count);
                        TryParse(item[j], "item_chance", out item_chance);

                        if (item_name.Equals(item[j].attributes["item_name"]))
                        {
                            if (dict.ContainsKey(item_count))
                            {
                                dict[item_count] += item_chance;
                            }
                            else
                            {
                                dict.Add(item_count, item_chance);
                            }

                            if (j == item.Count - 1)
                            {
                                doItems.Add(item_name, dict);
                            }
                        }
                        else
                        {
                            doItems.Add(item_name, dict);
                            dict = new Dictionary<int, int>();
                            item_name = item[j].attributes["item_name"];
                            dict.Add(item_count, item_chance);
                        }
                    }

                    doItems.ToList().ForEach(p => executable[i].Add(new DoItem(i, p.Key, p.Value)));
                }
            }

            //Stat Event
            {
                List<Event> stat = indexed.FindAll(e => e.event_type == EventType.STAT);
                if (stat.Count > 0)
                {
                    if (!executable.ContainsKey(i)) executable.Add(i, new List<IDo>());

                    stat.Sort(delegate (Event x, Event y) { return x.attributes["stat_name"].CompareTo(y.attributes["stat_name"]); });

                    Dictionary<string, Dictionary<int, int>> doStats = new Dictionary<string, Dictionary<int, int>>();
                    Dictionary<int, int> dict = new Dictionary<int, int>();
                    string stat_name = stat.First().attributes["stat_name"];

                    for (int j = 0; j < stat.Count; j++)
                    {
                        int stat_count = 0, stat_chance = 0;
                        TryParse(stat[j], "stat_count", out stat_count);
                        TryParse(stat[j], "stat_chance", out stat_chance);

                        if (stat_name.Equals(stat[j].attributes["stat_name"]))
                        {
                            if (dict.ContainsKey(stat_count))
                            {
                                dict[stat_count] += stat_chance;
                            }
                            else
                            {
                                dict.Add(stat_count, stat_chance);
                            }

                            if (j == stat.Count - 1)
                            {
                                doStats.Add(stat_name, dict);
                            }
                        }
                        else
                        {
                            doStats.Add(stat_name, dict);
                            dict = new Dictionary<int, int>();
                            stat_name = stat[j].attributes["stat_name"];
                            dict.Add(stat_count, stat_chance);
                        }
                    }

                    doStats.ToList().ForEach(p => executable[i].Add(new DoItem(i, p.Key, p.Value)));
                }
            }

            // Window Event
            {
                List<Event> window = indexed.FindAll(e => e.event_type == EventType.WINDOW);
                if (window.Count > 0)
                {
                    if (!executable.ContainsKey(i)) executable.Add(i, new List<IDo>());
                    window.ForEach(WEvent => executable[i].Add(new DoWindow(i, WEvent.attributes["window_text"])));
                }
            }

            //Next Event
            {
                List<Event> next = indexed.FindAll(e => e.event_type == EventType.NEXT);
                if (next.Count > 0)
                {
                    if (!executable.ContainsKey(i)) executable.Add(i, new List<IDo>());
                    executable[i].Add(new DoNext(i, next.First().attributes["room_name"], game));
                }
            }

            //Condition Event
            {
                List<Event> condition = indexed.FindAll(e => e.event_type == EventType.CONDITION);

                if (condition.Count > 0)
                {
                    if (!executable.ContainsKey(i)) executable.Add(i, new List<IDo>());

                    foreach (Event @event in condition)
                    {
                        int bottom = 0, top = 0, trueId = 0, falseId = 0;
                        TryParse(@event, "condition_min", out bottom);
                        TryParse(@event, "condition_max", out top);
                        TryParse(@event, "condition_true_id", out trueId);
                        TryParse(@event, "condition_false_id", out falseId);
                        string stat_name = "", item_name = "";

                        if (@event.attributes.ContainsKey("condition_stat_name"))
                            stat_name = @event.attributes["condition_stat_name"];

                        if (@event.attributes.ContainsKey("condition_item_name"))
                            item_name = @event.attributes["condition_item_name"];

                        executable[i].Add(new DoCondition(i, stat_name, item_name, bottom, top, trueId, falseId));
                    }
                }
            }

            //ChancedCondition Event
            {
                List<Event> ccondition = indexed.FindAll(e => e.event_type == EventType.CCONDITION);

                if (ccondition.Count > 0)
                {
                    if (!executable.ContainsKey(i)) executable.Add(i, new List<IDo>());

                    foreach (Event @event in ccondition)
                    {
                        int trueId = 0, falseId = 0;
                        float luck = 0f;
                        TryParse(@event, "ccondition_luck", out luck);
                        TryParse(@event, "ccondition_true_id", out trueId);
                        TryParse(@event, "ccondition_false_id", out falseId);
                        string stat_name = "", item_name = "";

                        if (@event.attributes.ContainsKey("ccondition_stat_name"))
                            stat_name = @event.attributes["ccondition_stat_name"];

                        if (@event.attributes.ContainsKey("ccondition_item_name"))
                            item_name = @event.attributes["ccondition_item_name"];

                        executable[i].Add(new DoCCondition(i, luck, stat_name, item_name, trueId, falseId));
                    }
                }
            }
        }
    }

    private void TryParse(Event @event, string attribute_name, out int value)
    {
        if (!int.TryParse(@event.attributes[attribute_name], out value))
            throw new ArgumentException(string.Format("Event attribute \'{0}\' has incorrect value \'{1}\'. Event id = {2}",
                attribute_name, @event.attributes[attribute_name], @event.event_id));
    }

    private void TryParse(Event @event, string attribute_name, out float value)
    {
        if (!float.TryParse(@event.attributes[attribute_name], out value))
            throw new ArgumentException(string.Format("Event attribute \'{0}\' has incorrect value \'{1}\'. Event id = {2}",
                attribute_name, @event.attributes[attribute_name], @event.event_id));
    }

    public void Execute(GameObject sender, int id)
    {
        if (executable.ContainsKey(id))
        {
            List<IDo> indexed = executable[id];
            List<IDo> item = indexed.FindAll(i => i.event_type == EventType.ITEM);
            List<IDo> stat = indexed.FindAll(i => i.event_type == EventType.STAT);
            List<IDo> condition = indexed.FindAll(i => i.event_type == EventType.CONDITION);
            List<IDo> ccondition = indexed.FindAll(i => i.event_type == EventType.CCONDITION);
            List<IDo> window = indexed.FindAll(i => i.event_type == EventType.WINDOW);
            List<IDo> next = indexed.FindAll(i => i.event_type == EventType.NEXT);

            indexed = new List<IDo>();
            indexed.AddRange(item);
            indexed.AddRange(stat);
            indexed.AddRange(condition);
            indexed.AddRange(ccondition);
            indexed.AddRange(window);
            indexed.AddRange(next);

            for (int i = 1; i < indexed.Count; i++)
                indexed[i - 1].Link = indexed[i];

            indexed.First().Do(sender);

            print(string.Format("Execute id = {0}", id));
        }
        else
        {
            Debug.LogWarning(string.Format("Not found event with id = {0}", id));
        }
    }
}

public enum EventType { EMPTY, NEXT, ITEM, WINDOW, STAT, CONDITION, CCONDITION }

public struct Event
{
    // core parameters
    public int event_id;
    public EventType event_type;
    public Dictionary<string, string> attributes;

    public Event(int event_id, EventType event_type)
    {
        this.event_id = event_id;
        this.event_type = event_type;
        attributes = new Dictionary<string, string>();
    }

    public static Event Empty()
    {
        return new Event(-1, EventType.EMPTY);
    }

    public static EventType ParseEventType(string type)
    {
        switch (type)
        {
            case "next": return EventType.NEXT;
            case "item": return EventType.ITEM;
            case "condition": return EventType.CONDITION;
            case "ccondition": return EventType.CCONDITION;
            case "stat": return EventType.STAT;
            case "window": return EventType.WINDOW;
            default: return EventType.EMPTY;
        }
    }
}

public interface IDo
{
    int Id { get; set; }
    IDo Link { get; set; }

    EventType event_type { get; }

    void Condition();

    void Next();

    void Do(GameObject sender);
}

public class DoNext : IDo
{
    private string roomName;
    private int id;
    private GameController game;
    private IDo link;
    private GameObject sender;
    public IDo Link { get => link; set => link = value; }

    public int Id { get => id; set => id = value; }

    public EventType event_type => EventType.NEXT;

    public DoNext(int id, string roomName, GameController game)
    {
        Id = id;
        this.roomName = roomName;
        this.game = game;
        link = null;
        sender = null;
    }

    public void Do(GameObject sender)
    {
        this.sender = sender;
        game.NextRoom(roomName);
        Condition();
    }

    public void Condition()
    {
        if (Link != null)
            Next();
    }

    public void Next()
    {
        Link.Do(sender);
    }
}

public class DoItem : IDo
{
    private int id;
    private string item_name;
    private Dictionary<int, int> chances;
    private IDo link;
    private GameObject sender;
    public IDo Link { get => link; set => link = value; }

    public int Id { get => id; set => id = value; }

    public EventType event_type => EventType.ITEM;

    public DoItem(int id, string item_name, Dictionary<int, int> chances)
    {
        Id = id;
        this.item_name = item_name;
        this.chances = new Dictionary<int, int>(chances);
        link = null;
        sender = null;
    }

    public void Do(GameObject sender)
    {
        this.sender = sender;
        int max = chances.Values.Sum() + 1;
        int chance = Random.Range(0, max);
        foreach (var pair in chances)
        {
            chance -= pair.Value;
            if (chance <= 0)
            {
                FlyingTextManager.Instance.AddFlyingText(sender, string.Format("{0} {1}", pair.Key, item_name));
                HeroController.Instance.AddItem(item_name, pair.Key);
                return;
            }
        }
        FlyingTextManager.Instance.AddFlyingText(sender, string.Format("{0} {1}", chances.Keys.Last(), item_name));
        HeroController.Instance.AddItem(item_name, chances.Keys.Last());
        Condition();
    }

    public void Condition()
    {
        if (Link != null)
            Next();
    }

    public void Next()
    {
        Link.Do(sender);
    }
}

public class DoStat : IDo
{
    private int id;
    private HeroController hero;
    private string stat_name;
    private Dictionary<int, int> chances;
    private IDo link;
    private GameObject sender;
    public IDo Link { get => link; set => link = value; }

    public DoStat(int id, string stat_name)
    {
        this.id = id;
        this.stat_name = stat_name;
        chances = new Dictionary<int, int>();
        link = null;
        sender = null;
    }

    public int Id { get => id; set => id = value; }

    public EventType event_type => EventType.STAT;

    public void Do(GameObject sender)
    {
        this.sender = sender;
        int max = chances.Values.Sum() + 1;
        int chance = Random.Range(0, max);
        int value = 0;
        foreach (var pair in chances)
        {
            chance -= pair.Value;
            if (chance <= 0)
            {
                value = pair.Key;
                return;
            }
        }

        if (value == 0 && DragHelper.Instance.item != null)
        {
            value = Helper.Instance.GetItemValue(DragHelper.Instance.item.name);
        }

        //switch (stat_name)
        //{
        //    case "strength":
        //        FlyingTextManager.Instance.AddFlyingText(sender, string.Format("{0} {1}", "Strength", value));
        //        HeroController.Instance.Strength += value;
        //        break;
        //    case "agility":
        //        FlyingTextManager.Instance.AddFlyingText(sender, string.Format("{0} {1}", "Agility", value));
        //        HeroController.Instance.Agility += value;
        //        break;
        //    case "persistence":
        //        FlyingTextManager.Instance.AddFlyingText(sender, string.Format("{0} {1}", "Persistence", value));
        //        HeroController.Instance.Persistence += value;
        //        break;
        //    case "attention":
        //        FlyingTextManager.Instance.AddFlyingText(sender, string.Format("{0} {1}", "Attention", value));
        //        HeroController.Instance.Attention += value;
        //        break;
        //    case "hp":
        //        FlyingTextManager.Instance.AddFlyingText(sender, string.Format("{0} {1}", "HP", value));
        //        HeroController.Instance.HP += value;
        //        break;
        //    case "money":
        //        FlyingTextManager.Instance.AddFlyingText(sender, string.Format("{0} {1}", value, "coins"));
        //        HeroController.Instance.Money += value;
        //        break;
        //}
        Condition();
    }

    public void Condition()
    {
        if (Link != null)
            Next();
    }

    public void Next()
    {
        Link.Do(sender);
    }
}

public class DoWindow : IDo
{
    private int id;
    private string text;
    private IDo link;
    private GameObject sender;
    public IDo Link { get => link; set => link = value; }

    public DoWindow(int id, string text)
    {
        this.id = id;
        this.text = text;
        link = null;
        sender = null;
    }

    public int Id { get => id; set => id = value; }

    public EventType event_type => EventType.WINDOW;

    public void Do(GameObject sender)
    {
        this.sender = sender;
        EventWindow.Instance.ShowWindow(text, this);
    }

    public void Condition()
    {
        if (Link != null)
            Next();
    }

    public void Next()
    {
        Link.Do(sender);
    }
}

public class DoCondition : IDo
{
    private int id, min, max, trueId, falseId;
    private string stat_name, item_name;
    private IDo link;
    private GameObject sender;
    public IDo Link { get => link; set => link = value; }

    public DoCondition(int id, string stat_name, string item_name, int min, int max, int trueId, int falseId)
    {
        this.id = id;
        this.min = min;
        this.max = max;
        this.trueId = trueId;
        this.falseId = falseId;
        this.stat_name = stat_name;
        this.item_name = item_name;
        link = null;
        sender = null;
    }

    public int Id { get => id; set => id = value; }

    public EventType event_type => EventType.CONDITION;

    public void Do(GameObject sender)
    {
        bool win = false;
        //switch (stat_name)
        //{
        //    case "strength":
        //        win = InRange(HeroController.Instance.Strength, min, max);
        //        break;
        //    case "agility":
        //        win = InRange(HeroController.Instance.Agility, min, max);
        //        break;
        //    case "persistence":
        //        win = InRange(HeroController.Instance.Persistence, min, max);
        //        break;
        //    case "attention":
        //        win = InRange(HeroController.Instance.Attention, min, max);
        //        break;
        //    case "hp":
        //        win = InRange(HeroController.Instance.HP, min, max);
        //        break;
        //    case "money":
        //        win = InRange(HeroController.Instance.Money, min, max);
        //        break;
        //    case "item":
        //        foreach (var item in HeroController.Instance.Items)
        //        {
        //            if (item.Name.Equals(item_name) && item.Count > 0)
        //            {
        //                win = true;
        //                break;
        //            }
        //        }
        //        break;
        //}

        EventController.Instance.Execute(sender, win ? trueId : falseId);
        Condition();
    }

    private bool InRange(int value, int min, int max)
    {
        return value >= min && value < max;
    }

    public void Condition()
    {
        if (Link != null)
            Next();
    }

    public void Next()
    {
        Link.Do(sender);
    }
}

public class DoCCondition : IDo
{
    private int id, trueId, falseId;
    private float luck;
    private string stat_name, item_name;
    private IDo link;
    private GameObject sender;
    public IDo Link { get => link; set => link = value; }

    public DoCCondition(int id, float luck, string stat_name, string item_name, int trueId, int falseId)
    {
        this.id = id;
        this.luck = luck;
        this.trueId = trueId;
        this.falseId = falseId;
        this.stat_name = stat_name;
        this.item_name = item_name;
        link = null;
        sender = null;
    }

    public int Id { get => id; set => id = value; }

    public EventType event_type => EventType.CCONDITION;

    public void Do(GameObject sender)
    {
        this.sender = sender;
        bool win = false;
        int chance = Random.Range(0, 100);
        //switch (stat_name)
        //{
        //    case "strength":
        //        win = HeroController.Instance.Strength * luck >= chance;
        //        break;
        //    case "agility":
        //        win = HeroController.Instance.Agility * luck >= chance;
        //        break;
        //    case "persistence":
        //        win = HeroController.Instance.Persistence * luck >= chance;
        //        break;
        //    case "attention":
        //        win = HeroController.Instance.Attention * luck >= chance;
        //        break;
        //    case "hp":
        //        win = HeroController.Instance.HP * luck >= chance;
        //        break;
        //    case "money":
        //        win = HeroController.Instance.Money * luck >= chance;
        //        break;
        //    case "item":
        //        foreach (var item in HeroController.Instance.Items)
        //        {
        //            if (item.Name.Equals(item_name) && item.Count > 0)
        //            {
        //                win = 50f * luck >= chance;
        //                break;
        //            }
        //        }
        //        break;
        //}

        EventController.Instance.Execute(sender, win ? trueId : falseId);
        Condition();
    }

    public void Condition()
    {
        if (Link != null)
            Next();
    }

    public void Next()
    {
        Link.Do(sender);
    }
}
