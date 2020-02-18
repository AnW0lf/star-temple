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
    public GameObject storyWordPrefab;
    [Header("Annotations")]
    public AnnotationsController annotations;

    public List<StoryWordController> words { get; private set; } = new List<StoryWordController>();

    private Room room;

    public void SetStory(Room room)
    {
        this.room = room;
        ClearStory();
        AddWords(this.room.story);
        StartCoroutine(ShowWords());
    }

    private void ClearStory()
    {
        words.ForEach(word => word.Hide());
        StopAllCoroutines();
        words.Clear();
        annotations.Clear();
    }

    private void AddWords(StoryWord[] story)
    {
        story.ToList().ForEach(word =>
        {
            words.Add(Instantiate(storyWordPrefab, transform).GetComponent<StoryWordController>());
            words.Last().SetWord(word);
        });
    }

    private IEnumerator ShowWords()
    {
        WaitForSeconds delay = new WaitForSeconds(showDelayStep);
        foreach (var word in words)
        {
            if (word != null)
            {
                word.Show(showDelay);
                yield return delay;
            }
        }
    }

    public void AddAnnotation(int index)
    {
        if (room.annotations.ToList().Where(a => a.id == index).Count() > 0)
        {
            Annotation annotation = room.annotations.ToList().Find(a => a.id == index);
            annotations.AddAnnotation(annotation);
        }
        else
        {
            print("Not found!!!");
        }
    }
}
