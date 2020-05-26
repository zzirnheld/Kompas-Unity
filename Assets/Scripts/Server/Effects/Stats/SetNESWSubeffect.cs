using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetNESWSubeffect : ServerSubeffect
{
    public int NVal = -1;
    public int EVal = -1;
    public int SVal = -1;
    public int WVal = -1;

    public CharacterCard CharTarget { get => Target as CharacterCard; }

    public int RealNVal { get => NVal < 0 ? CharTarget.N : NVal; }
    public int RealEVal { get => EVal < 0 ? CharTarget.E : EVal; }
    public int RealSVal { get => SVal < 0 ? CharTarget.S : SVal; }
    public int RealWVal { get => WVal < 0 ? CharTarget.W : WVal; }

    public override void Resolve()
    {
        ServerGame.SetStats(CharTarget, RealNVal, RealEVal, RealSVal, RealWVal);
        ServerEffect.ResolveNextSubeffect();
    }
}
