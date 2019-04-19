using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableEffect
{
    public enum SubeffectType { TargetCardOnBoard, ChangeNESW }

    //used for knowing what effect type to deserialize as
    public SubeffectType[] subeffectTypes;

    //array of strings that will get deserialized
    public string[] subeffects;
}
