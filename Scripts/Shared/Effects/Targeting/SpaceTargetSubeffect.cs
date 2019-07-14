using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceTargetSubeffect : Subeffect
{
    public SpaceRestriction spaceRestriction;

    public override void Initialize()
    {
        base.Initialize();
        spaceRestriction.subeffect = this;
    }

    public override void Resolve()
    {
        if (!parent.serverGame.ExistsSpaceTarget(spaceRestriction))
        {
            Debug.Log("No coords exist for " + parent.thisCard.CardName + " effect");
            parent.NoTargetExists();
            return;
        }

        parent.serverGame.serverNetworkCtrl
            .GetSpaceTarget(parent.serverGame, parent.effectController, parent.thisCard,
                                            System.Array.IndexOf(parent.thisCard.Effects, parent),
                                            parent.effectIndex);
    }

    public bool Target(int x, int y)
    {
        //evaluate the target. if it's valid, confirm it as the target (that's what the true is for)
        if (spaceRestriction.Evaluate(x, y))
        {
            parent.coords.Add(new Vector2Int(x, y));
            parent.serverGame.serverNetworkCtrl.AcceptTarget(parent.serverGame, parent.serverGame.Players[parent.effectController].ConnectionID);
            parent.ResolveNextSubeffect();
            Debug.Log("Adding " + x + ", " + y + " as coords");
            return true;
        }

        return false;
    }
}
