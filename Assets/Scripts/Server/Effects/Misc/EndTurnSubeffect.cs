using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnSubeffect : ServerSubeffect
{
    public override bool Resolve()
    {
        ServerGame.SwitchTurn();
        return ServerEffect.ResolveNextSubeffect();
    }
}
