using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetNESWSubeffect : ServerSubeffect
{
    public int NVal = -1;
    public int EVal = -1;
    public int SVal = -1;
    public int WVal = -1;

    public int RealNVal => NVal < 0 ? Target.N : NVal;
    public int RealEVal => EVal < 0 ? Target.E : EVal;
    public int RealSVal => SVal < 0 ? Target.S : SVal;
    public int RealWVal => WVal < 0 ? Target.W : WVal;

    public override void Resolve()
    {
        Target.SetCharStats(RealNVal, RealEVal, RealSVal, RealWVal);
        ServerEffect.ResolveNextSubeffect();
    }
}
