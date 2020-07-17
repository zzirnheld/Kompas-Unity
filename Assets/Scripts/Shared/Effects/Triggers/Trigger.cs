using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trigger
{
    public TriggerCondition triggerCondition;
    public TriggerRestriction triggerRestriction;

    public bool Optional = false;
    public string blurb = "";
}
