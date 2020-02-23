using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceTargetSubeffect : Subeffect
{
    public SpaceRestriction spaceRestriction;

    public override void Initialize()
    {
        base.Initialize();
        Debug.Log("Space restriction is null? " + (spaceRestriction == null));
        spaceRestriction.subeffect = this;
    }

    public override void Resolve()
    {
        if (!parent.serverGame.ExistsSpaceTarget(spaceRestriction))
        {
            Debug.Log("No coords exist for " + parent.thisCard.CardName + " effect");
            parent.EffectImpossible();
            return;
        }

        parent.EffectController.ServerNotifier.GetSpaceTarget(parent.thisCard, spaceRestriction, parent.X);
    }

    public bool SetTargetIfValid(int x, int y)
    {
        //evaluate the target. if it's valid, confirm it as the target (that's what the true is for)
        if (spaceRestriction.Evaluate(x, y))
        {
            parent.coords.Add(new Vector2Int(x, y));
            parent.ResolveNextSubeffect();
            Debug.Log("Adding " + x + ", " + y + " as coords");
            return true;
        }
        else Debug.Log(x + ", " + y + " not valid");

        return false;
    }
}
