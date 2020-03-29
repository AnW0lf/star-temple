using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnnotationWordController : MonoBehaviour, IDropHandler
{
    public AnnotationsController annotations { get; private set; }

    [SerializeField]
    private Text txt;
    [SerializeField]
    private WordEffects effects;
    private Button btn;
    private int event_id = -1, drop_id = -1;
    private string drop_type = "";
    public AnnotationWord word { get; private set; }
    public bool IsEvented { get { return !btn.interactable; } }
    public bool IsDropped { get; private set; } = false;

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
        while (txt.color.a < 0.65f)
        {
            txt.color += new Color(0f, 0f, 0f, increment * Time.deltaTime);
            yield return null;
        }
        txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, 0.65f);
    }

    public void SetWord(AnnotationWord word)
    {
        this.word = word;
        txt.text = word.word;
        event_id = word.event_id;
        drop_id = word.drop_id;
        drop_type = word.drop_type;

        IsDropped = drop_id <= 0;

        if (drop_id > 0 && event_id > 0)
        {
            txt.fontStyle = FontStyle.Bold;
            txt.raycastTarget = true;

            InitButton(true);
        }
        else if (drop_id > 0)
        {
            txt.fontStyle = FontStyle.Normal;
            txt.raycastTarget = true;

            InitButton(false);
        }
        else if (event_id > 0)
        {
            txt.fontStyle = FontStyle.Bold;
            txt.raycastTarget = true;

            InitButton(true);
        }
        else
        {
            txt.fontStyle = FontStyle.Normal;
            txt.raycastTarget = false;

            InitButton(false);
        }

        if (effects == null) effects = GetComponent<WordEffects>();

        effects.Begin();
    }

    private void InitButton(bool active)
    {
        btn.interactable = active;

        if (active)
        {
            btn.onClick.AddListener(() => EventController.Instance.Execute(gameObject, event_id));
            if (!word.event_reusable)
                btn.onClick.AddListener(() => btn.interactable = false);
        }
        else btn.onClick.RemoveAllListeners();
    }

    public void OnDrop(PointerEventData eventData)
    {
        ItemController droppedItem = DragHelper.Instance.item;
        if (droppedItem != null)
        {
            if (!IsDropped && drop_type == Helper.Instance.GetItemType(droppedItem.item.Name))
            {
                HeroController.Instance.SubtractItem(droppedItem.item.Name);
                EventController.Instance.Execute(gameObject, drop_id);

                if (!word.drop_reusable) IsDropped = true;
            }
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
