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
        public const string AdjacentToCardTarget = "Adjacent to Card Target";
        public const string AdjacentToSpaceTarget = "Adjacent to Space Target";
        public const string AdjacentToCardRestriction = "Adjacent to a Card that Fits Restriction";

        public const string ConnectedToSourceBy = "Connected to Source by Cards Fitting Restriction";
        public const string ConnectedToSourceBySpaces = "Connected to Source by Spaces Fitting Restriction";

        public const string ConnectedToCardTargetBy = "Connected to Card Target by";
        public const string ConnectedToCardTargetBySpaces = "Connected to Card Target by Spaces Fitting Restriction";
        public const string ConnectedToCardTargetByXSpaces = "Connected to Card Target by X Spaces Fitting Restriction";

        public const string InSourcesAOE = "In Source's AOE";
        public const string NotInAOE = "Not In AOE";
        public const string InCardTargetsAOE = "In Card Target's AOE";
        public const string InAOEOfCardFittingRestriction = "In AOE of Card Fitting Restriction";
        public const string NotInAOEOf = "Not In AOE of Card Fitting Restriction";
        public const string LimitAdjacentCardsFittingRestriction = "Limit Number of Adjacent Cards Fitting Restriction";
        public const string InAOEOfNumberFittingRestriction = "In AOE of Number of Cards Fitting Restriction";
        public const string InAOESourceAlsoIn = "In AOE Source is Also In";

        public const string SourceDisplacementToSpaceMatchesSpaceTarget = "Source Displacement to Space Matches Space Target";
        public const string SourceDisplacementToSpaceSameDirectionAsSpaceTarget 
            = "Source Displacement to Space Same Direction as Space Target";
        public const string SourceDisplacementToCardTargetSameDirectionAsSpaceTarget 
            = "Source Displacement to Card Target Same Direction as Space Target";
        public const string ConstantSubjectiveDisplacementFromSource = "Constant Subjective Displacement from Source";
        public const string BehindSource = "Behind Source";

        //distance
        public const string DistanceToSourceFitsXRestriction = "Distance to Source Fits X Restriction";
        public const string DistanceToCardTargetFitsXRestriction = "Distance to Card Target Fits X Restriction";
        public const string DistanceToSpaceTargetFitsXRestriction = "Distance to Space Target Fits X Restriction";

        public const string FurtherFromSourceThanCardTarget = "Further from Source than Card Target";
        public const string FurtherFromSourceThanSpaceTarget = "Further from Source than Space Target";
        public const string TowardsSourceFromCardTarget = "Towards Source from Card Target";
        public const string TowardsCardTargetFromSource = "Towards Card Target from Source";
        public const string DirectlyAwayFromCardTarget = "Directly Away from Card Target";

        //misc
        public const string CanPlayCardTarget = "Can Play Card Target to This Space";
        public const string CanMoveCardTarget = "Can Move Card Target to This Space";
        public const string CanMoveSource = "Can Move Source to This Space";
        public const string Empty = "Empty";
        public const string Surrounded = "Surrounded";
        public const string CardHereFitsRestriction = "Card Here Fits Restriction";

        public const string OnSourcesDiagonal = "On Source's Diagonal";
        public const string OnCardTargetsDiagonal = "On Card Target's Diagonal";
        public const string OnAxisOfLastTwoSpaces = "On Axis of Last Two Spaces";

        public const string OnEdge = "On Edge of Board";
        public const string Corner = "Corner";
        //TODO: eventually make a "targetdirection" subeffect that appends the direction as a Space to the list of coords,
        // then replace these with something comparing directions
        public const string SameDirectionFromSpaceToCardTargetAsSpaceTargetToSource 
            = "Same Direction from Space to Card Target as Space Target to Source";
        public const string OppositeDirectionFromSourceToSpaceAsSourceToSpaceTarget 
            = "Opposite Direction from Source to Space as Source to Space Target";
        public const string DirectionFromSourceIsSpaceTarget = "Direction from Source is Space Target"; //In the direction of Subeffect.Space from source
        #endregion space restrictions

        public string[] spaceRestrictions;
        public CardRestriction adjacencyRestriction;
        public CardRestriction limitAdjacencyRestriction;
        public int adjacencyLimit;
        public CardRestriction connectednessRestriction;
        public SpaceRestriction spaceConnectednessRestriction;
        public CardRestriction hereFitsRestriction;
        public CardRestriction inAOEOfRestriction;

        public NumberRestriction distanceXRestriction;
        public NumberRestriction connectedSpacesXRestriction;
        public NumberRestriction numberOfCardsInAOEOfRestriction;

        public string[] playRestrictionsToIgnore = new string[0];

        public int constant;

        public int displacementX;
        public int displacementY;

        public string blurb = "";
        //Using rather than an "Empty" restriction for, at this point, historical reasons - TODO fix
        public bool mustBeEmpty = true;

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
            var target = theoreticalTarget ?? Subeffect?.CardTarget;

            if (space == null)
            {
                Debug.LogError("Tried to check a null space in SpaceRestriction!");
                return false;
            }

            return restriction != null && restriction switch
            {
                //adjacency
                AdjacentToSource => Source.IsAdjacentTo(space),
                AdjacentToCardTarget => target?.IsAdjacentTo(space) ?? false,
                AdjacentToSpaceTarget => space.AdjacentTo(Subeffect.SpaceTarget),
                AdjacentToCardRestriction => Game.boardCtrl.CardsAdjacentTo(space).Any(c => adjacencyRestriction.IsValidCard(c, context)),

                ConnectedToSourceBy => Game.boardCtrl.AreConnectedBySpaces(Subeffect.Source.Position, space, connectednessRestriction, context),
                ConnectedToSourceBySpaces 
                    => Game.boardCtrl.AreConnectedBySpaces(Subeffect.Source.Position, space, 
                            s => spaceConnectednessRestriction.IsValidSpace(s, context)),

                ConnectedToCardTargetBy => Game.boardCtrl.AreConnectedBySpaces(target.Position, space, connectednessRestriction, context),
                ConnectedToCardTargetBySpaces => Game.boardCtrl.AreConnectedBySpaces(target.Position, space, spaceConnectednessRestriction, context),
                ConnectedToCardTargetByXSpaces 
                    => Game.boardCtrl.AreConnectedByNumberOfSpacesFittingPredicate(target.Position, space, 
                            s => spaceConnectednessRestriction.IsValidSpace(s, context),
                            connectedSpacesXRestriction.IsValidNumber),

                InSourcesAOE => Source.SpaceInAOE(space),
                NotInAOE => !Source.SpaceInAOE(space),
                InCardTargetsAOE => target.SpaceInAOE(space),
                InAOEOfCardFittingRestriction => Game.Cards.Any(c => c.SpaceInAOE(space) && inAOEOfRestriction.IsValidCard(c, context)),
                NotInAOEOf => !Game.Cards.Any(c => c.SpaceInAOE(space) && inAOEOfRestriction.IsValidCard(c, context)),
                InAOEOfNumberFittingRestriction 
                    => numberOfCardsInAOEOfRestriction.IsValidNumber(Game.Cards.Count(c => c.SpaceInAOE(space) && inAOEOfRestriction.IsValidCard(c, context))),
                LimitAdjacentCardsFittingRestriction 
                    => Game.boardCtrl.CardsAdjacentTo(space)
                            .Where(c => limitAdjacencyRestriction.IsValidCard(c, context))
                            .Count() <= adjacencyLimit,
                InAOESourceAlsoIn => Game.Cards.Any(c => c.SpaceInAOE(space) && c.CardInAOE(Source)),

                SourceDisplacementToSpaceMatchesSpaceTarget => Source.Position.DisplacementTo(space) == Subeffect.SpaceTarget,
                SourceDisplacementToSpaceSameDirectionAsSpaceTarget => Source.Position.DirectionFromThisTo(space) == Subeffect.SpaceTarget,
                SourceDisplacementToCardTargetSameDirectionAsSpaceTarget => target.Position.DirectionFromThisTo(space) == Subeffect.SpaceTarget,
                ConstantSubjectiveDisplacementFromSource 
                    => Controller.SubjectiveCoords(Source.Position).DisplacementTo(Controller.SubjectiveCoords(space)) == (displacementX, displacementY),
                BehindSource => Source.SpaceBehind(space),

                //distance
                DistanceToSourceFitsXRestriction => distanceXRestriction.IsValidNumber(Source.DistanceTo(space)),
                DistanceToCardTargetFitsXRestriction => distanceXRestriction.IsValidNumber(target.DistanceTo(space)),
                DistanceToSpaceTargetFitsXRestriction => distanceXRestriction.IsValidNumber(Subeffect.SpaceTarget.DistanceTo(space)),

                FurtherFromSourceThanCardTarget => Source.DistanceTo(space) > Source.DistanceTo(target),
                FurtherFromSourceThanSpaceTarget => Source.DistanceTo(space) > Source.DistanceTo(Subeffect.SpaceTarget),

                TowardsSourceFromCardTarget => Source.DistanceTo(space) < Source.DistanceTo(target),
                TowardsCardTargetFromSource => target.DistanceTo(space) < target.DistanceTo(Source),

                DirectlyAwayFromCardTarget => target.SpaceDirectlyAwayFrom(space, Source),

                //misc
                CanPlayCardTarget => target.PlayRestriction.IsValidEffectPlay(space, Subeffect.Effect, Subeffect.PlayerTarget, context, 
                    ignoring: playRestrictionsToIgnore),
                CanMoveCardTarget => target.MovementRestriction.IsValidEffectMove(space),
                CanMoveSource => Source.MovementRestriction.IsValidEffectMove(space),

                Empty => Game.boardCtrl.IsEmpty(space),
                Surrounded => Game.boardCtrl.Surrounded(space),
                CardHereFitsRestriction => hereFitsRestriction.IsValidCard(Game.boardCtrl.GetCardAt(space), context),

                OnSourcesDiagonal => Source.SameDiagonal(space),
                OnCardTargetsDiagonal => target.SameDiagonal(space),
                OnAxisOfLastTwoSpaces => space.SameAxis(Subeffect.SpaceTarget, Subeffect.Effect.GetSpace(-2)),

                OnEdge => space.IsEdge,
                Corner => space.IsCorner,
                SameDirectionFromSpaceToCardTargetAsSpaceTargetToSource 
                    => target.Position.DirectionFromThisTo(space) == Subeffect.SpaceTarget.DirectionFromThisTo(Source.Position),
                OppositeDirectionFromSourceToSpaceAsSourceToSpaceTarget 
                    => Source.Position.DirectionFromThisTo(space) * -1 == Source.Position.DirectionFromThisTo(Subeffect.SpaceTarget),
                DirectionFromSourceIsSpaceTarget => Source.Position.DirectionFromThisTo(space) == Subeffect.SpaceTarget,

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