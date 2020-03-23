using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class Unfade : MonoBehaviour
{
    public float duration;
    public LeanTweenType alphaEase;

    private void Awake()
    {
        LeanTween.textAlpha(GetComponent<RectTransform>(), 0f, 0f);
    }

    void Start()
    {
        LeanTween.textAlpha(GetComponent<RectTransform>(), 1f, duration).setEase(alphaEase);
    }
}
