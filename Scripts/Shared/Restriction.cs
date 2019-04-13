using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Restriction
{
    public enum GenericRestrictions { }
    public int distanceRequirement;
    public bool sameDiagonal;
    
    public GenericRestrictions[] restrictions;

    public virtual bool Evaluate()
    {
        Debug.Log("Parent restriction always returns false");
        return false;
    }
}
