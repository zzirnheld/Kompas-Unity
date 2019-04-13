using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Subeffect
{
    private Effect parent;

    public Subeffect(Effect parent)
    {
        this.parent = parent;
    }

    public virtual void Resolve()
    {
        Debug.Log("Resolving a base subeffect. nothing happens");
    }

}
