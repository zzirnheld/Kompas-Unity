using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Subeffect
{
    public abstract Effect Effect { get; }
    public abstract Player Controller { get; }

    public int SubeffIndex { get; protected set; }

    public Card Source => Effect.Source;
}
