using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowLayoutGroup : MonoBehaviour
{
    public float spacing = 1.1f;
    private RectTransform self;

    private bool isPost = false;

    private void Awake()
    {
        self = GetComponent<RectTransform>();
    }

    public void Post()
    {
        float width = self.sizeDelta.x, offsetX = 0f, offsetY = 0f;
        List<RectTransform> children = transform.GetComponentsInChildren<RectTransform>().ToList();
        children.Remove(self);
        children.ForEach(child => child.pivot = Vector2.up);

        foreach (RectTransform child in children)
        {
            if (offsetX + child.sizeDelta.x > width)
            {
                offsetY -= child.sizeDelta.y * spacing;
                offsetX = 0f;
            }
            child.anchoredPosition = new Vector2(offsetX, offsetY);
            offsetX += child.sizeDelta.x;

            if(offsetX == 0 && offsetY == 0)
            {
                Invoke("Post", 0.1f);
                return;
            }
        }

        offsetY -= children.Last().sizeDelta.y * spacing;
        self.sizeDelta = new Vector2(self.sizeDelta.x, Mathf.Abs(offsetY));
    }

}
