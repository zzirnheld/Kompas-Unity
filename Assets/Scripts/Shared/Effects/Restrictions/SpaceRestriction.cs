using KompasCore.Cards;
using KompasCore.Effects.Restrictions;
using KompasCore.Exceptions;
using KompasCore.GameCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects
{
    public class SpaceRestriction : RestrictionBase<Space>
    {
        public Subeffect Subeffect => InitializationContext.subeffect;
        public GameCard Source => InitializationContext.source;
        public Game Game => InitializationContext.game;
        public Player Controller => InitializationContext.Controller;
        public Effect Effect => InitializationContext.effect;

        #region space restrictions
        //adjacency
        public const string AdjacentToSource = "Adjacent to Source";
        public const string AdjacentToCardTarget = "Adjacent to Card Target";
        public const string AdjacentToSpaceTarget = "Adjacent to Space Target";
        public const string AdjacentToCardRestriction = "Adjacent to a Card that Fits Restriction";

        public const string ConnectedToSourceBy = "Connected to Source by Cards Fitting Restriction";
        public const string ConnectedToSourceBySpaces = "Connected to Source by Spaces Fitting Restriction";

        public const string ConnectedToCardTargetBy = "Connected to Card Target by Cards Fitting Restriction";
        public const string ConnectedToCardTargetBySpaces = "Connected to Card Target by Spaces Fitting Restriction";
        public const string ConnectedToCardTargetByXSpaces = "Connected to Card Target by X Spaces Fitting Restriction";

        public const string InSourcesAOE = "In Source's AOE";
        public const string NotInAOE = "Not In AOE";
        public const string InCardTargetsAOE = "In Card Target's AOE";
        public const string InAOEOfCardFittingRestriction = "In AOE of Card Fitting Restriction";
        public const string NotInAOEOf = "Not In AOE of Card Fitting Restriction";
        public const string InAOEOfNumberFittingRestriction = "In AOE of Number of Cards Fitting Restriction";
        public const string InAOESourceAlsoIn = "In AOE Source is Also In";
        public const string InAOECardTargetAlsoIn = "In AOE Card Target is Also In";
        public const string InAOESpaceTargetAlsoIn = "In AOE Space Target is Also In";
        public const string SourceInSpaceOverlapsCardRestriction 
            = "If Source Were In The Targeted Space, It Would Overlap Card Restriction";
        #endregion space restrictions

        public string[] spaceRestrictions = { };
        public CardRestriction adjacencyRestriction;
        public CardRestriction connectednessRestriction;
        public SpaceRestriction spaceConnectednessRestriction;
        public CardRestriction hereFitsRestriction;
        public CardRestriction inAOEOfRestriction;
        public CardRestriction overlapRestriction;
        public CardRestriction alsoInAOEOfRestriction;

        public NumberRestriction distanceXRestriction;
        public NumberRestriction connectedSpacesXRestriction;
        public NumberRestriction numberOfCardsInAOEOfRestriction;

        public string[] playRestrictionsToIgnore = new string[0];

        public int displacementX;
        public int displacementY;

        public string blurb = "";

        //TODO correct any places still using mustBeEmpty

        public List<SpaceRestrictionElement> spaceRestrictionElements = new List<SpaceRestrictionElement>();

        public Func<Space, bool> AsThroughPredicate(IResolutionContext context)
            => s => IsValidSpace(s, context);

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);

            adjacencyRestriction?.Initialize(initializationContext);
            connectednessRestriction?.Initialize(initializationContext);
            spaceConnectednessRestriction?.Initialize(initializationContext);
            hereFitsRestriction?.Initialize(initializationContext);
            inAOEOfRestriction?.Initialize(initializationContext);
            overlapRestriction?.Initialize(initializationContext);
            alsoInAOEOfRestriction?.Initialize(initializationContext);

            distanceXRestriction?.Initialize(initializationContext);
            connectedSpacesXRestriction?.Initialize(initializationContext);
            numberOfCardsInAOEOfRestriction?.Initialize(initializationContext);

            foreach (var sre in spaceRestrictionElements)
            {
                sre.Initialize(initializationContext);
            }
        }

        private bool IsConnectedToTargetByXSpaces(Space space, GameCard target, IResolutionContext context)
        {
            return Game.BoardController.AreConnectedByNumberOfSpacesFittingPredicate(target.Position, space,
                            s => spaceConnectednessRestriction.IsValidSpace(s, context),
                            dist => connectedSpacesXRestriction.IsValid(dist, context));
        }

        private bool InAOEOfNumberOfCardsFittingRestriction(Space space, IResolutionContext context)
        {
            var count = Game.Cards.Count(c => c.SpaceInAOE(space) && inAOEOfRestriction.IsValid(c, context));
            return numberOfCardsInAOEOfRestriction.IsValid(count, context);
        }

        private bool CardInSpaceOverlapsCardRestriction(GameCard card, Space potentialSpace, IResolutionContext context)
        {
            return Game.Cards.Any(c => overlapRestriction.IsValid(c, context) && card.Overlaps(c, potentialSpace));
        }

        /// <summary>
        /// Whether the given restriction is valid for the given space.
        /// </summary>
        /// <param name="restriction">The restriction to be evaluated</param>
        /// <param name="x">The x coordinate of the space</param>
        /// <param name="y">The y coordinate of the space</param>
        /// <param name="theoreticalTarget">If this space restriction is being considered with a theoretical additional target, this is it</param>
        /// <returns></returns>
        private bool IsRestrictionValid(string restriction, Space space, GameCard theoreticalTarget, IResolutionContext context)
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
                AdjacentToCardTarget => target.IsAdjacentTo(space),
                AdjacentToSpaceTarget => space.AdjacentTo(Subeffect.SpaceTarget),
                AdjacentToCardRestriction => Game.BoardController.CardsAdjacentTo(space).Any(c => adjacencyRestriction.IsValid(c, context)),

                ConnectedToSourceBy => Game.BoardController.AreConnectedBySpaces(Subeffect.Source.Position, space, connectednessRestriction, context),
                ConnectedToSourceBySpaces
                    => Game.BoardController.AreConnectedBySpaces(Subeffect.Source.Position, space,
                            s => spaceConnectednessRestriction.IsValidSpace(s, context)),

                ConnectedToCardTargetBy => Game.BoardController.AreConnectedBySpaces(target.Position, space, connectednessRestriction, context),
                ConnectedToCardTargetBySpaces => Game.BoardController.AreConnectedBySpaces(target.Position, space, spaceConnectednessRestriction, context),
                ConnectedToCardTargetByXSpaces => IsConnectedToTargetByXSpaces(space, target, context),

                InSourcesAOE => Source.SpaceInAOE(space),
                NotInAOE => !Source.SpaceInAOE(space),
                InCardTargetsAOE => target.SpaceInAOE(space),
                InAOEOfCardFittingRestriction => Game.Cards.Any(c => c != null && c.SpaceInAOE(space) && inAOEOfRestriction.IsValid(c, context)),
                NotInAOEOf => !Game.Cards.Any(c => c.SpaceInAOE(space) && inAOEOfRestriction.IsValid(c, context)),
                InAOEOfNumberFittingRestriction => InAOEOfNumberOfCardsFittingRestriction(space, context),
                InAOESourceAlsoIn => Game.Cards.Any(c => c.SpaceInAOE(space) && c.CardInAOE(Source) && alsoInAOEOfRestriction.IsValid(c, context)),
                InAOECardTargetAlsoIn => Game.Cards.Any(c => c.SpaceInAOE(space) && c.CardInAOE(target) && alsoInAOEOfRestriction.IsValid(c, context)),
                InAOESpaceTargetAlsoIn => Game.Cards.Any(c => c.SpaceInAOE(space) && c.SpaceInAOE(Subeffect.SpaceTarget) && alsoInAOEOfRestriction.IsValid(c, context)),
                SourceInSpaceOverlapsCardRestriction => CardInSpaceOverlapsCardRestriction(Source, space, context),

                _ => throw new ArgumentException($"Invalid space restriction {restriction}", "restriction"),
            };
        }

        private bool IsRestrictionValidWithDebug(string r, Space space, GameCard theoreticalTarget, IResolutionContext context)
        {
            try
            {
                bool success = IsRestrictionValid(r, space, theoreticalTarget, context);
                //if (!success) Debug.Log($"Space resetriction {r} was flouted by {space}");
                return success;
            }
            catch (NullReferenceException nre)
            {
                Debug.Log($"Space resetriction {r} was flouted by {space} throwing exception {nre}. Init context was {InitializationContext}");
                return false;
            }
        }

        public bool IsValidSpace(Space space, IResolutionContext context, GameCard theoreticalTarget = null)
        {
            ComplainIfNotInitialized();
            if (!space.IsValid) throw new InvalidSpaceException(space, "Invalid space to consider for restriction!");

            return spaceRestrictions.All(r => IsRestrictionValidWithDebug(r, space, theoreticalTarget, context))
                && spaceRestrictionElements.All(sre => sre.IsValid(space, context));
        }

        public Func<Space, bool> IsValidFor(IResolutionContext context) => s => IsValidSpace(s, context);

        public override string ToString()
            => $"Space restriction of card {Source} on subeff {Subeffect}, restrictions {string.Join(",", spaceRestrictions)}";
    }
}