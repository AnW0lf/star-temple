using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnnotationWordController : MonoBehaviour
{
    public AnnotationsController annotations { get; private set; }

    [SerializeField]
    private Text txt;
    private Button btn;
    private int event_id;
    public AnnotationWord word { get; private set; }

    private void Awake()
    {
        annotations = GetComponentInParent<AnnotationsController>();

        btn = GetComponent<Button>();
    }

    public void Show(float delay)
    {
        StartCoroutine(Showing(1f / delay));
    }

    private IEnumerator Showing(float increment)
    {
        while (txt.color.a < 1f)
        {
            txt.color += new Color(0f, 0f, 0f, increment * Time.deltaTime);
            yield return null;
        }
    }

    public void SetWord(AnnotationWord word)
    {
        this.word = word;
        txt.text = word.word;
        event_id = word.event_id;
        switch (word.type)
        {
            case WordType.REGULAR:
                txt.fontStyle = FontStyle.Normal;
                txt.raycastTarget = true;
                btn.interactable = false;
                break;
            case WordType.SEPARATOR:
                txt.fontStyle = FontStyle.Normal;
                txt.raycastTarget = false;
                btn.interactable = false;
                break;
            case WordType.BUTTON:
                txt.fontStyle = FontStyle.Bold;
                txt.raycastTarget = true;
                btn.interactable = true;
                btn.onClick.AddListener(() => btn.interactable = false);
                btn.onClick.AddListener(() => EventController.Instance.Execute(event_id));
                break;
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
