using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemoveItemWindowController : MonoBehaviour
{
    public GameObject window;
    public Text questionTxt;
    public string sentence = "Вы хотите удалить \'{0}\'?";
    public GameObject remove, removeOne, removeAll;

    public static RemoveItemWindowController Instance { get; private set; } = null;

    private Item item;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Show(Item item)
    {
        this.item = item;
        questionTxt.text = string.Format(sentence, item.name);

        if (item.count > 1)
        {
            remove.SetActive(false);
            removeOne.SetActive(true);
            removeAll.SetActive(true);
        }
        else if (item.count == 1)
        {
            remove.SetActive(true);
            removeOne.SetActive(false);
            removeAll.SetActive(false);
        }
        else Hide();

        window.SetActive(true);
    }

    public void Remove(bool all)
    {
        for (int i = 0; i < (all ? item.count : 1); i++)
            HeroController.Instance.SubtractItem(item.name);
    }

    public void Hide()
    {
        window.SetActive(false);
    }
}
