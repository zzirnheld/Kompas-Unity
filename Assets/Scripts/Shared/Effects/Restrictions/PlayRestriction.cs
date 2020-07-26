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
        public const string OnFriendlyCard = "On Friendly Card";
        public const string FriendlyTurnIfNotFast = "Friendly Turn";
        public const string HasCostInPips = "Has Cost in Pips";
        public const string NothingIsResolving = "Nothing is Resolving";
        public const string NotNormally = "Cannot be Played Normally";
        public const string MustNormally = "Must be Played Normally";

        public string[] NormalRestrictions = 
            { PlayedByCardOwner, FromHand, StandardPlayRestriction, StandardSpellRestriction, FriendlyTurnIfNotFast, HasCostInPips, NothingIsResolving };
        public string[] EffectRestrictions = { StandardSpellRestriction };

        public void SetInfo(GameCard card)
        {
            Card = card;
        }

        private bool RestrictionValid(string r, int x, int y, Player player, bool normal)
        {
            switch (r)
            {
                case PlayedByCardOwner: return player == Card.Owner;
                case FromHand: return Card.Location == CardLocation.Hand;
                case StandardPlayRestriction: return Card.Game.ValidStandardPlaySpace(x, y, Card.Controller);
                case StandardSpellRestriction: return Card.CardType != 'S' ||
                        Card.Game.boardCtrl.CardsAdjacentTo(x, y).All(c => c.CardType != 'S');
                case OnFriendlyCard: return Card.Game.boardCtrl.GetCardAt(x, y)?.Controller == Card.Controller;
                case HasCostInPips: return Card.Controller.Pips >= Card.Cost;
                case FriendlyTurnIfNotFast: return Card.Fast || Card.Game.TurnPlayer == Card.Controller;
                case NothingIsResolving: return Card.Game.CurrStackEntry == null;
                case NotNormally: return !normal;
                default:
                    Debug.LogError($"You forgot to check for condition {r} in RestrictionInvalid for PlayRestriction");
                    return true;
            }
        }

        public bool EvaluateNormalPlay(int x, int y, Player player)
            => NormalRestrictions.All(r => RestrictionValid(r, x, y, player, true));

        public bool EvaluateEffectPlay(int x, int y, Effect effect)
            => EffectRestrictions.All(r => RestrictionValid(r, x, y, effect.Controller, false));
    }
}