using System.Collections.Generic;
using KompasCore.Cards;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects
{
    public class PlayRestriction
    {
        public GameCard Card { get; private set; }

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

        public const string SpaceFitsRestriction = "Space Must Fit Restriction";

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

        public void SetInfo(GameCard card)
        {
            Card = card;

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


            onCardRestriction?.Initialize(Card, effect: default, subeffect: default);
            adjacentCardRestriction?.Initialize(Card, effect: default, subeffect: default);
            spaceRestriction?.Initialize(Card, Card.Controller, effect: default, subeffect: default);
            //Debug.Log($"Finished setting info for play restriction of card {card.CardName}");
        }

        private bool IsValidAugSpace(Space space, Player player)
        {
            var cardThere = Card.Game.boardCtrl.GetCardAt(space);
            return cardThere != null && cardThere.CardType != 'A'
                && (cardThere.Controller == player || cardThere.AdjacentCards.Any(c => c.Controller == player));
        }

        private bool IsOnAugmentSubtypes(Space space)
        {
            var subtypes = Card.Game.boardCtrl.GetCardAt(space)?.SubtypeText;
            Debug.Log($"Subtypes: {subtypes}");
            return Card.AugmentSubtypes?.All(st => subtypes?.Contains(st) ?? false) ?? true;
        }

        private bool IsRestrictionValid(string r, Space space, Player player, ActivationContext context, bool normal) => r != null && r switch
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

            EmptySpace => Card.Game.boardCtrl.IsEmpty(space),
            OnBoardCardFriendlyOrAdjacent => IsValidAugSpace(space, player),

            FastOrNothingIsResolving => Card.Fast || Card.Game.NothingHappening,
            FriendlyTurnIfNotFast => Card.Fast || Card.Game.TurnPlayer == Card.Controller,
            EnemyTurn => Card.Game.TurnPlayer != Card.Controller,

            OnCharacter => Card.Game.boardCtrl.GetCardAt(space)?.CardType == 'C',
            OnCardFittingRestriction => onCardRestriction.IsValidCard(Card.Game.boardCtrl.GetCardAt(space), context),
            OnAugmentSubtypes => IsOnAugmentSubtypes(space),
            OnCardFloutingRestriction => onCardFloutedRestriction.IsValidCard(Card.Game.boardCtrl.GetCardAt(space), context),

            NotNormally => !normal,
            MustNormally => normal,

            CheckUnique => !(Card.Unique && Card.AlreadyCopyOnBoard),
            AdjacentToCardFittingRestriction => Card.Game.boardCtrl.CardsAdjacentTo(space).Any(c => adjacentCardRestriction.IsValidCard(c, context)),
            SpaceFitsRestriction => spaceRestriction.IsValidSpace(space, context),

            _ => throw new System.ArgumentException($"You forgot to check play restriction {r}", "r"),
        };

        private bool IsValidPlay(Space to) => to != null && to.IsValid;
        private bool PlayerCanAffordCost(Player player) => player.Pips >= Card.Cost;

        public bool IsValidNormalPlay(Space to, Player player, string[] ignoring = default)
            => IsValidPlay(to)
                && PlayerCanAffordCost(player)
                && normalRestrictions
                    .Except(ignoring ?? new string[0])
                    .All(r => IsRestrictionValid(r, to, player, default, true));

        public bool IsValidEffectPlay(Space to, Effect effect, Player controller, ActivationContext context, string[] ignoring = default)
            => IsValidPlay(to)
                && effectRestrictions
                    .Except(ignoring ?? new string[0])
                    .All(r => IsRestrictionValid(r, to, controller, context, normal: false));

        public bool IsRecommendedPlay(Space space, Player controller, ActivationContext context, bool normal)
        //=> recommendationRestrictions.All(r => RestrictionValid(r, x, y, controller, normal: normal));
        {
            //Debug.Log($"Checking {space} against recommendations {string.Join(", ", recommendationRestrictions)}");
            return recommendationRestrictions.All(r => IsRestrictionValid(r, space, controller, context: context, normal: normal));
        }

        public bool IsRecommendedNormalPlay(Space space, Player player)
            => IsValidNormalPlay(space, player)
            && IsRecommendedPlay(space, player, context: default, normal: true);
    }
}