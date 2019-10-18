using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableEffect
{
    //used for knowing what effect type to deserialize as
    public SubeffectType[] subeffectTypes;

    //array of strings that will get deserialized
    public string[] subeffects;

    //used for knowing what trigger to deserialize as
    public TriggerCondition triggerCondition;

    //string to be deserialized
    public string trigger;

    public int? maxTimesCanUsePerTurn;
}
