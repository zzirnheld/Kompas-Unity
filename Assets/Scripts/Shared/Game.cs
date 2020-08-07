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
        public static readonly CardLocation[] HiddenLocations 
            = new CardLocation[] { CardLocation.Nowhere, CardLocation.Deck, CardLocation.Hand };

        public static Game mainGame;

        public enum TargetMode { Free, OnHold, BoardTarget, HandTarget, SpaceTarget }

        //other scripts
        public UIController uiCtrl;

        //game mechanics
        public BoardController boardCtrl;
        public AnnihilationController annihilationCtrl;
        //game objects
        public GameObject boardObject;

        //list of card names 
        public CardRepository cardRepo;

        public abstract Player[] Players { get; }
        public int TurnPlayerIndex { get; protected set; } = 0;
        public Player TurnPlayer { get { return Players[TurnPlayerIndex]; } }

        //game data
        public abstract IEnumerable<GameCard> Cards { get; }
        public int FirstTurnPlayer { get; protected set; }
        public int RoundCount { get; protected set; } = 1;
        public virtual int TurnCount { get; protected set; } = 1;
        public virtual int Leyload { get; set; } = 1;

        public TargetMode targetMode = TargetMode.Free;

        public virtual void OnClickBoard(int x, int y) { }
        public virtual void Lose(int controllerIndex) { }

        public abstract GameCard GetCardWithID(int id);

        public virtual IStackable CurrStackEntry => null;

        //game mechanics
        //checking for valid target

        public bool ExistsCardTarget(CardRestriction restriction) => Cards.Any(c => restriction.Evaluate(c));

        public bool ExistsSpaceTarget(SpaceRestriction restriction)
        {
            for (int x = 0; x < 7; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    if (restriction.Evaluate(x, y)) return true;
                }
            }

            return false;
        }

        private bool IsFriendlyAdjacentToCoords(int x, int y, GameCard potentialFriendly, Player friendly)
        {
            return boardCtrl.GetCardAt(x, y) == null
                && potentialFriendly != null && potentialFriendly.IsAdjacentTo(x, y)
                && potentialFriendly.Controller == friendly;
        }

        public bool ValidStandardPlaySpace(int x, int y, Player friendly)
        {
            //first see if there's an adjacent friendly card to this space
            if (boardCtrl.ExistsCardOnBoard(c => IsFriendlyAdjacentToCoords(x, y, c, friendly))) return true;
            //if there isn't, check if the player is Surrounded
            else
            {
                //iterate through all possible spaces
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        //if there *is* a possible space to play it to, they're not surrounded
                        if (boardCtrl.ExistsCardOnBoard(c => IsFriendlyAdjacentToCoords(i, j, c, friendly))) return false;
                    }
                }
                //if we didn't find a single place to play a card normally, any space is fair game, by the Surrounded rule
                return true;
            }
        }

        public bool ExistsEffectPlaySpace(PlayRestriction restriction, Effect eff)
        {
            for(int x = 0; x < 7; x++)
            {
                for(int y = 0; y < 7; y++)
                {
                    if (restriction.EvaluateEffectPlay(x, y, eff)) return true;
                }
            }

            return false;
        }

        public bool ValidSpellSpace(int x, int y)
        {
            return boardCtrl.CardsAdjacentTo(x, y).Where(c => c.CardType == 'S').Count() <= 2;
        }

        protected void ResetCardsForTurn()
        {
            foreach (var c in Cards) c?.ResetForTurn(TurnPlayer);
        }
    }
}
