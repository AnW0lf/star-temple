using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleCtrl : MonoBehaviour
{
    public GameObject battleWordPref;
    public Transform content;
    public string assumptionPattern = "Вы понимаете, что {enemy} собирается совершить {kick_1} или {kick_2}.";
    public string questionPattern = "Что вы будете @делать ?";

    private List<BattleWordCtrl> words = new List<BattleWordCtrl>();

    public void SetBattle(Enemy enemy, Phrase phrase)
    {
        Clear();

        BattleWordCtrl bwc;

        foreach (string word in phrase.words)
        {
            bwc = Instantiate(battleWordPref, content).GetComponent<BattleWordCtrl>();
            bwc.SetWord(word, false);
            words.Add(bwc);
        }

        bwc = Instantiate(battleWordPref, content).GetComponent<BattleWordCtrl>();
        bwc.SetWord(" ", false);
        words.Add(bwc);

        List<string> aWords = new List<string>(assumptionPattern.Split(' '));
        List<string> qWords = new List<string>(questionPattern.Split(' '));

        for (int i = aWords.Count; i > 0; i--) aWords.Insert(i, " ");
        aWords.Add(" ");
        for (int i = qWords.Count; i > 0; i--) qWords.Insert(i, " ");

        foreach (string aWord in aWords)
        {
            bwc = Instantiate(battleWordPref, content).GetComponent<BattleWordCtrl>();
            string str = aWord;
            if (aWord.Contains("{enemy}")) str = aWord.Replace("{enemy}", enemy.name);
            else if (aWord.Contains("{kick_1}"))
            {
                str = aWord.Replace("{kick_1}", phrase.kick_1.Key.name);
                bwc.Chance = phrase.kick_1.Value;
            }
            else if (aWord.Contains("{kick_2}"))
            {
                str = aWord.Replace("{kick_2}", phrase.kick_2.Key.name);
                bwc.Chance = phrase.kick_2.Value;
            }

            bwc.SetWord(str, false);
            words.Add(bwc);
        }

        foreach (string qWord in qWords)
        {
            bwc = Instantiate(battleWordPref, content).GetComponent<BattleWordCtrl>();

            string str = qWord;

            if (qWord.StartsWith("@"))
            {
                str = qWord.Replace("@", "");
                bwc.SetWord(str, true);
            }
            else
            {
                bwc.SetWord(str, false);
            }
            words.Add(bwc);
        }
    }

    public void Clear()
    {
        if (words != null && words.Count > 0)
            words.ForEach(word => Destroy(word.gameObject));

        words.Clear();
    }
}
