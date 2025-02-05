using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SnakesSO")]
public class SnakesSO : ScriptableObject
{
    public int Head;
    public int Tail;
    public Sprite Prefab;

    public float X;
    public float Y;
}
