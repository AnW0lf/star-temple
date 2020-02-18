using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnnotationsController : MonoBehaviour
{
    public GameObject annotationPrefab, annotationWordPrefab;
    public Transform content;
    [Range(0f, 2f)]
    public float showDelayStep = 0.2f;
    [Range(0.1f, 2f)]
    public float showDelay = 0.2f;

    public List<GameObject> annotations { get; private set; } = new List<GameObject>();

    public void AddAnnotation(Annotation annotation)
    {
        Transform annotationTransform = Instantiate(annotationPrefab, content).transform;
        annotations.Add(annotationTransform.gameObject);
        foreach(AnnotationWord word in annotation.words)
        {
            AnnotationWordController AWord = Instantiate(annotationWordPrefab, annotationTransform).GetComponent<AnnotationWordController>();
            AWord.SetWord(word);
        }
        StartCoroutine(ShowWords());
    }

    private IEnumerator ShowWords()
    {
        WaitForSeconds delay = new WaitForSeconds(showDelayStep);
        foreach (var word in annotations.Last().GetComponentsInChildren<AnnotationWordController>())
        {
            if (word != null)
            {
                word.Show(showDelay);
                yield return delay;
            }
        }
    }

    public void Clear()
    {
        annotations.ForEach(annotation => Destroy(annotation, 0.05f));
        annotations.Clear();
    }
}
