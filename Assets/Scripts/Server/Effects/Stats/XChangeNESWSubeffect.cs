using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class XChangeNESWSubeffect : ServerSubeffect
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
            ServerEffect.EffectImpossible();
            return;
        }

        ServerGame.SetStats(charCard,
            charCard.N + ServerEffect.X * nMult + nMod,
            charCard.E + ServerEffect.X * eMult + eMod,
            charCard.S + ServerEffect.X * sMult + sMod,
            charCard.W + ServerEffect.X * wMult + wMod);
        ServerEffect.ResolveNextSubeffect();
    }
}
