using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChooseXSubeffect : Subeffect
{
    public XRestriction XRest;

    public override void Initialize(Effect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        XRest.Subeffect = this;
    }

    private void AskForX()
    {
        EffectController.ServerNotifier.GetXForEffect(ThisCard, Effect.EffectIndex, SubeffIndex);
    }

    public override void Resolve()
    {
        AskForX();
    }

    public void SetXIfLegal(int x)
    {
        if (XRest.Evaluate(x))
        {
            Effect.X = x;
            Effect.ResolveNextSubeffect();
        }
        else AskForX();
    }
}
