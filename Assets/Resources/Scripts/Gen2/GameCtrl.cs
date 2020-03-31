using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCtrl : MonoBehaviour
{
    public string roomName = "room_1";
    public StoryCtrl story;
    public string chapterNumberPattern;
    public Text chapterNumber, chapterName;
    public static GameCtrl current { get; private set; } = null;

    public int Counter { get; private set; } = 0;

    private Room room;

    private void Awake()
    {
        if (current == null) current = this;
        else if (current != this) Destroy(gameObject);

        Clear();
    }

    public void Clear()
    {
        Counter = 0;
    }

    public void LoadRoom(string roomName)
    {
        room = IOHelper.LoadRoom(roomName);

        InventoryCtrl.current.Visible = room.type != "introduction";

        CustomEventSystem.current.SetTriggers(room.triggers);
        CustomEventSystem.current.SetEvents(room.events);
        story.SetStory(room);

        if (Counter == 0)
        {
            chapterNumber.text = "";
            chapterName.text = room.name;
        }
        else
        {
            chapterNumber.text = string.Format(chapterNumberPattern, Counter);
            chapterName.text = room.name;
        }

        Counter++;
    }

    public void CallAnnotation(int id)
    {
        story.CallAnnotation(id);
    }
}

public class Room
{
    public string name;
    public string type;
    public StoryWord[] story;
    public Annotation[] annotations;
    public CustomEvent[] events;
    public Dictionary<string, int> triggers;

    public Room(string name, string type, StoryWord[] story, Annotation[] annotations, CustomEvent[] events, Dictionary<string, int> triggers)
    {
        this.name = name;
        this.type = type;
        this.story = story;
        this.annotations = annotations;
        this.events = events;
        this.triggers = triggers;
    }
}
