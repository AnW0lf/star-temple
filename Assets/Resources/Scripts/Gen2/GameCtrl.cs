using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCtrl : MonoBehaviour
{
    public string roomName = "room_1";
    public StoryCtrl story;
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
        room = Helper.Instance.LoadRoom(roomName);
        story.SetStory(room);
        Counter++;
    }
}
