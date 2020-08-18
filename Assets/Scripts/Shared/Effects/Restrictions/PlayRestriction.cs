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
        public const string NothingIsResolving = "Nothing is Resolving";

        public const string NotNormally = "Cannot be Played Normally";
        public const string MustNormally = "Must be Played Normally";
        public const string OnFriendlyCard = "On Friendly Card";

        public const string DefaultNormal = "Default Normal Restrictions";
        public const string DefaultEffect = "Default Effect Restrictions";
        public static readonly string[] DefaultNormalRestrictions =
            { PlayedByCardOwner, FromHand, StandardPlayRestriction, StandardSpellRestriction, FriendlyTurnIfNotFast, HasCostInPips, NothingIsResolving };
        public static readonly string[] DefaultEffectRestrictions = { StandardSpellRestriction, StandardPlayRestriction };

        public const string AugNormal = "Augment Normal Restrictions";
        public const string AugEffect = "Augment Effect Restrictions";
        public static readonly string[] AugmentNormalRestrictions =
            { PlayedByCardOwner, FromHand, OnFriendlyCard, StandardSpellRestriction, FriendlyTurnIfNotFast, HasCostInPips, NothingIsResolving };
        public static readonly string[] AugmentEffectRestrictions = { StandardSpellRestriction, OnFriendlyCard };

        public List<string> normalRestrictions = new List<string> { DefaultNormal };
        public List<string> effectRestrictions = new List<string> { DefaultEffect };

        public void SetInfo(GameCard card)
        {
            Card = card;

            if (normalRestrictions.Contains(DefaultNormal)) normalRestrictions.AddRange(DefaultNormalRestrictions);
            if (normalRestrictions.Contains(AugNormal)) normalRestrictions.AddRange(AugmentNormalRestrictions);

            if (effectRestrictions.Contains(DefaultEffect)) effectRestrictions.AddRange(DefaultEffectRestrictions);
            if (effectRestrictions.Contains(AugEffect)) effectRestrictions.AddRange(AugmentEffectRestrictions);
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
                case StandardSpellRestriction: return Card.CardType != 'S' || Card.Game.ValidSpellSpace(x, y);
                case HasCostInPips: return Card.Controller.Pips >= Card.Cost;
                case FriendlyTurnIfNotFast: return Card.Fast || Card.Game.TurnPlayer == Card.Controller;
                case NothingIsResolving: return Card.Fast || Card.Game.CurrStackEntry == null;

                case OnFriendlyCard: return Card.Game.boardCtrl.GetCardAt(x, y)?.Controller == Card.Controller;
                case NotNormally: return !normal;
                case MustNormally: return normal;

                default:
                    Debug.LogError($"You forgot to check for condition {r} in RestrictionInvalid for PlayRestriction");
                    return true;
            }
        }

        public bool EvaluateNormalPlay(int x, int y, Player player)
            => normalRestrictions.All(r => RestrictionValid(r, x, y, player, true));

        public bool EvaluateEffectPlay(int x, int y, Effect effect)
            => effectRestrictions.All(r => RestrictionValid(r, x, y, effect.Controller, false));
    }
}