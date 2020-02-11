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
            obj.GetComponent<Text>().text = word.word;
            Invoke("Show", 0.5f);
        }
        Show();
    }

    private void Show()
    {
        annotations.ForEach(annotation => annotation.GetComponentsInChildren<Fader>().ToList().ForEach(fader => fader.Show()));
    }
}
