using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableEffect
{
    //array of strings that will get deserialized
    public string[] subeffects;

    //used for knowing what trigger to deserialize as
    public TriggerCondition triggerCondition;

    //string to be deserialized
    public string trigger;

    public ActivationRestriction activationRestriction;

    public string blurb;
}
