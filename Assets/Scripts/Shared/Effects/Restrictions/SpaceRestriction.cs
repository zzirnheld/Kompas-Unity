using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpaceRestriction
{
    public Subeffect Subeffect { get; private set; }

    public enum SpaceRestrictions
    {
        CanSummonTarget = 0,
        Empty = 1,
        AdjacentToThisCard = 100,
        AdjacentToWithRestriction = 101,
        InAOE = 150,
        DistanceX = 200,
        DistanceToTargetX = 201,
        DistanceToTargetC = 251
    }

    public SpaceRestrictions[] restrictionsToCheck;
    public BoardRestriction adjacencyRestriction;

    public int C;

    public void Initialize(Subeffect subeffect)
    {
        this.Subeffect = subeffect;
    }

    private bool ExistsCardWithRestrictionAdjacentToCoords(BoardRestriction r, int x, int y)
    {
        for (int i = 0; i < 7; i++)
        {
            for(int j = 0; j < 7; j++)
            {
                Card c = Subeffect.Effect.Game.boardCtrl.GetCardAt(i, j);
                if (c != null && c.IsAdjacentTo(x, y) && r.Evaluate(c)) return true;
            }
        }

        return false;
    }

    public bool Evaluate((int x, int y) space) => Evaluate(space.x, space.y);

    public bool Evaluate(int x, int y)
    {
        Debug.Log($"Space restriction for {Subeffect.Source.name} evaluating {x}, {y}");
        if (!Subeffect.Effect.Game.boardCtrl.ValidIndices(x, y)) return false;

        foreach(SpaceRestrictions r in restrictionsToCheck)
        {
            switch (r)
            {
                case SpaceRestrictions.CanSummonTarget:
                    if (!Subeffect.Effect.Game.boardCtrl.CanSummonTo(Subeffect.Controller.index, x, y)) return false;
                    break;
                case SpaceRestrictions.Empty:
                    if (Subeffect.Effect.Game.boardCtrl.GetCardAt(x, y) != null) return false;
                    break;
                case SpaceRestrictions.AdjacentToThisCard:
                    if (!Subeffect.Source.IsAdjacentTo(x, y)) return false;
                    break;
                case SpaceRestrictions.AdjacentToWithRestriction:
                    if (!ExistsCardWithRestrictionAdjacentToCoords(adjacencyRestriction, x, y)) return false;
                    break;
                case SpaceRestrictions.InAOE:
                    if (!Subeffect.Source.SpaceInAOE(x, y)) return false;
                    break;
                case SpaceRestrictions.DistanceX:
                    if (Subeffect.Source.DistanceTo(x, y) != Subeffect.Effect.X) return false;
                    break;
                case SpaceRestrictions.DistanceToTargetX:
                    if (Subeffect.Target.DistanceTo(x, y) != Subeffect.Effect.X) return false;
                    break;
                case SpaceRestrictions.DistanceToTargetC:
                    if (Subeffect.Target.DistanceTo(x, y) != C) return false;
                    break;
                default:
                    Debug.LogError($"Unrecognized space restriction enum {r}");
                    break;
            }
        }

        return true;
    }
}
