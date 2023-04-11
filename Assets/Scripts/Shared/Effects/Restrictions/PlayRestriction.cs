using System.Collections.Generic;
using KompasCore.Cards;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects
{
    public class PlayRestriction : ContextInitializeableBase
    {
        public const string PlayedByCardOwner = "Played By Card Owner";
        public const string FromHand = "From Hand";
        public const string StandardPlayRestriction = "Adjacent to Friendly Card";
        public const string StandardSpellRestriction = "Not Adjacent to Other Spell";
        public const string FriendlyTurnIfNotFast = "Friendly Turn";
        public const string HasCostInPips = "Has Cost in Pips";
        public const string FastOrNothingIsResolving = "Nothing is Resolving";
        public const string CheckUnique = "Check Unique";
        public const string EmptySpace = "Empty Space";

        public const string EnemyTurn = "Enemy Turn";
        public const string NotNormally = "Cannot be Played Normally";
        public const string MustNormally = "Must be Played Normally";
        public const string OnBoardCardFriendlyOrAdjacent = "On Board Card";
        public const string OnCharacter = "On Character";
        public const string OnAugmentSubtypes = "On Character with Augment Subtypes";
        public const string OnCardFittingRestriction = "On Card that Fits Restriction";
        public const string OnCardFloutingRestriction = "On Card that Flouts Restriction";
        public const string AdjacentToCardFittingRestriction = "Adjacent to Card Fitting Restriction";

        public const string CountExistingCards = "Number of Cards Exist";

        public const string SpaceFitsRestriction = "Space Must Fit Restriction";
        public const string SpaceMustFloutRestriction = "Space Must Flout Restriction";

        public const string DefaultNormal = "Default Normal Restrictions";
        public const string DefaultEffect = "Default Effect Restrictions";
        public static readonly string[] DefaultNormalRestrictions =
            { PlayedByCardOwner, FromHand, StandardPlayRestriction, EmptySpace, StandardSpellRestriction,
            FriendlyTurnIfNotFast, HasCostInPips, FastOrNothingIsResolving, CheckUnique };
        public static readonly string[] DefaultEffectRestrictions = { StandardSpellRestriction, StandardPlayRestriction, EmptySpace, CheckUnique };

        public const string AugNormal = "Augment Normal Restrictions";
        public const string AugEffect = "Augment Effect Restrictions";
        public static readonly string[] AugmentNormalRestrictions =
            { PlayedByCardOwner, FromHand, OnBoardCardFriendlyOrAdjacent, OnCharacter, OnAugmentSubtypes, StandardSpellRestriction,
            FriendlyTurnIfNotFast, HasCostInPips, FastOrNothingIsResolving, CheckUnique };
        public static readonly string[] AugmentEffectRestrictions = { StandardSpellRestriction, OnBoardCardFriendlyOrAdjacent, OnAugmentSubtypes, CheckUnique };

        public List<string> normalRestrictions = null;
        public string[] normalRestrictionsToIgnore = { };
        public List<string> effectRestrictions = null;
        public string[] effectRestrictionsToIgnore = { };
        public List<string> recommendationRestrictions = null;

        public CardRestriction onCardRestriction;
        public CardRestriction onCardFloutedRestriction;
        public CardRestriction adjacentCardRestriction;

        public SpaceRestriction spaceRestriction;
        public SpaceRestriction floutedSpaceRestriction;

        public NumberRestriction countCardNumberRestriction;
        public CardRestriction countCardRestriction;

        public string[] augSubtypes;

        public EffectInitializationContext initializationContext { get; private set; }
        public GameCard Card => InitializationContext.source;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);

            normalRestrictions ??= new List<string> { DefaultNormal };
            effectRestrictions ??= new List<string> { DefaultEffect };
            recommendationRestrictions ??= new List<string>();
            //if (recommendationRestrictions.Count > 0) 
            //  Debug.Log($"More than one recommendation restriction: {string.Join(recommendationRestrictions)}");

            if (normalRestrictions.Contains(DefaultNormal))
                normalRestrictions.AddRange(DefaultNormalRestrictions);
            if (normalRestrictions.Contains(AugNormal))
                normalRestrictions.AddRange(AugmentNormalRestrictions);
            normalRestrictions.RemoveAll(normalRestrictionsToIgnore.Contains);

            if (effectRestrictions.Contains(DefaultEffect))
                effectRestrictions.AddRange(DefaultEffectRestrictions);
            if (effectRestrictions.Contains(AugEffect))
                effectRestrictions.AddRange(AugmentEffectRestrictions);
            effectRestrictions.RemoveAll(effectRestrictionsToIgnore.Contains);


            onCardRestriction?.Initialize(initializationContext);
            adjacentCardRestriction?.Initialize(initializationContext);
            spaceRestriction?.Initialize(initializationContext);
            floutedSpaceRestriction?.Initialize(initializationContext);
            countCardNumberRestriction?.Initialize(initializationContext);
            countCardRestriction?.Initialize(initializationContext);
            //Debug.Log($"Finished setting info for play restriction of card {card.CardName}");
        }

        private bool IsValidAugSpace(Space space, Player player)
        {
            var cardThere = Card.Game.BoardController.GetCardAt(space);
            return cardThere != null && cardThere.CardType != 'A'
                && (cardThere.Controller == player || cardThere.AdjacentCards.Any(c => c.Controller == player));
        }

        private bool IsOnAugmentSubtypes(Space space)
        {
            var subtypes = Card.Game.BoardController.GetCardAt(space)?.SubtypeText;
           // Debug.Log($"Subtypes: {subtypes}");
            return augSubtypes?.All(st => subtypes?.Contains(st) ?? false) ?? true;
        }

        private bool IsRestrictionValid(string r, Space space, Player player, IResolutionContext context, bool normal) => r != null && r switch
        {
            DefaultNormal => true,
            DefaultEffect => true,
            AugNormal => true,
            AugEffect => true,

            PlayedByCardOwner => player == Card.Owner,
            FromHand => Card.Location == CardLocation.Hand,
            StandardPlayRestriction => Card.Game.ValidStandardPlaySpace(space, Card.Controller),
            StandardSpellRestriction => Card.Game.ValidSpellSpaceFor(Card, space),
            HasCostInPips => PlayerCanAffordCost(Card.Controller),

            EmptySpace => Card.Game.BoardController.IsEmpty(space),
            OnBoardCardFriendlyOrAdjacent => IsValidAugSpace(space, player),

            FastOrNothingIsResolving => Card.Game.NothingHappening,
            FriendlyTurnIfNotFast => Card.Game.TurnPlayer == Card.Controller,
            EnemyTurn => Card.Game.TurnPlayer != Card.Controller,

            OnCharacter => Card.Game.BoardController.GetCardAt(space)?.CardType == 'C',
            OnCardFittingRestriction => onCardRestriction.IsValid(Card.Game.BoardController.GetCardAt(space), context),
            OnAugmentSubtypes => IsOnAugmentSubtypes(space),
            OnCardFloutingRestriction => onCardFloutedRestriction.IsValid(Card.Game.BoardController.GetCardAt(space), context),

            NotNormally => !normal,
            MustNormally => normal,

            CheckUnique => !(Card.Unique && InitializationContext.game.BoardHasCopyOf(Card)),
            AdjacentToCardFittingRestriction => Card.Game.BoardController.CardsAdjacentTo(space).Any(c => adjacentCardRestriction.IsValid(c, context)),
            SpaceFitsRestriction => spaceRestriction.IsValidSpace(space, context),
            SpaceMustFloutRestriction => !floutedSpaceRestriction.IsValidSpace(space, context),

            CountExistingCards => countCardNumberRestriction.IsValid(Card.Game.Cards.Count(c => countCardRestriction.IsValid(c, context)), context),

            _ => throw new System.ArgumentException($"You forgot to check play restriction {r}", "r"),
        };

        private bool IsValidPlay(Space to)
        {
            ComplainIfNotInitialized();
            return to != null && to.IsValid;
        }
        private bool PlayerCanAffordCost(Player player) => player.Pips >= Card.Cost;

        public bool IsValidNormalPlay(Space to, Player player, string[] ignoring = default)
            => IsValidPlay(to)
                && PlayerCanAffordCost(player)
                && normalRestrictions
                    .Except(ignoring ?? new string[0])
                    .All(r => IsRestrictionValid(r, to, player, default, true));

        public bool IsValidEffectPlay(Space to, Effect effect, Player controller, IResolutionContext context, string[] ignoring = default)
            => IsValidPlay(to)
                && effectRestrictions
                    .Except(ignoring ?? new string[0])
                    .All(r => IsRestrictionValid(r, to, controller, context, normal: false));

        public bool IsRecommendedPlay(Space space, Player controller, IResolutionContext context, bool normal)
            => IsValidPlay(space) 
                && recommendationRestrictions
                    .All(r => IsRestrictionValid(r, space, controller, context: context, normal: normal));

        public bool IsRecommendedNormalPlay(Space space, Player player)
            => IsValidNormalPlay(space, player)
                && IsRecommendedPlay(space, player, context: default, normal: true);
    }
}