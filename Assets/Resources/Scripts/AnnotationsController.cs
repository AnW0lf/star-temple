using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnnotationsController : MonoBehaviour
{
    public GameObject annotationPrefab, annotationWordPrefab;
    public Transform content;

    public List<GameObject> annotations { get; private set; } = new List<GameObject>();

    public void AddAnnotation(Annotation annotation)
    {
        Transform annotationTransform = Instantiate(annotationPrefab, content).transform;
        annotations.Add(annotationTransform.gameObject);
        foreach(AnnotationWord word in annotation.words)
        {
            GameObject obj = Instantiate(annotationWordPrefab, annotationTransform);
            Text wTxt = obj.GetComponent<Text>();
            wTxt.text = word.word;
            switch (word.type)
            {
                case WordType.REGULAR:
                    wTxt.fontStyle = FontStyle.Italic;
                    break;
                case WordType.BUTTON:
                    wTxt.fontStyle = FontStyle.BoldAndItalic;
                    Button wBtn = obj.GetComponent<Button>();
                    wBtn.interactable = true;
                    wBtn.onClick.AddListener(() => EventController.Instance.Execute(word.event_id));
                    break;
                case WordType.SEPARATOR:
                    wTxt.fontStyle = FontStyle.Italic;
                    break;
            }
            Invoke("Show", 0.5f);
        }
        Show();
    }

    public void Clear()
    {
        annotations.ForEach(annotation => Destroy(annotation, 0.05f));
        annotations.Clear();
    }

    private void Show()
    {
        annotations.ForEach(annotation => annotation.GetComponentsInChildren<Fader>().ToList().ForEach(fader => fader.Show()));
    }
}
