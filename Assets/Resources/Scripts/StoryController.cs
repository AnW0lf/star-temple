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

    public List<StoryWordController> words { get; private set; } = new List<StoryWordController>();

    private Room room;

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
        foreach (IWord word in words)
        {
            word.Show(showDelay);
            yield return delay;
        }
    }

    public void AddAnnotation(int index)
    {
        if (room.annotations.ToList().Where(a => a.id == index).Count() > 0)
        {
            Annotation annotation = room.annotations.ToList().Find(a => a.id == index);
            print(string.Format("Id : {0} | Length : {1} words", annotation.id, annotation.words.Length));
            annotations.AddAnnotation(annotation);
        }
        else
        {
            print("Not found!!!");
            foreach (var annotation in room.annotations)
            {
                print(string.Format("Id : {0} | Length : {1} words", annotation.id, annotation.words.Length));
            }
        }
    }
}
