using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCtrl : MonoBehaviour
{
    public GameObject container;
    public RectTransform[] screens;
    public float startDelay = 3f, swapDuration = 1f;
    public LeanTweenType swapEase;

    public static MainCtrl current { get; private set; } = null;

    private void Awake()
    {
        if (current == null) current = this;
        else if (current != this) Destroy(gameObject);
    }

    private void Start()
    {
        LeanTween.delayedCall(startDelay, () => MoveTo(1));
    }

    public void MoveTo(int index)
    {
        if (index < screens.Length)
        {
            container.LeanMoveLocalX(-screens[index].anchoredPosition.x - 500f, swapDuration).setEase(swapEase);
        }
    }
}
