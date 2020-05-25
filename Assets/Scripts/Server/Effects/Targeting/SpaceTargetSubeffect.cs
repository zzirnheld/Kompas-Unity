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

    public override void Resolve()
    {
        if (!ServerEffect.serverGame.ExistsSpaceTarget(spaceRestriction))
        {
            Debug.Log($"No valid coords exist for {ThisCard.CardName} effect");
            ServerEffect.EffectImpossible();
        }
        else EffectController.ServerNotifier.GetSpaceTarget(ThisCard, this);
    }

    public bool SetTargetIfValid(int x, int y)
    {
        //evaluate the target. if it's valid, confirm it as the target (that's what the true is for)
        if (spaceRestriction.Evaluate(x, y))
        {
            ServerEffect.Coords.Add(new Vector2Int(x, y));
            EffectController.ServerNotifier.AcceptTarget();
            ServerEffect.ResolveNextSubeffect();
            Debug.Log($"Adding {x}, {y} as coords");
            return true;
        }
        else Debug.Log($"{x}, {y} not valid");

        return false;
    }
}
