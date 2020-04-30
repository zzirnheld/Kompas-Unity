using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceTargetSubeffect : ServerSubeffect
{
    public SpaceRestriction spaceRestriction;

    public override void Initialize(ServerEffect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        Debug.Log("Space restriction is null? " + (spaceRestriction == null));
        spaceRestriction.Subeffect = this;
    }

    public override void Resolve()
    {
        if (!ServerEffect.serverGame.ExistsSpaceTarget(spaceRestriction))
        {
            Debug.Log("No coords exist for " + ThisCard.CardName + " effect");
            ServerEffect.EffectImpossible();
            return;
        }

        EffectController.ServerNotifier.GetSpaceTarget(ThisCard, this);
    }

    public bool SetTargetIfValid(int x, int y)
    {
        //evaluate the target. if it's valid, confirm it as the target (that's what the true is for)
        if (spaceRestriction.Evaluate(x, y))
        {
            ServerEffect.coords.Add(new Vector2Int(x, y));
            ServerEffect.ResolveNextSubeffect();
            Debug.Log("Adding " + x + ", " + y + " as coords");
            return true;
        }
        else Debug.Log(x + ", " + y + " not valid");

        return false;
    }
}
