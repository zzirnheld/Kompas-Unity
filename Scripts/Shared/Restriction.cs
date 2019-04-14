using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Restriction
{
    [System.NonSerialized] public Subeffect subeffect;

    public virtual bool Evaluate()
    {
        Debug.Log("Parent restriction always returns true");
        return true;
    }
}
