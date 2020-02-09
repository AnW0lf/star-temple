using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryWordController : MonoBehaviour, IWord
{
    public StoryController story { get; private set; }

    private Text text;
    private Button btn;
    private int annotation_id, event_id;

    private void Awake()
    {
        if (transform.GetSiblingIndex() == 0)
            story = FindObjectOfType<StoryController>();
        else
            story = transform.parent.GetChild(0).GetComponent<StoryWordController>().story;

        text = GetComponent<Text>();
        btn = GetComponent<Button>();
    }

    private void Start()
    {
        btn.onClick.AddListener(Interact);
    }

    public void Interact()
    {
        story.AddAnnotation(0);
    }

    public void Show(float delay)
    {
        StartCoroutine(Showing(1f / delay));
    }

    public void Hide(float delay)
    {
        StartCoroutine(Hiding(1f / delay));
    }

    private IEnumerator Showing(float increment)
    {
        while(text.color.a < 1f)
        {
            text.color += new Color(0f, 0f, 0f, increment * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator Hiding(float decrement)
    {
        while (text.color.a < 1f)
        {
            text.color -= new Color(0f, 0f, 0f, decrement * Time.deltaTime);
            yield return null;
        }
        Destroy(gameObject);
    }

    public void SetWord(StoryWord word)
    {
        text.text = word.word;
        annotation_id = word.annotation_id;
        event_id = word.event_id;
        switch (word.type)
        {
            case WordType.REGULAR:
                text.fontStyle = FontStyle.Normal;
                btn.interactable = false;
                text.raycastTarget = true;
                break;
            case WordType.SEPARATOR:
                text.fontStyle = FontStyle.Normal;
                btn.interactable = false;
                text.raycastTarget = false;
                break;
            case WordType.BUTTON:
                text.fontStyle = FontStyle.Bold;
                btn.interactable = true;
                text.raycastTarget = true;
                break;
        }
    }
}
