using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragHelper : MonoBehaviour
{
    public static DragHelper Instance = null;
    public ItemController item { get; set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance == this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}
