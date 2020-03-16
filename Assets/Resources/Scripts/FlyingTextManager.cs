using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingTextManager : MonoBehaviour
{
    public float duration = 1f, delay = 0.5f;

    public static FlyingTextManager Instance = null;

    private List<FlyingTextController> children;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        children = new List<FlyingTextController>();
        children.AddRange(transform.GetComponentsInChildren<FlyingTextController>());
        children.ForEach(c => c.gameObject.SetActive(false));
    }

    public void AddFlyingText(GameObject sender, string text)
    {
        FlyingTextController current;
        if ((current = children.Find(c => !c.IsActive)) != null)
        {
            current.SetText(text, sender.transform.position, Color.black, 45, duration, delay);
            current.transform.SetAsLastSibling();
        }
    }
}
