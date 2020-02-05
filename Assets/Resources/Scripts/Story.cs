using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Story : MonoBehaviour
{
    public GameObject storyWordPrefab;
    [Header("Annotations")]
    public Annotations annotations;

    public List<GameObject> words { get; private set; } = new List<GameObject>();

    public void AddWords(string[] words)
    {
        this.words.ForEach(word => Destroy(word));
        this.words.Clear();

        foreach (string word in words)
        {
            this.words.Add(Instantiate(storyWordPrefab, transform));
            this.words.Last().GetComponent<Text>().text = word;
            Invoke("Show", 1.5f);
        }
        GetComponent<RowLayoutGroup>().Post();
    }

    private void Show()
    {
        words.ForEach(word => word.GetComponent<Fader>().Show());
    }

    public void AddAnnotation(int index)
    {
        annotations.AddAnnotation(new string[] {" Телевизор - ", ", это штука", ", где", " я", " нахожусь", " ЕЕЕЕЕЕЕ!" });
    }
}
