using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameCtrl : MonoBehaviour
{
    public StoryCtrl story;
    public BattleCtrl battle;
    public string chapterNumberPattern;
    public Text chapterNumber, chapterName;
    public static GameCtrl current { get; private set; } = null;

    public int Counter { get; private set; } = 0;

    private Room room;
    private Enemy enemy;
    private Phrase phrase;
    private Journey journey;

    private void Awake()
    {
        if (current == null) current = this;
        else if (current != this) Destroy(gameObject);

        Clear();
    }

    public void Clear()
    {
        Counter = -1;
    }

    public void StartGame()
    {
        Clear();
        journey = IOHelper.LoadJourney(HeroCtrl.current.Level);
        NextRoom();
    }

    public void NextRoom()
    {
        Counter++;
        if (Counter >= journey.journey.Count)
        {
            MainCtrl.current.MoveTo(1);
            return;
        }
        LoadRoom(journey.journey[Counter].Key, journey.journey[Counter].Value);
    }

    public void LoadRoom(string fileName, RoomType type)
    {
        CustomEventSystem.current.Clear();
        story.Clear();
        battle.Clear();

        switch (type)
        {
            case RoomType.RIDDLE:
                LoadRiddle(fileName);
                break;
            case RoomType.BATTLE:
                LoadBattle(fileName);
                break;
            default:
                NextRoom();
                return;
        }

    }

    private void LoadRiddle(string fileName)
    {
        room = IOHelper.LoadRoom(fileName);

        InventoryCtrl.current.Visible = room.type != "hideinventory";

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
    }

    private void LoadBattle(string fileName)
    {
        enemy = IOHelper.LoadEnemy(fileName);
        InventoryCtrl.current.Visible = true;

        chapterNumber.text = "";
        chapterName.text = enemy.name;

        InventoryCtrl.current.OpenSpells();

        phrase = enemy.phrases.First().Key;

        battle.SetBattle(enemy, phrase);
    }

    public void BattleStep(CustomAction action)
    {
        int rnd = Random.Range(0, (phrase.kick_1.Value + phrase.kick_2.Value) + 1);
        rnd -= phrase.kick_1.Value;
        if (rnd <= 0) WindowCtrl.current.Show(BattleRound(action, phrase.kick_1.Key));
        else WindowCtrl.current.Show(BattleRound(action, phrase.kick_2.Key));

        WindowCtrl.current.OnHide += NextBattleStep;
    }

    private string BattleRound(CustomAction action, Kick kick)
    {
        float fullDamageToHero = (float)HeroCtrl.current.HP * ((float)kick.percent_damage / 100f) + (float)kick.damage;
        float damageToHero = fullDamageToHero - fullDamageToHero * ((float)action.percent_defence / 100f) - (float)action.defence;

        float fullDamageToEnemy = (float)enemy.hp * ((float)action.percent_damage / 100f) + (float)action.damage;
        float damageToEnemy = fullDamageToEnemy - fullDamageToEnemy * ((float)kick.percent_defence / 100f) - (float)kick.defence;

        int fde = (int)fullDamageToEnemy, de = (int)damageToEnemy, fdh = (int)fullDamageToHero, dh = (int)damageToHero;

        string text = string.Format("{0} делает {1}", enemy.name, kick.name);
        if (fdh > 0) text += string.Format(" и наносит {0} урона.", fdh);
        else text += ".";

        text += string.Format("\nВы делаете {0}", action.name);
        if (fde > 0) text += string.Format(" и наносите {0} урона.", fde);
        else text += ".";

        if ((fde - de) > 0) text += string.Format("\n{0} блокирует {1} входящего урона.", enemy.name, (fde - de));
        if ((fdh - dh) > 0) text += string.Format("\nВы блокируете {0} входящего урона.", (fdh - dh));

        if (de > 0)
        {
            text += string.Format("\n{0} получает {1} урона.", enemy.name, de);
            enemy.hp -= de;
        }
        else text += string.Format("{0} не получает урона.", enemy.name);

        if (dh > 0)
        {
            text += string.Format("\nВы получаету {0} урона.", dh);
            HeroCtrl.current.HP -= dh;
        }
        else text += string.Format("\nВы не получаете урона.");

        return text;
    }

    public void NextBattleStep()
    {
        if (enemy.hp > 0)
        {
            int max = enemy.phrases.Values.Sum() + 1;
            int rnd = Random.Range(0, max);

            phrase = enemy.phrases.Last().Key;
            foreach (var pair in enemy.phrases)
            {
                rnd -= pair.Value;
                if (rnd <= 0)
                {
                    phrase = pair.Key;
                    break;
                }
            }

            battle.SetBattle(enemy, phrase);
        }
        else
        {
            WindowCtrl.current.Show(string.Format("Вы убили это исчадье, которое именуют {0}, и за победу получаете {1} опыта.", enemy.name, enemy.exp));
            WindowCtrl.current.OnHide += NextRoom;
        }
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

public enum RoomType { RIDDLE, BATTLE }

public class Journey
{
    public int difficult;
    public List<KeyValuePair<string, RoomType>> journey;

    public Journey()
    {
        this.difficult = 0;
        this.journey = new List<KeyValuePair<string, RoomType>>();
    }

    public Journey(int difficult, List<KeyValuePair<string, RoomType>> journey)
    {
        this.difficult = difficult;
        this.journey = journey;
    }
}
