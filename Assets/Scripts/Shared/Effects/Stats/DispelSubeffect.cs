using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DispelSubeffect : Subeffect
{
    public override void Resolve()
    {
        ServerGame.Dispel(Target as SpellCard);
        Effect.ResolveNextSubeffect();
    }
}
