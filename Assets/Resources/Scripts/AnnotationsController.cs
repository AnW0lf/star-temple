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

    public void AddAnnotation(string[] words)
    {
        Transform annotation = Instantiate(annotationPrefab, content).transform;
        annotations.Add(annotation.gameObject);
        foreach(string word in words)
        {
            GameObject obj = Instantiate(annotationWordPrefab, annotation);
            obj.GetComponent<Text>().text = word;
            Invoke("Show", 0.5f);
        }
        annotation.GetComponent<RowLayoutGroup>().Post();
    }

    private void Show()
    {
        annotations.ForEach(annotation => annotation.GetComponentsInChildren<Fader>().ToList().ForEach(fader => fader.Show()));
    }
}
