using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroCtrl : MonoBehaviour
{
    public static HeroCtrl current { get; private set; } = null;

    private void Awake()
    {
        if (current == null) current = this;
        else Destroy(gameObject);
    }
}
