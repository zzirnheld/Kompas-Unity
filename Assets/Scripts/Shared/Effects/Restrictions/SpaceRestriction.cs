using KompasCore.Cards;
using System;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects
{
    public class SpaceRestriction
    {
        public Subeffect Subeffect { get; private set; }
        public GameCard Source { get; private set; }
        public Player Controller => Source.Controller;
        public Effect Effect { get; private set; }

        #region space restrictions
        //adjacency
        public const string AdjacentToSource = "Adjacent to Source";
        public const string AdjacentToTarget = "Adjacent to Target";
        public const string AdjacentToCoords = "Adjacent to Coords";
        public const string AdjacentToWithRestriction = "Adjacent to a Card that Fits Restriction";

        public const string ConnectedToSourceBy = "Connected to Source by Cards Fitting Restriction";
        public const string ConnectedToSourceBySpaces = "Connected to Source by Spaces Fitting Restriction";
        public const string ConnectedToTargetBy = "Connected to Target by";
        public const string ConnectedToTargetBySpaces = "Connected to Target by Spaces Fitting Restriction";
        public const string ConnectedToTargetByXSpaces = "Connected to Target by X Spaces Fitting Restriction";
        public const string ConnectedToAvatarBy = "Connected to Avatar by";

        public const string InAOE = "In AOE";
        public const string NotInAOE = "Not In AOE";
        public const string InTargetsAOE = "In Target's AOE";
        public const string InAOEOf = "In AOE Of";
        public const string NotInAOEOf = "Not In AOE Of";
        public const string LimitAdjacentCardsFittingRestriction = "Limit Number of Adjacent Cards Fitting Restriction";
        public const string InAOEOfNumberFittingRestriction = "In AOE of Number of Cards Fitting Restriction";

        //distance
        public const string DistanceX = "Distance to Source == X";

        public const string DistanceToSourceFitsXRestriction = "Distance to Source Fits X Restriction";
        public const string DistanceToTargetFitsXRestriction = "Distance to Target Fits X Restriction";

        public const string DistanceToTargetX = "Distance to Target == X";
        public const string DistanceToTargetC = "Distance to Target == Constant";
        public const string DistanceToTargetLTEC = "Distance to Target <= Constant";

        public const string FurtherFromSourceThanTarget = "Further from Source than Target";
        public const string FurtherFromSourceThanCoords = "Further from Source than Coords";
        public const string TowardsSourceFromTarget = "Towards Source from Target";
        public const string TowardsTargetFromSource = "Towards Target from Source";
        public const string DirectlyAwayFromTarget = "Directly Away from Target";

        //misc
        public const string CanPlayTarget = "Can Play Target to This Space";
        public const string CanMoveTarget = "Can Move Target to This Space";
        public const string CanMoveSource = "Can Move Source to This Space";
        public const string Empty = "Empty";
        public const string CardHereFitsRestriction = "Card Here Fits Restriction";
        public const string OnTargetsDiagonal = "On Target's Diagonal";
        public const string OnEdge = "On Edge of Board";
        public const string Corner = "Corner";
        //TODO: eventually make a "targetdirection" subeffect that appends the direction as a Space to the list of coords,
        // then replace these with something comparing directions
        public const string SameDirectionFromTargetAsSpace = "Same Direction From Target As Source From Space";
        public const string OppositeDirectionFromTargetThanSpace = "Opposite Direction From Target Than Space";
        #endregion space restrictions

        public string[] spaceRestrictions;
        public CardRestriction adjacencyRestriction;
        public CardRestriction limitAdjacencyRestriction;
        public int adjacencyLimit;
        public CardRestriction connectednessRestriction;
        public SpaceRestriction spaceConnectednessRestriction;
        public CardRestriction hereFitsRestriction;
        public CardRestriction inAOEOfRestriction;

        public XRestriction distanceXRestriction;
        public XRestriction connectedSpacesXRestriction;
        public XRestriction numberOfCardsInAOEOfRestriction;

        public string[] playRestrictionsToIgnore = new string[0];

        public int constant;

        public string blurb = "";
        public bool mustBeEmpty = true;

        // Necessary because json doesn't let you have nice things, like constructors with arguments,
        // so I need to make sure manually that I've bothered to set up relevant arguments.
        private bool initialized = false;

        public void Initialize(GameCard source, Player controller, Effect effect)
        {
            Source = source;
            Effect = effect;

            adjacencyRestriction?.Initialize(source, effect);
            connectednessRestriction?.Initialize(source, effect);
            spaceConnectednessRestriction?.Initialize(source, controller, effect);
            limitAdjacencyRestriction?.Initialize(source, effect);
            hereFitsRestriction?.Initialize(source, effect);
            inAOEOfRestriction?.Initialize(source, effect);
            distanceXRestriction?.Initialize(source);
            numberOfCardsInAOEOfRestriction?.Initialize(source);

            initialized = true;
        }

        public void Initialize(Subeffect subeffect)
        {
            Initialize(subeffect.Source, subeffect.Controller, subeffect.Effect);
            Subeffect = subeffect;
            adjacencyRestriction?.Initialize(subeffect);
            connectednessRestriction?.Initialize(subeffect);
            spaceConnectednessRestriction?.Initialize(subeffect);
            limitAdjacencyRestriction?.Initialize(subeffect);
            hereFitsRestriction?.Initialize(subeffect);
        }

        /// <summary>
        /// Whether the given restriction is valid for the given space.
        /// </summary>
        /// <param name="restriction">The restriction to be evaluated</param>
        /// <param name="x">The x coordinate of the space</param>
        /// <param name="y">The y coordinate of the space</param>
        /// <param name="theoreticalTarget">If this space restriction is being considered with a theoretical additional target, this is it</param>
        /// <returns></returns>
        private bool RestrictionValid(string restriction, Space space, GameCard theoreticalTarget, ActivationContext context)
        {
            //would use ?? but GameCard inherits from monobehavior which overrides comparison with null
            var target = theoreticalTarget != null ? theoreticalTarget : Subeffect?.Target;

            switch (restriction)
            {
                //adjacency
                case AdjacentToSource:          return Source.IsAdjacentTo(space);
                case AdjacentToTarget:          return target.IsAdjacentTo(space);
                case AdjacentToCoords:          return space.AdjacentTo(Subeffect.Space);
                case AdjacentToWithRestriction: return Source.Game.boardCtrl.CardsAdjacentTo(space).Any(c => adjacencyRestriction.Evaluate(c, context));

                case ConnectedToSourceBy:       
                    return Source.Game.boardCtrl.ShortestPath(Subeffect.Source, space, connectednessRestriction, context) < 50;
                case ConnectedToSourceBySpaces: 
                    return Source.Game.boardCtrl.ShortestPath(Subeffect.Source.Position, space, s => spaceConnectednessRestriction.Evaluate(s, context)) < 50;
                case ConnectedToTargetBy:       
                    return Source.Game.boardCtrl.ShortestPath(target, space, connectednessRestriction, context) < 50;
                case ConnectedToTargetBySpaces:
                    return Source.Game.boardCtrl.ShortestPath(target.Position, space, s => spaceConnectednessRestriction.Evaluate(s, context)) < 50;
                case ConnectedToTargetByXSpaces:
                    return connectedSpacesXRestriction.Evaluate(Source.Game.boardCtrl.ShortestPath(target.Position, space, 
                        s => spaceConnectednessRestriction.Evaluate(s, context)));
                case ConnectedToAvatarBy:       return Source.Game.boardCtrl.ShortestPath(Source.Controller.Avatar, space, connectednessRestriction, context) < 50;

                case InAOE:                     return Source.SpaceInAOE(space);
                case NotInAOE:                  return !Source.SpaceInAOE(space);
                case InTargetsAOE:              return target.SpaceInAOE(space);
                case InAOEOf:                   return Source.Game.Cards.Any(c => c.SpaceInAOE(space) && inAOEOfRestriction.Evaluate(c, context));
                case NotInAOEOf:                return !Source.Game.Cards.Any(c => c.SpaceInAOE(space) && inAOEOfRestriction.Evaluate(c, context));
                case InAOEOfNumberFittingRestriction:
                    //return numberOfCardsInAOEOfRestriction.Evaluate(Source.Game.Cards.Count(c => c.SpaceInAOE((x, y)) && inAOERestriction.Evaluate(c)));
                    var count = Source.Game.Cards.Count(c => c.SpaceInAOE(space) && inAOEOfRestriction.Evaluate(c, context));
                    Debug.Log($"{space} is in the aoe of {count} cards fitting the restriction {inAOEOfRestriction}");
                    return numberOfCardsInAOEOfRestriction.Evaluate(count);
                case LimitAdjacentCardsFittingRestriction:
                    return Source.Game.boardCtrl.CardsAdjacentTo(space)
                        .Where(c => limitAdjacencyRestriction.Evaluate(c, context))
                        .Count() <= adjacencyLimit;

                //distance
                case DistanceX:                   return Source.DistanceTo(space) == Subeffect.Effect.X;

                case DistanceToSourceFitsXRestriction:    return distanceXRestriction.Evaluate(Source.DistanceTo(space));
                case DistanceToTargetFitsXRestriction:    return distanceXRestriction.Evaluate(target.DistanceTo(space));

                case DistanceToTargetX:           return target.DistanceTo(space) == Subeffect.Effect.X;
                case DistanceToTargetC:           return target.DistanceTo(space) == constant;
                case DistanceToTargetLTEC:        return target.DistanceTo(space) <= constant;
                case FurtherFromSourceThanTarget: return Source.DistanceTo(space) > Source.DistanceTo(target);
                case FurtherFromSourceThanCoords: return Source.DistanceTo(space) > Source.DistanceTo(Subeffect.Space);
                case TowardsSourceFromTarget:     return Source.DistanceTo(space) < Source.DistanceTo(target);
                case TowardsTargetFromSource:     return target.DistanceTo(space) < target.DistanceTo(Source);
                case DirectlyAwayFromTarget:      return target.SpaceDirectlyAwayFrom(space, Source);

                //misc
                case CanPlayTarget: 
                    return target.PlayRestriction.EvaluateEffectPlay(space, Subeffect.Effect, Subeffect.Player, context, ignoring: playRestrictionsToIgnore);
                case CanMoveTarget: return target.MovementRestriction.EvaluateEffectMove(space);
                case CanMoveSource: return Source.MovementRestriction.EvaluateEffectMove(space);
                case Empty: return Source.Game.boardCtrl.GetCardAt(space) == null;
                case CardHereFitsRestriction: return hereFitsRestriction.Evaluate(Source.Game.boardCtrl.GetCardAt(space), context);
                case OnTargetsDiagonal: return target.SameDiagonal(space);
                case OnEdge: return space.IsEdge;
                case Corner: return space.IsCorner;
                case SameDirectionFromTargetAsSpace:
                    return target.Position.DirectionFromThisTo(space) == Subeffect.Space.DirectionFromThisTo(Source.Position);
                case OppositeDirectionFromTargetThanSpace:
                    /*Debug.Log($"Comparing {space} and {Subeffect.Space}, w/r/t {Source.Position}\n" +
                        $"Directions:{Source.Position.DirectionFromThisTo(space)} to {Source.Position.DirectionFromThisTo(Subeffect.Space)}");*/
                    return Source.Position.DirectionFromThisTo(space) * -1 == Source.Position.DirectionFromThisTo(Subeffect.Space);
                default: throw new ArgumentException($"Invalid space restriction {restriction}", "restriction");
            }
        }

        private bool RestrictionValidWithDebug(string r, Space space, GameCard theoreticalTarget, ActivationContext context)
        {
            bool success = RestrictionValid(r, space, theoreticalTarget, context);
            if (!success) Debug.Log($"Space resetriction {r} was flouted by {space}");
            return success;
        }

        public bool Evaluate(Space space, ActivationContext context, GameCard theoreticalTarget = null)
        {
            if (!initialized) throw new ArgumentException("Space restriction not initialized!");
            if (!space.Valid) return false;
            if (mustBeEmpty && Source.Game.boardCtrl.GetCardAt(space) != null)
            {
                //Debug.Log($"Space for {Source.CardName} needed to be empty and wasn't.");
                return false;
            }

            return spaceRestrictions.All(r => RestrictionValid(r, space, theoreticalTarget, context));
        }

        public override string ToString() => JsonUtility.ToJson(this);
    }
}