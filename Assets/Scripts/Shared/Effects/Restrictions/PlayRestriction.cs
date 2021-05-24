using System.Collections.Generic;
using KompasCore.Cards;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects
{
    [System.Serializable]
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

        public const string EnemyTurn = "Enemy Turn";
        public const string NotNormally = "Cannot be Played Normally";
        public const string MustNormally = "Must be Played Normally";
        public const string OnBoardCardFriendlyOrAdjacent = "On Board Card";
        public const string OnCardFittingRestriction = "On Card that Fits Restriction";
        public const string AdjacentToCardFittingRestriction = "Adjacent to Card Fitting Restriction";

        public const string SpaceFitsRestriction = "Space Must Fit Restriction";

        public const string DefaultNormal = "Default Normal Restrictions";
        public const string DefaultEffect = "Default Effect Restrictions";
        public static readonly string[] DefaultNormalRestrictions =
            { PlayedByCardOwner, FromHand, StandardPlayRestriction, StandardSpellRestriction, 
            FriendlyTurnIfNotFast, HasCostInPips, FastOrNothingIsResolving, CheckUnique };
        public static readonly string[] DefaultEffectRestrictions = { StandardSpellRestriction, StandardPlayRestriction, CheckUnique };

        public const string AugNormal = "Augment Normal Restrictions";
        public const string AugEffect = "Augment Effect Restrictions";
        public static readonly string[] AugmentNormalRestrictions =
            { PlayedByCardOwner, FromHand, OnBoardCardFriendlyOrAdjacent, StandardSpellRestriction, 
            FriendlyTurnIfNotFast, HasCostInPips, FastOrNothingIsResolving, CheckUnique };
        public static readonly string[] AugmentEffectRestrictions = { StandardSpellRestriction, OnBoardCardFriendlyOrAdjacent, CheckUnique };

        public List<string> normalRestrictions = null;
        public List<string> effectRestrictions = null;
        public List<string> recommendationRestrictions = null;

        public CardRestriction onCardRestriction = new CardRestriction();
        public CardRestriction adjacentCardRestriction = new CardRestriction();

        public SpaceRestriction spaceRestriction = new SpaceRestriction();

        public void SetInfo(GameCard card)
        {
            Card = card;

            normalRestrictions = normalRestrictions ?? new List<string> { DefaultNormal };
            effectRestrictions = effectRestrictions ?? new List<string> { DefaultEffect };
            recommendationRestrictions = recommendationRestrictions ?? new List<string>();
            //if (recommendationRestrictions.Count > 0) Debug.Log($"More than one recommendation restriction: {string.Join(recommendationRestrictions)}");

            if (normalRestrictions.Contains(DefaultNormal)) normalRestrictions.AddRange(DefaultNormalRestrictions);
            if (normalRestrictions.Contains(AugNormal)) normalRestrictions.AddRange(AugmentNormalRestrictions);

            if (effectRestrictions.Contains(DefaultEffect)) effectRestrictions.AddRange(DefaultEffectRestrictions);
            if (effectRestrictions.Contains(AugEffect)) effectRestrictions.AddRange(AugmentEffectRestrictions);

            onCardRestriction.Initialize(card, card.Controller, eff: default);
            adjacentCardRestriction.Initialize(card, card.Controller, eff: default);

            spaceRestriction.Initialize(card, card.Controller, effect: default);
            //Debug.Log($"Finished setting info for play restriction of card {card.CardName}");
        }

        private bool RestrictionValid(string r, int x, int y, Player player, bool normal)
        {
            switch (r)
            {
                case DefaultNormal:
                case DefaultEffect:
                case AugNormal:
                case AugEffect:
                    return true; //they're covered by the restrictions added on their behalf

                case PlayedByCardOwner: return player == Card.Owner;
                case FromHand: return Card.Location == CardLocation.Hand;
                case StandardPlayRestriction: return Card.Game.ValidStandardPlaySpace(x, y, Card.Controller);
                case StandardSpellRestriction: return Card.Game.ValidSpellSpaceFor(Card, x, y);
                case HasCostInPips: return Card.Controller.Pips >= Card.Cost;
                case FriendlyTurnIfNotFast: return Card.Fast || Card.Game.TurnPlayer == Card.Controller;
                case FastOrNothingIsResolving: return Card.Fast || Card.Game.NothingHappening;

                case EnemyTurn: return Card.Game.TurnPlayer != Card.Controller;
                case OnBoardCardFriendlyOrAdjacent:
                    var cardThere = Card.Game.boardCtrl.GetCardAt(x, y);
                    return cardThere != null 
                        && (cardThere.Controller == Card.Controller || cardThere.AdjacentCards.Any(c => c.Controller == Card.Controller));
                case OnCardFittingRestriction:
                    onCardRestriction.Initialize(Card, player, null);
                    return onCardRestriction.Evaluate(Card.Game.boardCtrl.GetCardAt(x, y));
                case NotNormally: return !normal;
                case MustNormally: return normal;
                case CheckUnique: return !(Card.Unique && Card.AlreadyCopyOnBoard);
                case AdjacentToCardFittingRestriction: 
                    return Card.Game.boardCtrl.CardsAdjacentTo(x, y).Any(adjacentCardRestriction.Evaluate);

                default: throw new System.ArgumentException($"You forgot to check play restriction {r}", "r");
            }
        }


        public bool EvaluateNormalPlay(int x, int y, Player player, bool checkCanAffordCost = false)
            => (!checkCanAffordCost || player.Pips >= Card.Cost) 
            && normalRestrictions.All(r => RestrictionValid(r, x, y, player, true));

        public bool EvaluateEffectPlay(int x, int y, Effect effect, Player controller)
            => effectRestrictions.All(r => RestrictionValid(r, x, y, controller, false));

        public bool EvaluateEffectPlay(int x, int y, Effect effect)
            => EvaluateEffectPlay(x, y, effect, effect.Controller);

        public bool RecommendedPlay(int x, int y, Player controller, bool normal)
        //=> recommendationRestrictions.All(r => RestrictionValid(r, x, y, controller, normal: normal));
        {
            Debug.Log($"Checking {x}, {y} against recommendations {string.Join(", ", recommendationRestrictions)}");
            return recommendationRestrictions.All(r => RestrictionValid(r, x, y, controller, normal: normal));
        }

        public bool RecommendedNormalPlay(int x, int y, Player player, bool checkCanAffordCost = false)
            => EvaluateNormalPlay(x, y, player, checkCanAffordCost)
            && RecommendedPlay(x, y, player, true);
    }
}