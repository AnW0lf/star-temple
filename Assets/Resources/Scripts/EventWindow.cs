using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventWindow : MonoBehaviour
{
    public GameObject window;
    public Text content;

    public static EventWindow Instance = null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void ShowWindow(string text)
    {
        content.text = text;
        window.SetActive(true);
    }
}
