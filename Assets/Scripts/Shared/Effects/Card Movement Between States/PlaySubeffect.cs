﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        ServerGame.Play(Target, X, Y, EffectController, Effect);
        Effect.ResolveNextSubeffect();
    }
}
