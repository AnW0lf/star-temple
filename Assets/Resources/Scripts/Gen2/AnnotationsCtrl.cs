using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnnotationsCtrl : MonoBehaviour
{
    public GameObject annotationPref, annotationWordPref;
    public Transform content;

    private List<GameObject> lstAnnot = new List<GameObject>();
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
        lstAnnot.Clear();
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
                lstAnnot.Add(container.gameObject);
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

public class Annotation
{
    public int id, chance;
    public List<AnnotationWord> words;

    public Annotation(int id, int chance)
    {
        this.id = id;
        this.chance = chance;
        this.words = new List<AnnotationWord>();
    }

    public static Annotation FromXml(XElement element)
    {
        if (!IOHelper.GetAttributeValue(element, "id", out int id))
            throw new ArgumentException(string.Format("Annotation has incorrec value of attribute \'id\'"));

        if (!IOHelper.GetAttributeValue(element, "chance", out int chance))
            throw new ArgumentException(string.Format("Annotation has incorrec value of attribute \'chance\'"));

        Annotation annotation = new Annotation(id, chance);

        foreach(XElement XWord in element.Elements("word"))
            annotation.words.Add(AnnotationWord.FromXml(XWord));

        return annotation;
    }
}
