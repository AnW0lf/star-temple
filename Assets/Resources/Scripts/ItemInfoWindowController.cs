using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoWindowController : MonoBehaviour
{
    public GameObject window;
    public Text text;

    public static ItemInfoWindowController Instance { get; private set; } = null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetText(string text)
    {
        this.text.text = text;
        window.SetActive(true);
    }

    public void HideWindow()
    {
        this.text.text = "";
        window.SetActive(false);
    }
}
