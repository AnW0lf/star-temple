using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

public class CustomEventSystem : MonoBehaviour
{
    public static CustomEventSystem current { get; private set; } = null;

    public Dictionary<string, int> Local { get; private set; } = new Dictionary<string, int>();
    public Dictionary<string, int> Global { get; private set; } = new Dictionary<string, int>();

    private Dictionary<int, List<IExecute>> executable = new Dictionary<int, List<IExecute>>();

    private void Awake()
    {
        if (current == null) current = this;
        else if (current != this) Destroy(gameObject);
    }

    public void SetEvents(CustomEvent[] xmlEvents)
    {
        executable = new Dictionary<int, List<IExecute>>();

        xmlEvents.ToList().ForEach(e =>
        {
            print("Add " + e.id + " " + executable.ContainsKey(e.id));
            if (!executable.ContainsKey(e.id))
                executable.Add(e.id, new List<IExecute>());
        });

        executable.ToList().ForEach(pair =>
        {
            List<CustomEvent> indexed = xmlEvents.ToList().FindAll(e => e.id == pair.Key);
            indexed.FindAll(i => i.type == CustomEventType.TRIGGER).ForEach(trigger => executable[pair.Key].Add(new ExecuteTrigger(trigger)));
            indexed.FindAll(i => i.type == CustomEventType.GIVE).ForEach(give => executable[pair.Key].Add(new ExecuteGive(give)));
            indexed.FindAll(i => i.type == CustomEventType.WINDOW).ForEach(window => executable[pair.Key].Add(new ExecuteWindow(window)));
            indexed.FindAll(i => i.type == CustomEventType.CONDITION).ForEach(condition => executable[pair.Key].Add(new ExecuteCondition(condition)));
            indexed.FindAll(i => i.type == CustomEventType.NEXT).ForEach(next => executable[pair.Key].Add(new ExecuteNext(next)));
        });

        executable.ToList().ForEach(pair =>
        {
            for (int i = 0; i < pair.Value.Count - 1; i++)
                pair.Value[i].Link = pair.Value[i + 1];
        });
    }

    public void SetTriggers(Dictionary<string, int> triggers)
    {
        Local.Clear();

        foreach(var pair in triggers)
        {
            string[] arr = pair.Key.Split(':');
            string name = arr[1];

            switch (arr[0])
            {
                case "local":
                    Local.Add(name, pair.Value);
                    break;
                case "global":
                    Global.Add(name, pair.Value);
                    break;
                default:
                    throw new ArgumentException(string.Format("Trigger {0} has incorrect location option \'{1}\'", name, arr[0]));
            }
        }
    }

    public void Execute(int id)
    {
        print("Execute " + id);
        if (executable.ContainsKey(id))
            executable[id].First().Execute();
    }

    // TODO
    public void Execute(int id, Item item)
    {
        Execute(id);
    }

    //TODO
    public void Execute(int id, Action action)
    {
        Execute(id);
    }
}

/// <summary>
/// 0 - NEXT
/// 1 - WINDOW
/// 2 - CONDITION
/// 3 - GIVE
/// 4 - TRIGGER
/// </summary>
public enum CustomEventType { NEXT, WINDOW, CONDITION, GIVE, TRIGGER }

public class CustomEvent
{
    public int id;
    public CustomEventType type;
    public Dictionary<string, string> options;

    public CustomEvent(int id, CustomEventType type, Dictionary<string, string> options)
    {
        this.id = id;
        this.type = type;
        this.options = options;
    }

    public override bool Equals(object obj)
    {
        return obj is CustomEvent @event &&
               id == @event.id &&
               EqualityComparer<CustomEventType>.Default.Equals(type, @event.type) &&
               EqualityComparer<Dictionary<string, string>>.Default.Equals(options, @event.options);
    }

    public override int GetHashCode()
    {
        var hashCode = 992619482;
        hashCode = hashCode * -1521134295 + id.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<CustomEventType>.Default.GetHashCode(type);
        hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<string, string>>.Default.GetHashCode(options);
        return hashCode;
    }

    public static CustomEvent FromXml(XElement element)
    {
        int id = -1;
        CustomEventType type = 0;
        Dictionary<string, string> options = new Dictionary<string, string>();

        foreach (XAttribute XAttr in element.Attributes())
        {
            switch (XAttr.Name.ToString())
            {
                case "event_id":
                    if (!IOHelper.GetAttributeValue(XAttr, out id))
                        throw new ArgumentException("Event has incorrect value of attribute \'event_id\'");
                    break;
                case "event_type":
                    if (!IOHelper.GetAttributeValue(XAttr, out int itype))
                        throw new ArgumentException("Event has incorrect value of attribute \'event_type\'");
                    type = (CustomEventType)itype;
                    break;
                default:
                    string value;
                    if (!IOHelper.GetAttributeValue(XAttr, out value))
                        throw new ArgumentException(string.Format("Event has incorrect value of attribute \'{0}\'", XAttr.Name.ToString()));
                    options.Add(XAttr.Name.ToString(), value);
                    break;
            }
        }

        if (!element.HasElements && element.Value.Length != 0)
            options.Add("text", element.Value);

        return new CustomEvent(id, type, options);
    }
}

public interface IExecute
{
    IExecute Link { get; set; }

    void Next();

    void Execute();
}

public class ExecuteNext : IExecute
{
    public string filter, roomName;

    public ExecuteNext(string roomName)
    {
        string[] arr = roomName.Split(':');
        if (arr.Length == 1)
        {
            this.roomName = roomName;
            filter = "";
        }
        else
        {
            filter = arr[0];
            this.roomName = arr[1];
        }
    }

    public ExecuteNext(CustomEvent e)
    {
        if (!e.options.ContainsKey("room_name"))
        {
            string str = "";
            e.options.ToList().ForEach(p => str += string.Format("| {0} : {1} |", p.Key, p.Value));
            throw new ArgumentException(string.Format("Event {0} {1} does not contains option \'room_name\'. {2}", e.type.ToString(), e.id, str));
        }

        string[] arr = e.options["room_name"].Split(':');
        if (arr.Length == 1)
        {
            this.roomName = arr[0];
            filter = "";
        }
        else
        {
            filter = arr[0];
            this.roomName = arr[1];
        }
    }

    public IExecute Link { get; set; } = null;

    public void Execute()
    {
        GameCtrl.current.LoadRoom(roomName);
        Next();
    }

    public void Next()
    {
        if (Link != null)
            Link.Execute();
    }
}

// TODO
public class ExecuteWindow : IExecute
{
    public string text;

    public ExecuteWindow(string text)
    {
        this.text = text;
    }

    public ExecuteWindow(CustomEvent e)
    {
        if (!e.options.ContainsKey("text"))
            throw new ArgumentException(string.Format("Event {0} {1} does not contains option \'text\'.", e.type.ToString(), e.id));

        text = e.options["text"];
    }

    public IExecute Link { get; set; } = null;

    public void Execute()
    {
        // Открыть окно с текстом text
        Next();
    }

    public void Next()
    {
        if (Link != null)
            Link.Execute();
    }
}


/// <summary>
/// EqualsTo is value equals to condition value
/// NotEqualsTo is value not equals (greater or less than) to condition value
/// GreaterThan is value greater than and not equals to condition value
/// GreaterThanOrEqualsTo is value greater than or equals to condition value
/// LessThan is value less than and not equals to condition value
/// LessThanOrEqualsTo is value less than or equals to condition value
/// </summary>
public enum ConditionType { EqualsTo, NotEqualsTo, GreaterThan, GreaterThanOrEqualsTo, LessThan, LessThanOrEqualsTo }

/// <summary>
/// NAME is string value of Hero name
/// HP is integer value of Hero hp
/// LEVEL is integer value of Hero level
/// MONEY is integer value of Hero money
/// ITEM is array of Hero items type of Item
/// ACTION is array of Hero actions type of Action
/// TRIGGER is dictionary of local and global triggers states
/// </summary>
public enum ConditionOption { NAME, HP, LEVEL, MONEY, ITEM, ACTION, TRIGGER }
public class ExecuteCondition : IExecute
{
    public ConditionType type;
    public ConditionOption option;
    public int value;
    public string name;
    public int trueId, falseId;

    public ExecuteCondition(ConditionType type, ConditionOption option, int value, string name, int trueId, int falseId)
    {
        this.type = type;
        this.option = option;
        this.value = value;
        this.name = name;
        this.trueId = trueId;
        this.falseId = falseId;
    }

    public ExecuteCondition(CustomEvent e)
    {
        if (e.options == null || !e.options.ContainsKey("condition_type"))
            throw new ArgumentException(string.Format("Event Condition {0} does not contain option \'condition_type\'", e.id));
        if (!int.TryParse(e.options["condition_type"], out int type))
            throw new ArgumentException(string.Format("Event Condition {0} has incorrect value of option \'condition_type\'", e.id));
        this.type = (ConditionType)type;

        if (e.options == null || !e.options.ContainsKey("condition_option"))
            throw new ArgumentException(string.Format("Event Condition {0} does not contain option \'condition_option\'", e.id));
        if (!int.TryParse(e.options["condition_option"], out int option))
            throw new ArgumentException(string.Format("Event Condition {0} has incorrect value of option \'condition_option\'", e.id));
        this.option = (ConditionOption)option;

        if (e.options == null || !e.options.ContainsKey("condition_value"))
            throw new ArgumentException(string.Format("Event Condition {0} does not contain option \'condition_value\'", e.id));
        if (!int.TryParse(e.options["condition_value"], out value))
            throw new ArgumentException(string.Format("Event Condition {0} has incorrect value of option \'condition_value\'", e.id));

        if (e.options != null && e.options.ContainsKey("condition_false_id"))
        {
            if (!int.TryParse(e.options["condition_false_id"], out falseId))
                throw new ArgumentException(string.Format("Event Condition {0} has incorrect value of option \'condition_false_id\'", e.id));
        }
        else falseId = 0;

        if (e.options != null && e.options.ContainsKey("condition_true_id"))
        {
            if (!int.TryParse(e.options["condition_true_id"], out trueId))
                throw new ArgumentException(string.Format("Event Condition {0} has incorrect value of option \'condition_true_id\'", e.id));
        }
        else trueId = 0;

        if (e.options != null && e.options.ContainsKey("condition_name"))
            name = e.options["condition_name"];
        else name = "";
    }

    public IExecute Link { get; set; } = null;

    private static bool CheckInt(ConditionType type, int value, int conditionValue)
    {
        switch (type)
        {
            case ConditionType.EqualsTo: return value == conditionValue;
            case ConditionType.NotEqualsTo: return value != conditionValue;
            case ConditionType.GreaterThan: return value > conditionValue;
            case ConditionType.GreaterThanOrEqualsTo: return value >= conditionValue;
            case ConditionType.LessThan: return value < conditionValue;
            case ConditionType.LessThanOrEqualsTo: return value <= conditionValue;
            default: return false;
        }
    }

    public void Execute()
    {
        switch (option)
        {
            case ConditionOption.HP:
                CustomEventSystem.current.Execute(CheckInt(type, HeroCtrl.current.HP, value) ? trueId : falseId);
                break;
            case ConditionOption.LEVEL:
                CustomEventSystem.current.Execute(CheckInt(type, HeroCtrl.current.Level, value) ? trueId : falseId);
                break;
            case ConditionOption.MONEY:
                CustomEventSystem.current.Execute(CheckInt(type, HeroCtrl.current.Money, value) ? trueId : falseId);
                break;
            case ConditionOption.NAME:
                {
                    switch (type)
                    {
                        case ConditionType.EqualsTo:
                            {
                                CustomEventSystem.current.Execute(HeroCtrl.current.Name.Equals(name) ? trueId : falseId);
                                break;
                            }
                        case ConditionType.NotEqualsTo:
                            {
                                CustomEventSystem.current.Execute(!HeroCtrl.current.Name.Equals(name) ? trueId : falseId);
                                break;
                            }
                        case ConditionType.GreaterThan:
                        case ConditionType.GreaterThanOrEqualsTo:
                        case ConditionType.LessThan:
                        case ConditionType.LessThanOrEqualsTo:
                            {
                                CustomEventSystem.current.Execute(falseId);
                                break;
                            }
                    }
                    break;
                }
            case ConditionOption.ITEM:
                {
                    switch (type)
                    {
                        case ConditionType.EqualsTo:
                            CustomEventSystem.current.Execute(HeroCtrl.current.Count(IOHelper.GetItem(name)) == value ? trueId : falseId);
                            break;
                        case ConditionType.NotEqualsTo:
                            CustomEventSystem.current.Execute(HeroCtrl.current.Count(IOHelper.GetItem(name)) != value ? trueId : falseId);
                            break;
                        case ConditionType.GreaterThan:
                            CustomEventSystem.current.Execute(HeroCtrl.current.Count(IOHelper.GetItem(name)) > value ? trueId : falseId);
                            break;
                        case ConditionType.GreaterThanOrEqualsTo:
                            CustomEventSystem.current.Execute(HeroCtrl.current.Count(IOHelper.GetItem(name)) >= value ? trueId : falseId);
                            break;
                        case ConditionType.LessThan:
                            CustomEventSystem.current.Execute(HeroCtrl.current.Count(IOHelper.GetItem(name)) < value ? trueId : falseId);
                            break;
                        case ConditionType.LessThanOrEqualsTo:
                            CustomEventSystem.current.Execute(HeroCtrl.current.Count(IOHelper.GetItem(name)) <= value ? trueId : falseId);
                            break;
                    }
                    break;
                }
            case ConditionOption.ACTION:
                {
                    switch (type)
                    {
                        case ConditionType.EqualsTo:
                            CustomEventSystem.current.Execute(HeroCtrl.current.Count(IOHelper.GetAction(name)) == value ? trueId : falseId);
                            break;
                        case ConditionType.NotEqualsTo:
                            CustomEventSystem.current.Execute(HeroCtrl.current.Count(IOHelper.GetAction(name)) != value ? trueId : falseId);
                            break;
                        case ConditionType.GreaterThan:
                            CustomEventSystem.current.Execute(HeroCtrl.current.Count(IOHelper.GetAction(name)) > value ? trueId : falseId);
                            break;
                        case ConditionType.GreaterThanOrEqualsTo:
                            CustomEventSystem.current.Execute(HeroCtrl.current.Count(IOHelper.GetAction(name)) >= value ? trueId : falseId);
                            break;
                        case ConditionType.LessThan:
                            CustomEventSystem.current.Execute(HeroCtrl.current.Count(IOHelper.GetAction(name)) < value ? trueId : falseId);
                            break;
                        case ConditionType.LessThanOrEqualsTo:
                            CustomEventSystem.current.Execute(HeroCtrl.current.Count(IOHelper.GetAction(name)) <= value ? trueId : falseId);
                            break;
                    }
                    break;
                }
            case ConditionOption.TRIGGER:
                {
                    string[] arr = name.Split(':');
                    string triggerName = arr[1];

                    Dictionary<string, int> dict;

                    switch (arr[0])
                    {
                        case "local":
                            dict = CustomEventSystem.current.Local;
                            break;
                        case "global":
                            dict = CustomEventSystem.current.Global;
                            break;
                        default:
                            throw new ArgumentException(string.Format("Event Condition has incorrect value of \'name\' = \'{0}\'", name));
                    }

                    if (!dict.ContainsKey(triggerName))
                    {
                        CustomEventSystem.current.Execute(falseId);
                        break;
                    }

                    switch (type)
                    {
                        case ConditionType.EqualsTo:
                            CustomEventSystem.current.Execute(dict[triggerName] == value ? trueId : falseId);
                            break;
                        case ConditionType.NotEqualsTo:
                            CustomEventSystem.current.Execute(dict[triggerName] != value ? trueId : falseId);
                            break;
                        case ConditionType.GreaterThan:
                            CustomEventSystem.current.Execute(dict[triggerName] > value ? trueId : falseId);
                            break;
                        case ConditionType.GreaterThanOrEqualsTo:
                            CustomEventSystem.current.Execute(dict[triggerName] >= value ? trueId : falseId);
                            break;
                        case ConditionType.LessThan:
                            CustomEventSystem.current.Execute(dict[triggerName] < value ? trueId : falseId);
                            break;
                        case ConditionType.LessThanOrEqualsTo:
                            CustomEventSystem.current.Execute(dict[triggerName] <= value ? trueId : falseId);
                            break;
                    }
                    break;
                }
        }

        Next();
    }

    public void Next()
    {
        if (Link != null)
            Link.Execute();
    }
}

/// <summary>
/// 0 - HP
/// 1 - LEVEL
/// 2 - MONEY
/// 3 - ITEM
/// 4 - ACTION
/// </summary>
public enum GiveType { HP, LEVEL, MONEY, ITEM, ACTION }
public class ExecuteGive : IExecute
{
    public GiveType type;
    public int value;
    public string name;

    public ExecuteGive(GiveType type, int value, string name)
    {
        this.type = type;
        this.value = value;
        this.name = name;
    }

    public ExecuteGive(CustomEvent e)
    {
        if (e.options == null || !e.options.ContainsKey("give_type"))
            throw new ArgumentException(string.Format("Event Give {0} does not contain option \'give_type\'", e.id));
        if (!int.TryParse(e.options["give_type"], out int type))
            throw new ArgumentException(string.Format("Event Give {0} has incorrect value of option \'give_type\'", e.id));
        this.type = (GiveType)type;

        if (e.options == null || !e.options.ContainsKey("give_value"))
            throw new ArgumentException(string.Format("Event Give {0} does not contain option \'give_value\'", e.id));
        if (!int.TryParse(e.options["give_type"], out value))
            throw new ArgumentException(string.Format("Event Give {0} has incorrect value of option \'give_value\'", e.id));

        if (e.options != null && e.options.ContainsKey("give_name"))
            name = e.options["give_name"];
    }

    public IExecute Link { get; set; } = null;

    public void Execute()
    {
        switch (type)
        {
            case GiveType.HP:
                {
                    HeroCtrl.current.HP += value;
                    break;
                }
            case GiveType.LEVEL:
                {
                    HeroCtrl.current.Level += value;
                    break;
                }
            case GiveType.MONEY:
                {
                    HeroCtrl.current.Money += value;
                    break;
                }
            case GiveType.ITEM:
                {
                    if (value > 0)
                    {
                        HeroCtrl.current.Add(IOHelper.GetItem(name), value);
                    }
                    else if (value < 0)
                    {
                        HeroCtrl.current.Subtract(IOHelper.GetItem(name), -value);
                    }
                    break;
                }
            case GiveType.ACTION:
                {
                    if (value > 0)
                    {
                        HeroCtrl.current.Add(IOHelper.GetAction(name), value);
                    }
                    else if (value < 0)
                    {
                        HeroCtrl.current.Subtract(IOHelper.GetItem(name), -value);
                    }
                    break;
                }
            default: break;
        }

        Next();
    }

    public void Next()
    {
        if (Link != null)
            Link.Execute();
    }
}

public enum TriggerType { LOCAL, GLOBAL }
public class ExecuteTrigger : IExecute
{
    public string name;
    public TriggerType type;
    public int value;
    public IExecute Link { get; set; } = null;

    public ExecuteTrigger(string name, int value)
    {
        string[] arr = name.Split(':');
        this.name = arr[1];

        switch (arr[0])
        {
            case "local":
                type = TriggerType.LOCAL;
                break;
            case "global":
                type = TriggerType.GLOBAL;
                break;
            default:
                throw new ArgumentException(string.Format("Event Trigger has incorrect value of \'name\' = \'{0}\'", name));
        }

        this.value = value;
    }

    public ExecuteTrigger(CustomEvent e)
    {
        if (e.options == null || !e.options.ContainsKey("trigger_name"))
            throw new ArgumentException(string.Format("Event Trigger {0} does not contain option \'trigger_name\'", e.id));

        string[] arr = e.options["trigger_name"].Split(':');
        name = arr[1];

        switch (arr[0])
        {
            case "local":
                type = TriggerType.LOCAL;
                break;
            case "global":
                type = TriggerType.GLOBAL;
                break;
            default:
                throw new ArgumentException(string.Format("Event Trigger {1} has incorrect value of \'name\' = \'{0}\'", name, e.id));
        }

        if (e.options == null || !e.options.ContainsKey("trigger_value"))
            throw new ArgumentException(string.Format("Event Trigger {0} does not contain option \'trigger_value\'", e.id));

        if (!int.TryParse(e.options["trigger_value"], out int value))
            throw new ArgumentException(string.Format("Event Trigger {1} has incorrect value of \'trigger_value\' = \'{0}\'", e.options["trigger_value"], e.id));

        this.value = value;
    }

    public void Execute()
    {
        if (type == TriggerType.LOCAL)
        {
            if (CustomEventSystem.current.Local.ContainsKey(name))
                CustomEventSystem.current.Local[name] = value;
            else CustomEventSystem.current.Local.Add(name, value);
        }
        else if (type == TriggerType.GLOBAL)
        {
            if (CustomEventSystem.current.Global.ContainsKey(name))
                CustomEventSystem.current.Global[name] = value;
            else CustomEventSystem.current.Global.Add(name, value);
        }

        Next();
    }

    public void Next()
    {
        if (Link != null)
            Link.Next();
    }
}
