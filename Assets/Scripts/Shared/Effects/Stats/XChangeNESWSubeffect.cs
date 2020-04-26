using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XChangeNESWSubeffect : Subeffect
{
    int nMult = 0;
    int eMult = 0;
    int sMult = 0;
    int wMult = 0;

    int nMod = 0;
    int eMod = 0;
    int sMod = 0;
    int wMod = 0;
    
    public override void Resolve()
    {
        if (!(Target is CharacterCard charCard))
        {
            Effect.EffectImpossible();
            return;
        }

        ServerGame.SetStats(charCard,
            charCard.N * nMult + nMod,
            charCard.E * eMult + eMod,
            charCard.S * sMult + sMod,
            charCard.W * wMult + wMod);
        Effect.ResolveNextSubeffect();
    }
}
