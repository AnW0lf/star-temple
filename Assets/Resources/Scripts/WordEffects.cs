using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class WordEffects : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField]
    private GameObject particle;

    [Header("Settings")]
    [SerializeField]
    private Color eventColor = Color.yellow;
    [SerializeField]
    private Color dropColor = Color.blue;
    [SerializeField]
    private int count = 10;
    [SerializeField]
    private float duration = 0.5f;

    private List<Image> particles;

    private RectTransform rect;

    private Color ec, dc;

    public delegate bool Condition();
    public Condition hasEvent, hasDrop;

    public void Begin()
    {
        Clean();

        ec = new Color(eventColor.r, eventColor.g, eventColor.b, 0f);
        dc = new Color(dropColor.r, dropColor.g, dropColor.b, 0f);

        rect = GetComponent<RectTransform>();

        for (int i = 0; i < count; i++)
            particles.Add(Instantiate(particle, rect.transform).GetComponent<Image>());

        particles.ForEach(p => StartCoroutine(Effect(p)));
    }

    private IEnumerator Effect(Image img)
    {
        WaitForSeconds sec = new WaitForSeconds(duration);
        img.rectTransform.localScale = Vector3.zero;
        yield return new WaitForSeconds(Random.Range(0f, duration));
        while (hasEvent() || hasDrop())
        {
            if (hasEvent() && hasDrop())
                img.color = Random.Range(0, 1) == 0 ? ec : dc;
            else if (hasEvent())
                img.color = ec;
            else if (hasDrop())
                img.color = dc;
            else break;

            img.rectTransform.anchoredPosition = new Vector2(Random.Range(0f, rect.sizeDelta.x), -Random.Range(0f, rect.sizeDelta.y));
            img.rectTransform.localScale = Vector3.zero;

            yield return new WaitForSeconds(Random.Range(0f, duration));

            img.rectTransform.LeanAlpha(1f, duration / 2f).setLoopPingPong(1);
            img.rectTransform.LeanScale(Vector3.one, duration / 2f).setLoopPingPong(1);

            yield return sec;
        }
        img.gameObject.SetActive(false);
    }

    private void Clean()
    {
        if (particles != null)
        {
            particles.ForEach(p => LeanTween.color(p.gameObject
                , new Color(p.color.r, p.color.g, p.color.b, 0f), duration)
            .setOnComplete(() => Destroy(p.gameObject)));
        }

        particles = new List<Image>();
    }
}
