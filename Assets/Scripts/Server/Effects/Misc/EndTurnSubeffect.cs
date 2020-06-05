using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnSubeffect : ServerSubeffect
{
    public override void Resolve()
    {
        ServerGame.SwitchTurn();
        ServerEffect.ResolveNextSubeffect();
    }
}
