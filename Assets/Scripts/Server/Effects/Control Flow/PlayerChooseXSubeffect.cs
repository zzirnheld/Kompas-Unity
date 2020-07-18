using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChooseXSubeffect : ServerSubeffect
{
    public XRestriction XRest;

    public override void Initialize(ServerEffect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        XRest.Subeffect = this;
    }

    private void AskForX()
    {
        EffectController.ServerNotifier.GetXForEffect(ThisCard, ServerEffect.EffectIndex, SubeffIndex);
    }

    public override bool Resolve()
    {
        AskForX();
        return false;
    }

    public void SetXIfLegal(int x)
    {
        if (XRest.Evaluate(x))
        {
            ServerEffect.X = x;
            ServerEffect.ResolveNextSubeffect();
        }
        else AskForX();
    }
}
