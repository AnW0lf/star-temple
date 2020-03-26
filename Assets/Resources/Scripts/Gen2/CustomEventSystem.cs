using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomEventSystem : MonoBehaviour
{
    public static CustomEventSystem current { get; private set; } = null;

    public Dictionary<string, bool> Local { get; private set; } = new Dictionary<string, bool>();
    public Dictionary<string, bool> Global { get; private set; } = new Dictionary<string, bool>();

    private void Awake()
    {
        if (current == null) current = this;
        else if (current != this) Destroy(gameObject);
    }


}
