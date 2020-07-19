using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceTargetSubeffect : ServerSubeffect
{
    public SpaceRestriction spaceRestriction;

    public override void Initialize(ServerEffect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        spaceRestriction.Initialize(this);
    }

    public override bool Resolve()
    {
        if (!ServerEffect.serverGame.ExistsSpaceTarget(spaceRestriction))
        {
            Debug.Log($"No valid coords exist for {ThisCard.CardName} effect");
            return ServerEffect.EffectImpossible();
        }
        else
        {
            EffectController.ServerNotifier.GetSpaceTarget(ThisCard, this);
            return false;
        }
    }

    public bool SetTargetIfValid(int x, int y)
    {
        //evaluate the target. if it's valid, confirm it as the target (that's what the true is for)
        if (spaceRestriction.Evaluate(x, y))
        {
            Debug.Log($"Adding {x}, {y} as coords");
            ServerEffect.Coords.Add((x, y));
            EffectController.ServerNotifier.AcceptTarget();
            ServerEffect.ResolveNextSubeffect();
            return true;
        }
        else Debug.Log($"{x}, {y} not valid");

        return false;
    }
}
