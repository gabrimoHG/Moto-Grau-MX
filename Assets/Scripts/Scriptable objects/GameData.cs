using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : ScriptableObject
{
    [Header("Header")]
    public string title;
    public int id;
    public bool isLocked;
}
