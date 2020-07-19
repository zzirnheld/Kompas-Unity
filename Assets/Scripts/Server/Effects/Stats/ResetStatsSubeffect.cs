using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetStatsSubeffect : ServerSubeffect
{
    public bool resetN = false;
    public bool resetE = false;
    public bool resetS = false;
    public bool resetW = false;
    public bool resetC = false;
    public bool resetA = false;

    public override bool Resolve()
    {
        if (resetN) Target.SetN(Target.BaseN, Effect);
        if (resetE) Target.SetE(Target.BaseE, Effect);
        if (resetS) Target.SetS(Target.BaseS, Effect);
        if (resetW) Target.SetW(Target.BaseW, Effect);
        if (resetC) Target.SetC(Target.BaseC, Effect);
        if (resetA) Target.SetA(Target.BaseA, Effect);

        return ServerEffect.ResolveNextSubeffect();
    }
}
