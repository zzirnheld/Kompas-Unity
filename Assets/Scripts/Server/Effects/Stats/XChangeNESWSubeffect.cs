using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class XChangeNESWSubeffect : Subeffect
{
    public int nMult = 0;
    public int eMult = 0;
    public int sMult = 0;
    public int wMult = 0;

    public int nMod = 0;
    public int eMod = 0;
    public int sMod = 0;
    public int wMod = 0;
    
    public override void Resolve()
    {
        if (!(Target is CharacterCard charCard))
        {
            Effect.EffectImpossible();
            return;
        }

        ServerGame.SetStats(charCard,
            charCard.N + Effect.X * nMult + nMod,
            charCard.E + Effect.X * eMult + eMod,
            charCard.S + Effect.X * sMult + sMod,
            charCard.W + Effect.X * wMult + wMod);
        Effect.ResolveNextSubeffect();
    }
}
