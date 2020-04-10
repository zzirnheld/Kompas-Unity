using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Removes any effect currently set to trigger if an effect is declared impossible.
/// </summary>
public class ClearOnImpossibleSubeffect : Subeffect
{
    public override void Resolve()
    {
        Effect.OnImpossible = null;
        Effect.ResolveNextSubeffect();
    }
}
