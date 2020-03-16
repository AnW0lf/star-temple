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
    private int event_id = -1, drop_id = -1;
    private string drop_type = "";
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
        drop_id = word.drop_id;
        drop_type = word.drop_type;

        if (drop_id > 0 && event_id > 0)
        {
            txt.fontStyle = FontStyle.BoldAndItalic;
            txt.raycastTarget = true;

            InitButton(true);
        }
        else if (drop_id > 0)
        {
            txt.fontStyle = FontStyle.Italic;
            txt.raycastTarget = true;

            InitButton(false);
        }
        else if (event_id > 0)
        {
            txt.fontStyle = FontStyle.BoldAndItalic;
            txt.raycastTarget = true;

            InitButton(true);
        }
        else
        {
            txt.fontStyle = FontStyle.Italic;
            txt.raycastTarget = false;

            InitButton(false);
        }
    }

    private void InitButton(bool active)
    {
        btn.interactable = active;

        if (active)
        {
            btn.onClick.AddListener(() => EventController.Instance.Execute(gameObject, event_id));
            if (!word.reusable_event)
                btn.onClick.AddListener(() => btn.interactable = false);
        }
        else btn.onClick.RemoveAllListeners();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
