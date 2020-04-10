using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpaceRestriction
{
    public Subeffect Subeffect;

    public enum SpaceRestrictions
    {
        CanSummonTarget = 0,
        AdjacentToThisCard = 100,
        AdjacentToWithRestriction = 101
    }

    public SpaceRestrictions[] restrictionsToCheck;
    public BoardRestriction adjacencyRestriction;

    private bool ExistsCardWithRestrictionAdjacentToCoords(BoardRestriction r, int x, int y)
    {
        bool? adj;

        for (int i = 0; i < 7; i++)
        {
            for(int j = 0; j < 7; j++)
            {
                adj = Subeffect.Effect.serverGame.boardCtrl.GetCardAt(i, j)?.IsAdjacentTo(x, y);
                if (adj.HasValue && adj.Value) return true; //if the card exists (so adj is not null) and the card is adjacent, return true
            }
        }

        return false;
    }

    public bool Evaluate(int x, int y)
    {
        Debug.Log("Evaluating " + x + ", " + y);
        if (!Subeffect.Effect.serverGame.boardCtrl.ValidIndices(x, y)) return false;

        foreach(SpaceRestrictions r in restrictionsToCheck)
        {
            switch (r)
            {
                case SpaceRestrictions.CanSummonTarget:
                    if (!Subeffect.Effect.serverGame.boardCtrl.CanSummonTo(Subeffect.Effect.effectControllerIndex, x, y)) return false;
                    break;
                case SpaceRestrictions.AdjacentToThisCard:
                    if (!Subeffect.ThisCard.IsAdjacentTo(x, y)) return false;
                    break;
                case SpaceRestrictions.AdjacentToWithRestriction:
                    if (!ExistsCardWithRestrictionAdjacentToCoords(adjacencyRestriction, x, y)) return false;
                    break;
            }
        }

        return true;
    }
}
