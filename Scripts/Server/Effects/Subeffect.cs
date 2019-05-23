using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Subeffect
{
    [System.NonSerialized] public Effect parent;
    
    /// <summary>
    /// parent resolve method. at the end, needs to call resolve subeffect in parent
    /// if it's an if, it does a specific index
    /// otherwise, it does currentIndex + 1
    /// </summary>
    public virtual void Resolve()
    {
        Debug.Log("Resolving a base subeffect. nothing happens");
    }

    /// <summary>
    /// Called by restrictions that have found a valid target to add to the list
    /// </summary>
    /// <param name="card"></param>
    public void ContinueResolutionWith(Card card)
    {
        parent.targets.Add(card);
        parent.ResolveSubeffect(parent.effectIndex + 1);
    }
}
