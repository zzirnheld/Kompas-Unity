using KompasCore.Cards;
using System;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects
{
    [Serializable]
    public class SpaceRestriction
    {
        public Subeffect Subeffect { get; private set; }
        public GameCard Source { get; private set; }
        public Player Controller { get; private set; }
        public Effect Effect { get; private set; }

        //adjacency
        public const string AdjacentToThisCard = "Adjacent to Source";
        public const string AdjacentToWithRestriction = "Adjacent to a Card that Fits Restriction";
        public const string AdjacentToTarget = "Adjacent to Target";
        public const string ConnectedToSourceBy = "Connected to Source by Cards Fitting Restriction";
        public const string InAOE = "In AOE";
        public const string NotInAOE = "Not In AOE";
        public const string LimitAdjacentCardsFittingRestriction = "Limit Number of Adjacent Cards Fitting Restriction";

        //distance
        public const string DistanceX = "Distance to Source == X";
        public const string DistanceToTargetX = "Distance to Target == X";
        public const string DistanceToTargetC = "Distance to Target == Constant";
        public const string DistanceToTargetLTEC = "Distance to Target <= Constant";
        public const string FurtherFromSourceThanTarget = "Further from Source than Target";
        public const string TowardsSourceFromTarget = "Towards Source from Target";
        public const string DirectlyAwayFromTarget = "Directly Away from Target";

        //misc
        public const string CanPlayTarget = "Can Play Target to This Space";
        public const string CanMoveTarget = "Can Move Target to This Space";
        public const string Empty = "Empty";
        public const string CardHereFitsRestriction = "Card Here Fits Restriction";
        public const string OnTargetsDiagonal = "On Target's Diagonal";
        public const string OnEdge = "On Edge of Board";

        public string[] spaceRestrictions;
        public CardRestriction adjacencyRestriction;
        public CardRestriction limitAdjacencyRestriction;
        public int adjacencyLimit;
        public CardRestriction connectednessRestriction;
        public CardRestriction hereFitsRestriction;

        public int constant;

        public string blurb = "";
        public bool mustBeEmpty = true;

        // Necessary because json doesn't let you have nice things, like constructors with arguments,
        // so I need to make sure manually that I've bothered to set up relevant arguments.
        private bool initialized = false;

        public void Initialize(GameCard source, Player controller, Effect effect)
        {
            Source = source;
            Controller = controller;
            Effect = effect;

            adjacencyRestriction = adjacencyRestriction ?? new CardRestriction();
            connectednessRestriction = connectednessRestriction ?? new CardRestriction();
            limitAdjacencyRestriction = limitAdjacencyRestriction ?? new CardRestriction();
            hereFitsRestriction = hereFitsRestriction ?? new CardRestriction();

            adjacencyRestriction.Initialize(source, controller, effect);
            connectednessRestriction.Initialize(source, controller, effect);
            limitAdjacencyRestriction.Initialize(source, controller, effect);
            hereFitsRestriction.Initialize(source, controller, effect);

            initialized = true;
        }

        public void Initialize(Subeffect subeffect)
        {
            Initialize(subeffect.Source, subeffect.Controller, subeffect.Effect);
            Subeffect = subeffect;
            adjacencyRestriction.Initialize(subeffect);
            connectednessRestriction.Initialize(subeffect);
            limitAdjacencyRestriction.Initialize(subeffect);
            hereFitsRestriction.Initialize(subeffect);
        }

        private bool RestrictionValid(string restriction, int x, int y)
        {
            switch (restriction)
            {
                //adjacency
                case AdjacentToThisCard:        return Source.IsAdjacentTo(x, y);
                case AdjacentToWithRestriction: return Source.Game.boardCtrl.CardsAdjacentTo(x, y).Any(c => adjacencyRestriction.Evaluate(c));
                case AdjacentToTarget:          return Subeffect.Target.IsAdjacentTo(x, y);
                case ConnectedToSourceBy:       return Source.Game.boardCtrl.ShortestPath(Subeffect.Source, x, y, connectednessRestriction) < 50;
                case InAOE:                     return Source.SpaceInAOE(x, y);
                case NotInAOE:                  return !Source.SpaceInAOE(x, y);
                case LimitAdjacentCardsFittingRestriction:
                    return Source.Game.boardCtrl.CardsAdjacentTo(x, y).Where(c => limitAdjacencyRestriction.Evaluate(c)).Count() <= adjacencyLimit;

                //distance
                case DistanceX:                   return Source.DistanceTo(x, y) == Subeffect.Effect.X;
                case DistanceToTargetX:           return Subeffect.Target.DistanceTo(x, y) == Subeffect.Effect.X;
                case DistanceToTargetC:           return Subeffect.Target.DistanceTo(x, y) == constant;
                case DistanceToTargetLTEC:        return Subeffect.Target.DistanceTo(x, y) <= constant;
                case FurtherFromSourceThanTarget: return Source.DistanceTo(x, y) > Source.DistanceTo(Subeffect.Target);
                case TowardsSourceFromTarget:     return Source.DistanceTo(x, y) < Source.DistanceTo(Subeffect.Target);
                case DirectlyAwayFromTarget:      return Subeffect.Target.SpaceDirectlyAwayFrom((x, y), Source);

                //misc
                case CanPlayTarget: return Subeffect.Target.PlayRestriction.EvaluateEffectPlay(x, y, Subeffect.Effect, Subeffect.Player);
                case CanMoveTarget: return Subeffect.Target.MovementRestriction.EvaluateEffectMove(x, y);
                case Empty: return Source.Game.boardCtrl.GetCardAt(x, y) == null;
                case CardHereFitsRestriction: return hereFitsRestriction.Evaluate(Source.Game.boardCtrl.GetCardAt(x, y));
                case OnTargetsDiagonal: return Subeffect.Target.OnMyDiagonal((x, y));
                case OnEdge: return x == 0 || x == 6 || y == 0 || y == 6;
                default: throw new ArgumentException($"Invalid space restriction {restriction}", "restriction");
            }
        }

        public bool Evaluate((int x, int y) space) => Evaluate(space.x, space.y);

        public bool Evaluate(int x, int y)
        {
            if (!initialized) throw new System.ArgumentException("Space restriction not initialized!");
            if (!Source.Game.boardCtrl.ValidIndices(x, y)) return false;
            if (mustBeEmpty && Source.Game.boardCtrl.GetCardAt(x, y) != null) return false;

            return spaceRestrictions.All(r => RestrictionValid(r, x, y));
        }

        public override string ToString() => JsonUtility.ToJson(this);
    }
}