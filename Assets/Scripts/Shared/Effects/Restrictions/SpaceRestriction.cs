using KompasCore.Cards;
using System;
using System.Collections.Generic;
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

        #region space restrictions
        //adjacency
        public const string AdjacentToThisCard = "Adjacent to Source";
        public const string AdjacentToWithRestriction = "Adjacent to a Card that Fits Restriction";
        public const string AdjacentToTarget = "Adjacent to Target";
        public const string ConnectedToSourceBy = "Connected to Source by Cards Fitting Restriction";
        public const string ConnectedToTargetBy = "Connected to Target by";
        public const string ConnectedToAvatarBy = "Connected to Avatar by";
        public const string InAOE = "In AOE";
        public const string NotInAOE = "Not In AOE";
        public const string InTargetsAOE = "In Target's AOE";
        public const string InAOEOf = "In AOE Of";
        public const string LimitAdjacentCardsFittingRestriction = "Limit Number of Adjacent Cards Fitting Restriction";
        public const string InAOEOfNumberFittingRestriction = "In AOE of Number of Cards Fitting Restriction";

        //distance
        public const string DistanceX = "Distance to Source == X";
        public const string DistanceFitsXRestriction = "Distance to Source Fits X Restriction";
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
        public const string Corner = "Corner";
        #endregion space restrictions

        public string[] spaceRestrictions;
        public CardRestriction adjacencyRestriction;
        public CardRestriction limitAdjacencyRestriction;
        public int adjacencyLimit;
        public CardRestriction connectednessRestriction;
        public CardRestriction hereFitsRestriction;
        public CardRestriction inAOERestriction = new CardRestriction();

        public XRestriction distanceXRestriction;
        public XRestriction numberOfCardsInAOEOfRestriction;

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
            inAOERestriction?.Initialize(source, controller, effect);
            distanceXRestriction?.Initialize(source);
            numberOfCardsInAOEOfRestriction?.Initialize(source);

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

        /// <summary>
        /// Whether the given restriction is valid for the given space.
        /// </summary>
        /// <param name="restriction">The restriction to be evaluated</param>
        /// <param name="x">The x coordinate of the space</param>
        /// <param name="y">The y coordinate of the space</param>
        /// <param name="theoreticalTarget">If this space restriction is being considered with a theoretical additional target, this is it</param>
        /// <returns></returns>
        private bool RestrictionValid(string restriction, int x, int y, GameCard theoreticalTarget)
        {
            //would use ?? but GameCard inherits from monobehavior which overrides comparison with null
            var target = theoreticalTarget != null ? theoreticalTarget : Subeffect?.Target;

            switch (restriction)
            {
                //adjacency
                case AdjacentToThisCard:        return Source.IsAdjacentTo(x, y);
                case AdjacentToWithRestriction: return Source.Game.boardCtrl.CardsAdjacentTo(x, y).Any(c => adjacencyRestriction.Evaluate(c));
                case AdjacentToTarget:          return target.IsAdjacentTo(x, y);
                case ConnectedToSourceBy:       return Source.Game.boardCtrl.ShortestPath(Subeffect.Source, x, y, connectednessRestriction) < 50;
                case ConnectedToTargetBy:       return Source.Game.boardCtrl.ShortestPath(Subeffect.Target, x, y, connectednessRestriction) < 50;
                case ConnectedToAvatarBy:       return Source.Game.boardCtrl.ShortestPath(Source.Controller.Avatar, x, y, connectednessRestriction) < 50;
                case InAOE:                     return Source.SpaceInAOE(x, y);
                case NotInAOE:                  return !Source.SpaceInAOE(x, y);
                case InTargetsAOE:              return target.SpaceInAOE((x, y));
                case InAOEOf:                   return Source.Game.Cards.Any(c => c.SpaceInAOE((x, y)) && inAOERestriction.Evaluate(c));
                case InAOEOfNumberFittingRestriction:
                    //return numberOfCardsInAOEOfRestriction.Evaluate(Source.Game.Cards.Count(c => c.SpaceInAOE((x, y)) && inAOERestriction.Evaluate(c)));
                    var count = Source.Game.Cards.Count(c => c.SpaceInAOE((x, y)) && inAOERestriction.Evaluate(c));
                    Debug.Log($"{x}, {y} is in the aoe of {count} cards fitting the restriction {inAOERestriction}");
                    return numberOfCardsInAOEOfRestriction.Evaluate(count);
                case LimitAdjacentCardsFittingRestriction:
                    return Source.Game.boardCtrl.CardsAdjacentTo(x, y).Where(c => limitAdjacencyRestriction.Evaluate(c)).Count() <= adjacencyLimit;

                //distance
                case DistanceX:                   return Source.DistanceTo(x, y) == Subeffect.Effect.X;
                case DistanceFitsXRestriction:    return distanceXRestriction.Evaluate(Source.DistanceTo(x, y));
                case DistanceToTargetX:           return target.DistanceTo(x, y) == Subeffect.Effect.X;
                case DistanceToTargetC:           return target.DistanceTo(x, y) == constant;
                case DistanceToTargetLTEC:        return target.DistanceTo(x, y) <= constant;
                case FurtherFromSourceThanTarget: return Source.DistanceTo(x, y) > Source.DistanceTo(target);
                case TowardsSourceFromTarget:     return Source.DistanceTo(x, y) < Source.DistanceTo(target);
                case DirectlyAwayFromTarget:      return target.SpaceDirectlyAwayFrom((x, y), Source);

                //misc
                case CanPlayTarget: return target.PlayRestriction.EvaluateEffectPlay(x, y, Subeffect.Effect, Subeffect.Player);
                case CanMoveTarget: return target.MovementRestriction.EvaluateEffectMove(x, y);
                case Empty: return Source.Game.boardCtrl.GetCardAt(x, y) == null;
                case CardHereFitsRestriction: return hereFitsRestriction.Evaluate(Source.Game.boardCtrl.GetCardAt(x, y));
                case OnTargetsDiagonal: return target.OnMyDiagonal((x, y));
                case OnEdge: return x == 0 || x == 6 || y == 0 || y == 6;
                case Corner: return (x == 0 || x == 6) && (y == 0 || y == 6);
                default: throw new ArgumentException($"Invalid space restriction {restriction}", "restriction");
            }
        }

        private bool RestrictionValidWithDebug(string r, int x, int y, GameCard theoreticalTarget)
        {
            bool success = RestrictionValid(r, x, y, theoreticalTarget);
            if (!success) Debug.Log($"Space resetriction {r} was flouted by {x}, {y}");
            return success;
        }

        public bool Evaluate((int x, int y) space, GameCard theoreticalTarget = null) => Evaluate(space.x, space.y, theoreticalTarget);

        public bool Evaluate(int x, int y, GameCard theoreticalTarget = null)
        {
            if (!initialized) throw new ArgumentException("Space restriction not initialized!");
            if (!Source.Game.boardCtrl.ValidIndices(x, y)) return false;
            if (mustBeEmpty && Source.Game.boardCtrl.GetCardAt(x, y) != null)
            {
                //Debug.Log($"Space for {Source.CardName} needed to be empty and wasn't.");
                return false;
            }

            return spaceRestrictions.All(r => RestrictionValid(r, x, y, theoreticalTarget));
        }

        public override string ToString() => JsonUtility.ToJson(this);
    }
}