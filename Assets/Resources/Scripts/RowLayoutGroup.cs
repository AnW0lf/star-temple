﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowLayoutGroup : MonoBehaviour
{
    public float verticalSpacing = 10f, horizontalSpacing = 0f, topPadding = 0f, bottomPadding = 0f, leftPadding = 0f, rightPadding = 0f;
    public Orientation orientation = Orientation.CENTER;
    public bool resize = true;
    private RectTransform self;

    private void Awake()
    {
        self = GetComponent<RectTransform>();
    }

    private void Update()
    {
        Post();
    }

    public void Post()
    {
        float width = self.sizeDelta.x, offsetX = leftPadding, offsetY = topPadding;
        List<RectTransform> children = new List<RectTransform>();

        for(int i = 0; i < transform.childCount; i++)
            children.Add(transform.GetChild(i).GetComponent<RectTransform>());

        children.ForEach(child => child.pivot = Vector2.up);
        Vector2[] sizes = new Vector2[children.Count], newPositions = new Vector2[children.Count];
        children.ForEach(child => sizes[children.IndexOf(child)] = child.sizeDelta);

        if (sizes.ToList().Where(size => size.x == 0).Count() > 0)
        {
            Invoke("Post", 0.01f);
            return;
        }

        int startIndex = 0;

        for (int i = 0; i < children.Count; i++)
        {
            if (offsetX + horizontalSpacing + sizes[i].x + rightPadding > width || i == children.Count - 1)
            {
                if (i == children.Count - 1)
                    offsetX += sizes[i].x;

                float offset = 0f;
                switch (orientation)
                {
                    case Orientation.CENTER:
                        offset = (width - offsetX) / 2f;
                        break;
                    case Orientation.RIGHT:
                        offset = width - offsetX;
                        break;
                }

                offsetY += sizes.ToList().GetRange(startIndex, (i == children.Count - 1 ? i + 1 : i) - startIndex).Max(v => v.y) + (startIndex > 0 ? verticalSpacing : 0f);
                offsetX = leftPadding;

                for (int j = startIndex; j < (i == children.Count - 1 ? i + 1 : i); j++)
                {
                    float yPos = sizes[j].y - offsetY;
                    newPositions[j] = new Vector2(offsetX + offset, yPos);
                    offsetX += horizontalSpacing + sizes[j].x;
                }

                offsetX = sizes[i].x;
                startIndex = i;
            }
            else
            {
                offsetX += sizes[i].x;
            }
        }

        for (int i = 0; i < children.Count; i++)
        {
            children[i].anchoredPosition = newPositions[i];
        }

        if (resize)
            self.sizeDelta = new Vector2(self.sizeDelta.x, Mathf.Abs(offsetY) + bottomPadding);
    }

}

public enum Orientation { LEFT, CENTER, RIGHT }