using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Text))]
public class FlyingTextController : MonoBehaviour
{
    [SerializeField]
    private RectTransform rect;
    [SerializeField]
    private Text txt;
    public bool IsActive { get { return gameObject.activeSelf; } }

    public void SetText(string text, Vector3 pos, Color clr, int fontSize, float duration, float delay)
    {
        rect.position = pos;
        txt.text = text;
        txt.color = clr;
        txt.fontSize = fontSize;
        gameObject.SetActive(true);

        LeanTween.moveLocalY(gameObject, 50f, duration).setOnComplete(() => LeanTween.alphaText(rect, 0f, delay).setOnComplete(OnComplete));
    }

    private void OnComplete()
    {
        gameObject.SetActive(false);
    }
}
