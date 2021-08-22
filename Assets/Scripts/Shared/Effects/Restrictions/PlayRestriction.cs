using System.Collections.Generic;
using KompasCore.Cards;
using System.Linq;

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
        public string[] normalRestrictionsToIgnore = new string[0];
        public List<string> effectRestrictions = null;
        public string[] effectRestrictionsToIgnore = new string[0];
        public List<string> recommendationRestrictions = null;

        public CardRestriction onCardRestriction;
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


            onCardRestriction?.Initialize(Card, eff: default);
            adjacentCardRestriction?.Initialize(Card, eff: default);
            spaceRestriction?.Initialize(Card, Card.Controller, effect: default);
            //Debug.Log($"Finished setting info for play restriction of card {card.CardName}");
        }

        private bool RestrictionValid(string r, Space space, Player player, bool normal)
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
                case StandardPlayRestriction: return Card.Game.ValidStandardPlaySpace(space, Card.Controller);
                case StandardSpellRestriction: return Card.Game.ValidSpellSpaceFor(Card, space);
                case HasCostInPips: return Card.Controller.Pips >= Card.Cost;
                case FriendlyTurnIfNotFast: return Card.Fast || Card.Game.TurnPlayer == Card.Controller;
                case FastOrNothingIsResolving: return Card.Fast || Card.Game.NothingHappening;

                case EnemyTurn: return Card.Game.TurnPlayer != Card.Controller;
                case OnBoardCardFriendlyOrAdjacent:
                    var cardThere = Card.Game.boardCtrl.GetCardAt(space);
                    return cardThere != null && cardThere.CardType != 'A'
                        && (cardThere.Controller == Card.Controller || cardThere.AdjacentCards.Any(c => c.Controller == Card.Controller));
                case OnCardFittingRestriction:
                    return onCardRestriction.Evaluate(Card.Game.boardCtrl.GetCardAt(space));
                case NotNormally: return !normal;
                case MustNormally: return normal;
                case CheckUnique: return !(Card.Unique && Card.AlreadyCopyOnBoard);
                case AdjacentToCardFittingRestriction: 
                    return Card.Game.boardCtrl.CardsAdjacentTo(space).Any(adjacentCardRestriction.Evaluate);
                case SpaceFitsRestriction:
                    return spaceRestriction.Evaluate(space);

                default: throw new System.ArgumentException($"You forgot to check play restriction {r}", "r");
            }
        }


        public bool EvaluateNormalPlay(Space to, Player player, bool checkCanAffordCost = false)
        { 
            return (!checkCanAffordCost || player.Pips >= Card.Cost)
                && normalRestrictions.All(r => RestrictionValid(r, to, player, true));
        }

        public bool EvaluateEffectPlay(Space to, Effect effect, Player controller, string[] ignoring = default)
        {
            return effectRestrictions
                .Except(ignoring ?? new string[0])
                .All(r => RestrictionValid(r, to, controller, false));
        }
      
        public bool RecommendedPlay(Space space, Player controller, bool normal)
        //=> recommendationRestrictions.All(r => RestrictionValid(r, x, y, controller, normal: normal));
        {
            //Debug.Log($"Checking {space} against recommendations {string.Join(", ", recommendationRestrictions)}");
            return recommendationRestrictions.All(r => RestrictionValid(r, space, controller, normal: normal));
        }

        public bool RecommendedNormalPlay(Space space, Player player, bool checkCanAffordCost = false)
            => EvaluateNormalPlay(space, player, checkCanAffordCost)
            && RecommendedPlay(space, player, true);
    }
}