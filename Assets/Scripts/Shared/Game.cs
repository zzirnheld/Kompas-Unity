using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.GameCore
{
    public abstract class Game : MonoBehaviour
    {
        public const string CardListPath = "Card Jsons/Card List";

        // The list of locations where cards generally shouldn't be made visible to the opponent
        public static readonly CardLocation[] HiddenLocations =
            new CardLocation[] { CardLocation.Nowhere, CardLocation.Deck, CardLocation.Hand };

        //other scripts
        public abstract UIController UIController { get; }

        //game mechanics
        public abstract BoardController BoardController { get; }

        public abstract Player[] Players { get; }
        public int TurnPlayerIndex { get; protected set; } = 0;
        public Player TurnPlayer => Players[TurnPlayerIndex];
        public int FirstTurnPlayer { get; protected set; }

        //game data
        public abstract CardRepository CardRepository { get; }
        public abstract IEnumerable<GameCard> Cards { get; }
        public int RoundCount { get; protected set; } = 1;
        public virtual int TurnCount { get; protected set; } = 1;
        public virtual int Leyload { get; set; } = 1;

        public abstract GameCard GetCardWithID(int id);

        public virtual IStackable CurrStackEntry => null;
        public abstract IEnumerable<IStackable> StackEntries { get; }
        public abstract bool NothingHappening { get; }

        //game mechanics
        public static bool IsHiddenLocation(CardLocation l) => HiddenLocations.Contains(l);

        public bool BoardHasCopyOf(GameCard card)
            => Cards.Any(c => c != card && c.Location == CardLocation.Board && c.Controller == card.Controller && c.CardName == card.CardName);

        public bool ValidSpellSpaceFor(GameCard card, Space space) => BoardController.ValidSpellSpaceFor(card, space);

        public bool ValidStandardPlaySpace(Space space, Player player)
        {
            bool isFriendlyAndAdjacentToCoords(GameCard toTest, Space adjacentTo)
                => toTest?.Controller == player && toTest.IsAdjacentTo(space);

            bool existsFriendlyAdjacent(Space adjacentTo)
                => BoardController.ExistsCardOnBoard(c => isFriendlyAndAdjacentToCoords(c, adjacentTo));

            //first see if there's an adjacent friendly card to this space
            if (existsFriendlyAdjacent(space)) return true;
            //if there isn't, check if the player is Surrounded
            //A player can play to any space if there isn't a space that is adjacent to a friendly card
            else return !Space.Spaces.Any(existsFriendlyAdjacent);
        }

        public bool ExistsEffectPlaySpace(PlayRestriction restriction, Effect eff)
            => Space.Spaces.Any(s => restriction.IsValidEffectPlay(s, eff, eff.Controller, eff.CurrActivationContext));


        protected void ResetCardsForTurn()
        {
            foreach (var c in Cards) c.ResetForTurn(TurnPlayer);
        }

        public abstract bool IsCurrentTarget(GameCard card);
        public abstract bool IsValidTarget(GameCard card);

        public virtual void Lose(int controllerIndex) { }
    }
}
