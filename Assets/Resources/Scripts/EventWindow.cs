using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventWindow : MonoBehaviour
{
    public GameObject window;
    public Text content;

    public static EventWindow Instance = null;
    private IDo link = null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowWindow(string text, IDo link)
    {
        this.link = link;
        content.text = text;
        window.SetActive(true);
    }

    public void HideWindow()
    {
        content.text = "";
        window.SetActive(false);
        if (link != null)
            link.Condition();
    }
}
