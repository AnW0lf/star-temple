using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnnotationsCtrl : MonoBehaviour
{
    public GameObject annotationPref, annotationWordPref;
    public Transform content;

    private List<GameObject> lstAnnot;
    private Annotation[] _annotations;
    public Annotation[] annotations
    {
        get => _annotations;
        set
        {
            _annotations = value;
            Clear();
        }
    }

    private void Clear()
    {
        if (lstAnnot != null && lstAnnot.Count > 0)
            lstAnnot.ForEach(a => Destroy(a, 0.05f));
        lstAnnot = new List<GameObject>();
    }

    public void CallAnnotation(int id)
    {
        if (annotations != null)
        {
            List<Annotation> ans = annotations.ToList().FindAll(a => a.id == id);
            if (ans.Count > 0)
            {
                Annotation a = rndAnnotation(ans);
                Transform container = Instantiate(annotationPref, content).transform;
                foreach (AnnotationWord word in a.words)
                    Instantiate(annotationWordPref, container).GetComponent<AnnotationWordCtrl>().SetWord(word);
            }
        }
    }

    private Annotation rndAnnotation(List<Annotation> annotations)
    {
        int max = 0;
        annotations.ForEach(a => max += a.chance);
        int rnd = Random.Range(0, max);

        foreach (Annotation a in annotations)
        {
            rnd -= a.chance;
            if (rnd <= 0)
                return a;
        }

        return annotations.Last();
    }
}
