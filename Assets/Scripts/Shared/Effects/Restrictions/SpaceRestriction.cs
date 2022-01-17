using KompasCore.Cards;
using KompasCore.Exceptions;
using KompasCore.GameCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects
{
    public class SpaceRestriction
    {
        public Subeffect Subeffect { get; private set; }
        public GameCard Source { get; private set; }
        public Game Game => Source.Game;
        public Player Controller => Source.Controller;
        public Effect Effect { get; private set; }

        #region space restrictions
        //adjacency
        public const string AdjacentToSource = "Adjacent to Source";
        public const string AdjacentToTarget = "Adjacent to Target";
        public const string AdjacentToCoords = "Adjacent to Coords";
        public const string AdjacentToCardRestriction = "Adjacent to a Card that Fits Restriction";

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
        public const string InAOESourceAlsoIn = "In AOE Source is Also In";

        public const string SourceDisplacementToSpaceMatchesCoords = "Source Displacement to Space Matches Coords";
        public const string SourceToSpaceSameDirectionAsCoords = "Source to Space Same Direction as Coords";
        public const string SourceToTargetSameDirectionAsCoords = "Source to Target Same Direction as Coords";
        public const string SubjectiveDisplacementFromSource = "Subjective Displacement from Source";
        public const string BehindSource = "Behind Source";

        //distance
        public const string DistanceToSourceFitsXRestriction = "Distance to Source Fits X Restriction";
        public const string DistanceToTargetFitsXRestriction = "Distance to Target Fits X Restriction";
        public const string DistanceToCoordsFitsXRestriction = "Distance to Coords Fits X Restriction";

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
        public const string Surrounded = "Surrounded";
        public const string CardHereFitsRestriction = "Card Here Fits Restriction";

        public const string OnSourcesDiagonal = "On Source's Diagonal";
        public const string OnTargetsDiagonal = "On Target's Diagonal";
        public const string OnAxisOfLastTwoSpaces = "On Axis of Last Two Spaces";

        public const string OnEdge = "On Edge of Board";
        public const string Corner = "Corner";
        //TODO: eventually make a "targetdirection" subeffect that appends the direction as a Space to the list of coords,
        // then replace these with something comparing directions
        public const string SameDirectionFromTargetAsSpace = "Same Direction From Target As Source From Space";
        public const string OppositeDirectionFromTargetThanSpace = "Opposite Direction From Target Than Space";
        public const string DirectionFromSource = "Direction From Source"; //In the direction of Subeffect.Space from source
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

        public int displacementX;
        public int displacementY;

        public string blurb = "";
        public bool mustBeEmpty = true;

        // Necessary because json doesn't let you have nice things, like constructors with arguments,
        // so I need to make sure manually that I've bothered to set up relevant arguments.
        private bool initialized = false;

        public void Initialize(Subeffect subeffect) => Initialize(subeffect.Source, subeffect.Controller, subeffect.Effect, subeffect);

        public void Initialize(GameCard source, Player controller, Effect effect, Subeffect subeffect)
        {
            Subeffect = subeffect;
            Source = source;
            Effect = effect;

            adjacencyRestriction?.Initialize(source, effect, subeffect);
            connectednessRestriction?.Initialize(source, effect, subeffect);
            spaceConnectednessRestriction?.Initialize(source, controller, effect, subeffect);
            limitAdjacencyRestriction?.Initialize(source, effect, subeffect);
            hereFitsRestriction?.Initialize(source, effect, subeffect);
            inAOEOfRestriction?.Initialize(source, effect, subeffect);
            distanceXRestriction?.Initialize(source, subeffect);
            connectedSpacesXRestriction?.Initialize(source, subeffect);
            numberOfCardsInAOEOfRestriction?.Initialize(source, subeffect);

            initialized = true;
        }

        /// <summary>
        /// Whether the given restriction is valid for the given space.
        /// </summary>
        /// <param name="restriction">The restriction to be evaluated</param>
        /// <param name="x">The x coordinate of the space</param>
        /// <param name="y">The y coordinate of the space</param>
        /// <param name="theoreticalTarget">If this space restriction is being considered with a theoretical additional target, this is it</param>
        /// <returns></returns>
        private bool IsRestrictionValid(string restriction, Space space, GameCard theoreticalTarget, ActivationContext context)
        {
            //would use ?? but GameCard inherits from monobehavior which overrides comparison with null
            var target = theoreticalTarget != null ? theoreticalTarget : Subeffect?.CardTarget;

            return restriction switch
            {
                //adjacency
                AdjacentToSource => Source.IsAdjacentTo(space),
                AdjacentToTarget => target?.IsAdjacentTo(space) ?? false,
                AdjacentToCoords => space.AdjacentTo(Subeffect.SpaceTarget),
                AdjacentToCardRestriction => Game.boardCtrl.CardsAdjacentTo(space).Any(c => adjacencyRestriction.IsValidCard(c, context)),

                ConnectedToSourceBy => Game.boardCtrl.AreConnectedBySpaces(Subeffect.Source.Position, space, connectednessRestriction, context),
                ConnectedToSourceBySpaces 
                    => Game.boardCtrl.AreConnectedBySpaces(Subeffect.Source.Position, space, 
                            s => spaceConnectednessRestriction.IsValidSpace(s, context)),
                ConnectedToTargetBy => Game.boardCtrl.AreConnectedBySpaces(target.Position, space, connectednessRestriction, context),
                ConnectedToTargetBySpaces => Game.boardCtrl.AreConnectedBySpaces(target.Position, space, spaceConnectednessRestriction, context),
                ConnectedToTargetByXSpaces 
                    => Game.boardCtrl.AreConnectedByNumberOfSpacesFittingPredicate(target.Position, space, 
                            s => spaceConnectednessRestriction.IsValidSpace(s, context),
                            connectedSpacesXRestriction.IsValidNumber),
                ConnectedToAvatarBy => Game.boardCtrl.AreConnectedBySpaces(Source.Controller.Avatar.Position, space, connectednessRestriction, context),

                InAOE => Source.SpaceInAOE(space),
                NotInAOE => !Source.SpaceInAOE(space),
                InTargetsAOE => target.SpaceInAOE(space),
                InAOEOf => Game.Cards.Any(c => c.SpaceInAOE(space) && inAOEOfRestriction.IsValidCard(c, context)),
                NotInAOEOf => !Game.Cards.Any(c => c.SpaceInAOE(space) && inAOEOfRestriction.IsValidCard(c, context)),
                InAOEOfNumberFittingRestriction 
                    => numberOfCardsInAOEOfRestriction.IsValidNumber(Game.Cards.Count(c => c.SpaceInAOE(space) && inAOEOfRestriction.IsValidCard(c, context))),
                LimitAdjacentCardsFittingRestriction 
                    => Game.boardCtrl.CardsAdjacentTo(space)
                            .Where(c => limitAdjacencyRestriction.IsValidCard(c, context))
                            .Count() <= adjacencyLimit,
                InAOESourceAlsoIn => Game.Cards.Any(c => c.SpaceInAOE(space) && c.CardInAOE(Source)),

                SourceDisplacementToSpaceMatchesCoords => Source.Position.DisplacementTo(space) == Subeffect.SpaceTarget,
                SourceToSpaceSameDirectionAsCoords => Source.Position.DirectionFromThisTo(space) == Subeffect.SpaceTarget,
                SourceToTargetSameDirectionAsCoords => target.Position.DirectionFromThisTo(space) == Subeffect.SpaceTarget,
                SubjectiveDisplacementFromSource 
                    => Controller.SubjectiveCoords(Source.Position).DisplacementTo(Controller.SubjectiveCoords(space)) == (displacementX, displacementY),
                BehindSource => Source.SpaceBehind(space),

                //distance
                DistanceToSourceFitsXRestriction => distanceXRestriction.IsValidNumber(Source.DistanceTo(space)),
                DistanceToTargetFitsXRestriction => distanceXRestriction.IsValidNumber(target.DistanceTo(space)),
                DistanceToCoordsFitsXRestriction => distanceXRestriction.IsValidNumber(Subeffect.SpaceTarget.DistanceTo(space)),

                FurtherFromSourceThanTarget => Source.DistanceTo(space) > Source.DistanceTo(target),
                FurtherFromSourceThanCoords => Source.DistanceTo(space) > Source.DistanceTo(Subeffect.SpaceTarget),

                TowardsSourceFromTarget => Source.DistanceTo(space) < Source.DistanceTo(target),
                TowardsTargetFromSource => target.DistanceTo(space) < target.DistanceTo(Source),

                DirectlyAwayFromTarget => target.SpaceDirectlyAwayFrom(space, Source),

                //misc
                CanPlayTarget => target.PlayRestriction.IsValidEffectPlay(space, Subeffect.Effect, Subeffect.PlayerTarget, context, 
                    ignoring: playRestrictionsToIgnore),
                CanMoveTarget => target.MovementRestriction.IsValidEffectMove(space),
                CanMoveSource => Source.MovementRestriction.IsValidEffectMove(space),

                Empty => Game.boardCtrl.IsEmpty(space),
                Surrounded => Game.boardCtrl.Surrounded(space),
                CardHereFitsRestriction => hereFitsRestriction.IsValidCard(Game.boardCtrl.GetCardAt(space), context),

                OnSourcesDiagonal => Source.SameDiagonal(space),
                OnTargetsDiagonal => target.SameDiagonal(space),
                OnAxisOfLastTwoSpaces => space.SameAxis(Subeffect.SpaceTarget, Subeffect.Effect.GetSpace(-2)),

                OnEdge => space.IsEdge,
                Corner => space.IsCorner,
                SameDirectionFromTargetAsSpace => target.Position.DirectionFromThisTo(space) == Subeffect.SpaceTarget.DirectionFromThisTo(Source.Position),
                OppositeDirectionFromTargetThanSpace => Source.Position.DirectionFromThisTo(space) * -1 == Source.Position.DirectionFromThisTo(Subeffect.SpaceTarget),
                DirectionFromSource => Source.Position.DirectionFromThisTo(space) == Subeffect.SpaceTarget,

                _ => throw new ArgumentException($"Invalid space restriction {restriction}", "restriction"),
            };
        }

        private bool IsRestrictionValidWithDebug(string r, Space space, GameCard theoreticalTarget, ActivationContext context)
        {
            bool success = IsRestrictionValid(r, space, theoreticalTarget, context);
            if (!success) Debug.Log($"Space resetriction {r} was flouted by {space}");
            return success;
        }

        public bool IsValidSpace(Space space, ActivationContext context, GameCard theoreticalTarget = null)
        {
            if (!initialized) throw new ArgumentException("Space restriction not initialized!");
            if (!space.IsValid) throw new InvalidSpaceException(space, "Invalid space to consider for restriction!");
            if (mustBeEmpty && !Game.boardCtrl.IsEmpty(space))
            {
                //Debug.Log($"Space for {Source.CardName} needed to be empty and wasn't.");
                return false;
            }

            return spaceRestrictions.All(r => IsRestrictionValidWithDebug(r, space, theoreticalTarget, context));
        }

        public override string ToString() 
            => $"Space restriction of card {Source} on subeff {Subeffect}, restrictions {string.Join(",", spaceRestrictions)}";
    }
}