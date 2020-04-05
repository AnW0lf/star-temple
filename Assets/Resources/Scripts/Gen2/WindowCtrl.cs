using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class WindowCtrl : MonoBehaviour
{
    public static WindowCtrl current { get; private set; } = null;
    public GameObject fader, window;
    public Text text;

    public Action OnHide;

    private void Awake()
    {
        if (current == null) current = this;
        else if (current != this) Destroy(gameObject);
    }

    public void Clear()
    {
        text.text = "";
        OnHide = null;
    }

    public void Show(string text)
    {
        this.text.text = text;
        fader.SetActive(true);
        window.SetActive(true);
    }

    public void Hide()
    {
        fader.SetActive(false);
        window.SetActive(false);
        OnHide?.Invoke();
    }
}
