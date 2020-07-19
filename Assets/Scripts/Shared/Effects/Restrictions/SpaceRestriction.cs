using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SpaceRestriction
{
    public Subeffect Subeffect { get; private set; }

    public const string CanPlayTarget = "Can Play Target to This Space"; //0,
    public const string Empty = "Empty"; //1,
    public const string AdjacentToThisCard = "Adjacent to Source"; //100,
    public const string AdjacentToWithRestriction = "Adjacent to a Card that Fits Restriction"; //101,
    public const string AdjacentToTarget = "Adjacent to Target"; //102,
    public const string ConnectedToSourceBy = "Connected to Source by Cards Fitting Restriction"; //110,
    public const string InAOE = "In AOE"; //150,
    public const string DistanceX = "Distance to Source == X"; //200,
    public const string DistanceToTargetX = "Distance to Target == X"; //201,
    public const string DistanceToTargetC = "Distance to Target == Constant"; //251,
    public const string FurtherFromSourceThanTarget = "Further from Source than Target"; //260

    public string[] spaceRestrictions;
    public CardRestriction adjacencyRestriction = new CardRestriction();
    public CardRestriction connectednessRestriction = new CardRestriction(); 

    public int constant;

    public string blurb = "";

    public void Initialize(Subeffect subeffect)
    {
        this.Subeffect = subeffect;
        adjacencyRestriction.Initialize(subeffect);
        connectednessRestriction.Initialize(subeffect);
    }

    public bool Evaluate((int x, int y) space) => Evaluate(space.x, space.y);

    public bool Evaluate(int x, int y)
    {
        Debug.Log($"Space restriction for {Subeffect.Source.CardName} evaluating {x}, {y}");
        if (!Subeffect.Effect.Game.boardCtrl.ValidIndices(x, y)) return false;

        foreach(var r in spaceRestrictions)
        {
            switch (r)
            {
                case CanPlayTarget:
                    if (!Subeffect.Effect.Game.boardCtrl.CanPlayTo(Subeffect.Controller.index, x, y)) return false;
                    break;
                case Empty:
                    if (Subeffect.Effect.Game.boardCtrl.GetCardAt(x, y) != null) return false;
                    break;
                case AdjacentToThisCard:
                    if (!Subeffect.Source.IsAdjacentTo(x, y)) return false;
                    break;
                case AdjacentToWithRestriction:
                    if (!Subeffect.Game.boardCtrl.CardsAdjacentTo(x,y).Any(c => adjacencyRestriction.Evaluate(c))) return false;
                    break;
                case AdjacentToTarget:
                    if (!Subeffect.Target.IsAdjacentTo(x, y)) return false;
                    break;
                case ConnectedToSourceBy:
                    if (Subeffect.Game.boardCtrl.ShortestPath(Subeffect.Source, x, y, connectednessRestriction) >= 50) return false;
                    break;
                case InAOE:
                    if (!Subeffect.Source.SpaceInAOE(x, y)) return false;
                    break;
                case DistanceX:
                    if (Subeffect.Source.DistanceTo(x, y) != Subeffect.Effect.X) return false;
                    break;
                case DistanceToTargetX:
                    if (Subeffect.Target.DistanceTo(x, y) != Subeffect.Effect.X) return false;
                    break;
                case DistanceToTargetC:
                    if (Subeffect.Target.DistanceTo(x, y) != constant) return false;
                    break;
                case FurtherFromSourceThanTarget:
                    if (Subeffect.Source.DistanceTo(x, y) <= Subeffect.Source.DistanceTo(Subeffect.Target)) return false;
                    break;
                default:
                    Debug.LogError($"Unrecognized space restriction enum {r}");
                    break;
            }
        }

        return true;
    }
}
