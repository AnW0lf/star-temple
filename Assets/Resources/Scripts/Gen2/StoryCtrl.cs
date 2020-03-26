using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryCtrl : MonoBehaviour
{
    public AnnotationsCtrl annotations;
    public GameObject storyWordPref;
    public Transform content;

    private List<StoryWordCtrl> words = new List<StoryWordCtrl>();

    public void SetStory(Room room)
    {
        Clear();
        foreach (StoryWord word in room.story)
        {
            StoryWordCtrl swc = Instantiate(storyWordPref, content).GetComponent<StoryWordCtrl>();
            swc.SetWord(word);
            words.Add(swc);
        }
        annotations.annotations = room.annotations;
    }

    private void Clear()
    {
        if (words != null && words.Count > 0)
            words.ForEach(word => Destroy(word.gameObject));

        words.Clear();
    }

}

public interface IWord
{
    bool HasEvent { get; }
    bool HasDrop { get; }
}
