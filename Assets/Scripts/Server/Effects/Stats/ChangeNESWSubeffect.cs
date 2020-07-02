using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChangeNESWSubeffect : ServerSubeffect
{
    public int nChange = 0;
    public int eChange = 0;
    public int sChange = 0;
    public int wChange = 0;

    public override void Resolve()
    {
        Target.AddToCharStats(nChange, eChange, sChange, wChange);
        ServerEffect.ResolveNextSubeffect();
    }
}
