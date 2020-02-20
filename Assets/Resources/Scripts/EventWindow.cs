using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventWindow : MonoBehaviour
{
    public GameObject wordPrefab;
    public GameObject window;
    public Transform content;

    public static EventWindow Instance = null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowWindow(WindowWord[] words)
    {
        for (int i = content.childCount - 1; i >= 0; i++)
            Destroy(content.GetChild(i).gameObject);

        foreach (var word in words)
            Instantiate(wordPrefab, content).GetComponent<Text>().text = word.word;

        window.SetActive(true);
    }
}
