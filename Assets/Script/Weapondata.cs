using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName ="new weapondata", menuName = "weapondata")]
public class Weapondata : ScriptableObject
{
    public new string name;
    public GameObject prefab;
    public float fireinterval;
}
