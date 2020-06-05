using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetNESWSubeffect : ServerSubeffect
{
    public int NVal = -1;
    public int EVal = -1;
    public int SVal = -1;
    public int WVal = -1;

    public CharacterCard CharTarget => Target as CharacterCard;

    public int RealNVal => NVal <= 0 ? CharTarget.N : NVal;
    public int RealEVal => EVal <= 0 ? CharTarget.E : EVal;
    public int RealSVal => SVal <= 0 ? CharTarget.S : SVal;
    public int RealWVal => WVal <= 0 ? CharTarget.W : WVal;

    public override void Resolve()
    {
        ServerGame.SetStats(CharTarget, RealNVal, RealEVal, RealSVal, RealWVal);
        ServerEffect.ResolveNextSubeffect();
    }
}
