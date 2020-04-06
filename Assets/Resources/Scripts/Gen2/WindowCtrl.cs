using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class WindowCtrl : MonoBehaviour
{
    public static WindowCtrl current { get; private set; } = null;
    public Image fader, window;
    public Text text;

    public Action OnHide;

    private void Awake()
    {
        if (current == null) current = this;
        else if (current != this) Destroy(gameObject);
        Hide();
    }

    public void Clear()
    {
        text.text = "";
        OnHide = null;
    }

    public void Show(string text)
    {
        this.text.text = text;
        this.text.enabled = true;
        Invoke("ShowScroll", 0.05f);
    }

    private void ShowScroll()
    {
        fader.enabled = true;
        window.enabled = true;
    }

    public void Hide()
    {
        fader.enabled = false;
        this.text.enabled = false;
        window.enabled = false;
        OnHide?.Invoke();
    }
}
