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
        Target.AddToCharStats(
            ServerEffect.X * nMult + nMod,
            ServerEffect.X * eMult + eMod,
            ServerEffect.X * sMult + sMod,
            ServerEffect.X * wMult + wMod);
        ServerEffect.ResolveNextSubeffect();
    }
}
