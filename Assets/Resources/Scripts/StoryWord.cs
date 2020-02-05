using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryWord : MonoBehaviour
{
    private Story story;

    private void Awake()
    {
        story = GetComponentInParent<Story>();
    }

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(Interact);
    }

    public void Interact()
    {
        story.AddAnnotation(0);
    }
}
