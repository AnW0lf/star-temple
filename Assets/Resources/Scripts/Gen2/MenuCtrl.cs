using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCtrl : MonoBehaviour
{
    public static MenuCtrl current { get; private set; } = null;

    private void Awake()
    {
        if (current == null) current = this;
        else if (current != this) Destroy(gameObject);
    }

    public void StartGame()
    {
        MainCtrl.current.MoveTo(2);
        GameCtrl.current.LoadRoom(GameCtrl.current.roomName);
    }
}
