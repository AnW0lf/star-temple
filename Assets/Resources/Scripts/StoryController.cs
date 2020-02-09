using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StoryController : MonoBehaviour
{
    [Range(0.1f, 5f)]
    public float showDelayStep = 0.2f;
    [Range(0.1f, 5f)]
    public float showDelay = 0.8f;
    [Range(0.1f, 5f)]
    public float hideDelay = 0.8f;
    public GameObject storyWordPrefab;
    [Header("Annotations")]
    public AnnotationsController annotations;

    public List<IWord> words { get; private set; } = new List<IWord>();

    private RowLayoutGroup rowLayoutGroup;

    private Room room;

    private void OnEnable()
    {
        Initialize();
    }

    private void Initialize()
    {
        rowLayoutGroup = GetComponent<RowLayoutGroup>();
    }

    public void SetStory(Room room)
    {
        this.room = room;
        ClearStory(hideDelay);
        AddWords(this.room.story);
        StartCoroutine(ShowWords());
    }

    private void ClearStory(float delay)
    {
        words.ForEach(word => word.Hide(hideDelay));
    }

    private void AddWords(StoryWord[] story)
    {
        story.ToList().ForEach(word => {
            words.Add(Instantiate(storyWordPrefab, transform).GetComponent<IWord>());
            words.Last().SetWord(word);
            });
        rowLayoutGroup.Post();
    }

    private IEnumerator ShowWords()
    {
        WaitForSeconds delay = new WaitForSeconds(showDelayStep);
        foreach(IWord word in words)
        {
            word.Show(showDelay);
            yield return delay;
        }
    }

    public void AddAnnotation(int index)
    {
        annotations.AddAnnotation(new string[] {" Телевизор - ", ", это штука", ", где", " я", " нахожусь", " ЕЕЕЕЕЕЕ!" });
    }
}
