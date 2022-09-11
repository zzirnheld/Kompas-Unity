using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.GameCore
{
    public abstract partial class Game : MonoBehaviour
    {
        public const string CardListPath = "Card Jsons/Card List";

        // The list of locations where cards generally shouldn't be made visible to the opponent
        public static readonly CardLocation[] HiddenLocations =
            new CardLocation[] { CardLocation.Nowhere, CardLocation.Deck, CardLocation.Hand };

        //other scripts
        public abstract UIController UIController { get; }

        //game mechanics
        public abstract BoardController BoardController { get; }

        //list of card names 
        public CardRepository cardRepo;

        public abstract Player[] Players { get; }
        public int TurnPlayerIndex { get; protected set; } = 0;
        public Player TurnPlayer => Players[TurnPlayerIndex];

        //game data
        public abstract IEnumerable<GameCard> Cards { get; }
        public int FirstTurnPlayer { get; protected set; }
        public int RoundCount { get; protected set; } = 1;
        public virtual int TurnCount { get; protected set; } = 1;
        public virtual int Leyload { get; set; } = 1;

        //public TargetMode targetMode = TargetMode.Free;

        public virtual void OnClickBoard(int x, int y) { }
        public virtual void Lose(int controllerIndex) { }

        public abstract GameCard GetCardWithID(int id);

        public virtual IStackable CurrStackEntry => null;
        public abstract IEnumerable<IStackable> StackEntries { get; }
        public abstract bool NothingHappening { get; }

        //game mechanics
        public static bool IsHiddenLocation(CardLocation l) => HiddenLocations.Contains(l);

        public bool BoardHasCopyOf(GameCard card)
            => Cards.Any(c => c != card && c.Location == CardLocation.Board && c.Controller == card.Controller && c.CardName == card.CardName);
        /*{
            Debug.Log($"Checking if board has copy of {card.CardName} with controller index {card.ControllerIndex}");
            return Cards.Any(c => c != card && c.Location == CardLocation.Field && c.Controller == card.Controller && c.CardName == card.CardName);
        }*/

        public bool ValidSpellSpaceFor(GameCard card, Space space) => BoardController.ValidSpellSpaceFor(card, space);

        private bool IsFriendlyAdjacentToCoords(Space space, GameCard potentialFriendly, Player friendly)
        {
            return BoardController.IsEmpty(space)
                && potentialFriendly != null && potentialFriendly.IsAdjacentTo(space)
                && potentialFriendly.Controller == friendly;
        }

        public bool ValidStandardPlaySpace(Space space, Player friendly)
        {
            //first see if there's an adjacent friendly card to this space
            if (BoardController.ExistsCardOnBoard(c => IsFriendlyAdjacentToCoords(space, c, friendly))) return true;
            //if there isn't, check if the player is Surrounded
            else
            {
                //iterate through all possible spaces
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        //if there *is* a possible space to play it to, they're not surrounded
                        if (BoardController.ExistsCardOnBoard(c => IsFriendlyAdjacentToCoords((i, j), c, friendly))) return false;
                    }
                }
                //if we didn't find a single place to play a card normally, any space is fair game, by the Surrounded rule
                return true;
            }
        }

        public bool ExistsEffectPlaySpace(PlayRestriction restriction, Effect eff)
            => Space.Spaces.Any(s => restriction.IsValidEffectPlay(s, eff, eff.Controller, eff.CurrActivationContext));


        protected void ResetCardsForTurn()
        {
            foreach (var c in Cards) c.ResetForTurn(TurnPlayer);
        }

        public abstract bool IsCurrentTarget(GameCard card);
        public abstract bool IsValidTarget(GameCard card);
    }
}
