using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Hero header")]
    public HeroController hero;
    [Range(0.1f, 10f)]
    public float speed = 1f;
    public float error = 1f;

    [Header("Story")]
    public StoryController story;

    [Header("Inventory")]
    public Inventory inv;

    [Header("Events")]
    public EventController eventController;

    [Header("First Run")]
    public string roomName = "room_1";

    private bool moveHero;

    public int RoomCounter { get; private set; }
    private Room room;

    private void OnEnable()
    {
        hero.rect.anchoredPosition = new Vector2(0f, -1280f);
        moveHero = true;
    }

    private void Update()
    {
        if (moveHero) MoveHero();
    }

    //public void StartStory(Hero hero)
    //{
    //    this.hero.Hero = hero;
    //    LoadInventory();
    //    RoomCounter = 0;
    //}

    //private void LoadInventory()
    //{
    //    inv.LoadItems(hero.Items.ToArray());
    //}

    public void NextRoom(string roomName)
    {
        RoomCounter++;
        room = Helper.Instance.LoadRoom(roomName);
        if (!room.name.Equals("introduction")) inv.Show();
        else inv.Hide();
        story.SetStory(room);
        eventController.Events = room.events;
    }

    private void MoveHero()
    {
        float distance = -hero.rect.anchoredPosition.y * Time.deltaTime * speed;
        hero.rect.anchoredPosition += Vector2.up * distance;

        if (Mathf.Abs(hero.rect.anchoredPosition.y) < error)
        {
            hero.rect.anchoredPosition = Vector2.zero;
            moveHero = false;
            NextRoom(roomName);
        }
    }
}
