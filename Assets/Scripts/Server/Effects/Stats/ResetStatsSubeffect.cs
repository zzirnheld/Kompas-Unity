using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetStatsSubeffect : ServerSubeffect
{
    public bool ResetN = false;
    public bool ResetE = false;
    public bool ResetS = false;
    public bool ResetW = false;
    public bool ResetC = false;
    public bool ResetA = false;

    public override bool Resolve()
    {
        if (ResetN) Target.SetN(Target.BaseN, Effect);
        if (ResetE) Target.SetE(Target.BaseE, Effect);
        if (ResetS) Target.SetS(Target.BaseS, Effect);
        if (ResetW) Target.SetW(Target.BaseW, Effect);
        if (ResetC) Target.SetC(Target.BaseC, Effect);
        if (ResetA) Target.SetA(Target.BaseA, Effect);

        return ServerEffect.ResolveNextSubeffect();
    }
}
