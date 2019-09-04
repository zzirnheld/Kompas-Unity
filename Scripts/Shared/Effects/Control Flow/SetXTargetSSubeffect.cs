﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetXTargetSSubeffect : Subeffect
{
    public override void Resolve()
    {
        if (!(Target is CharacterCard charTarget))
        {
            parent.EffectImpossible();
            return;
        }

        parent.X = charTarget.S;
        ServerGame.serverNetworkCtrl.NotifyEffectX(ServerGame, parent.thisCard, parent.EffectIndex, parent.X);
        parent.ResolveNextSubeffect();
    }
}
