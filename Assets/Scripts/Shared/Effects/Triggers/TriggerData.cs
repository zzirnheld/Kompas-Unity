using KompasCore.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TriggerData
{
    public string triggerCondition;
    public TriggerRestriction triggerRestriction;

    public bool optional = false;
    public string blurb = "Trigger";
}
