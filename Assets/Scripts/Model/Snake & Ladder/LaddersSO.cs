using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LaddersSO")]
public class LaddersSO : ScriptableObject {
    public int Start;
    public int End;
    public GameObject Prefab;

    public float X;
    public float Y;
}
