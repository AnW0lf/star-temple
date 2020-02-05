using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    [Range(0.1f, 5f)]
    public float second = 0.5f;
    public List<Image> images;
    public List<Text> texts;

    private bool showing = false, hiding = false;

    private void Update()
    {
        if (showing && hiding) hiding = false;

        if (showing)
        {
            texts.ForEach(text => text.color =
            new Color(text.color.r, text.color.g, text.color.b, text.color.a + (1 / second) * Time.deltaTime));
            images.ForEach(image => image.color =
            new Color(image.color.r, image.color.g, image.color.b, image.color.a + (1 / second) * Time.deltaTime));

            if (texts.Where(text => text.color.a < 1f).Count() == 0 && images.Where(image => image.color.a < 1f).Count() == 0)
            {
                showing = false;
            }
        }

        if (hiding)
        {
            texts.ForEach(text => text.color =
            new Color(text.color.r, text.color.g, text.color.b, text.color.a - (1 / second) * Time.deltaTime));
            images.ForEach(image => image.color =
            new Color(image.color.r, image.color.g, image.color.b, image.color.a - (1 / second) * Time.deltaTime));

            if (texts.Where(text => text.color.a < 1f).Count() == 0 && images.Where(image => image.color.a < 1f).Count() == 0)
            {
                hiding = false;
            }
        }
    }

    public void Show()
    {
        hiding = false;
        showing = true;
    }

    public void Hide()
    {
        hiding = true;
        showing = false;
    }
}
