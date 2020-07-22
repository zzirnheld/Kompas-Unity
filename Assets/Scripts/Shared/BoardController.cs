using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KompasCore.Effects;
using KompasCore.Cards;

namespace KompasCore.GameCore
{
    public class BoardController : MonoBehaviour
    {
        public Game game;

        public const int SpacesInGrid = 7;
        public const float BoardLenOffset = 7f;
        public const float LenOneSpace = 2f;
        public const float SpaceOffset = LenOneSpace / 2;

        public static int PosToGridIndex(float pos)
        {
            return (int)((pos + BoardLenOffset) / (LenOneSpace));
        }

        public static float GridIndexToPos(int gridIndex)
        {
            return (float)((gridIndex * LenOneSpace) + SpaceOffset - BoardLenOffset);
        }

        public static Vector3 GridIndicesFromPos(int x, int y)
        {
            return new Vector3(GridIndexToPos(x), 0.2f, GridIndexToPos(y));
        }

        private readonly GameCard[,] cards = new GameCard[SpacesInGrid, SpacesInGrid];

        //helper methods
        #region helper methods
        public bool ValidIndices(int x, int y)
        {
            return x >= 0 && y >= 0 && x < 7 && y < 7;
        }

        //get game data
        public GameCard GetCardAt(int x, int y)
        {
            if (!ValidIndices(x, y)) return null;
            return cards[x, y];
        }

        public List<GameCard> CardsAdjacentTo(int x, int y)
        {
            var list = new List<GameCard>();

            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    var card = GetCardAt(i, j);
                    if ((i, j) != (x, y) && card != null) list.Add(card);
                }
            }

            return list;
        }

        public List<GameCard> CardsWhere(Func<GameCard, bool> predicate)
        {
            var list = new List<GameCard>();
            foreach (var card in cards) if (predicate(card)) list.Add(card);
            return list;
        }

        /// <summary>
        /// A really bad Dijkstra's because this is a fun side project and I'm not feeling smart today
        /// </summary>
        /// <param name="src">The card to start looking from</param>
        /// <param name="x">The x coordinate you want a distance to</param>
        /// <param name="y">The y coordinate you want a distance to</param>
        /// <param name="through">What all cards you go through must fit</param>
        /// <returns></returns>
        public int ShortestPath(GameCard src, int x, int y, CardRestriction through)
        {
            //record shortest distances to cards
            var dist = new Dictionary<GameCard, int>();
            //and if you've seen them
            var seen = new HashSet<GameCard>();
            //the queue of nodes to process next. things should only go on here once, the first time they're seen
            var queue = new Queue<GameCard>();

            //set up the structures with the source node
            queue.Enqueue(src);
            dist.Add(src, 0);
            seen.Add(src);

            //iterate until the queue is empty, in which case you'll have seen all connected cards that fit the restriction.
            while (queue.Any())
            {
                //consider the next node's adjacent cards
                var next = queue.Dequeue();
                foreach (var card in next.AdjacentCards.Where(c => through.Evaluate(c)))
                {
                    //if that adjacent card is never seen before, initialize its distance and add it to the structures
                    if (!seen.Contains(card))
                    {
                        seen.Add(card);
                        queue.Enqueue(card);
                        dist[card] = dist[next] + 1;
                    }
                    //otherwise, relax its distance if appropriate
                    else if (dist[next] + 1 < dist[card]) dist[card] = dist[next] + 1;
                }
            }

            //then, go through the list of cards adjacent to our target location
            //choose the card that's closest to our source
            int min = 50;
            foreach (var card in CardsAdjacentTo(x, y))
            {
                if (dist.ContainsKey(card) && dist[card] < min) min = dist[card];
            }
            return min;
        }

        public bool ExistsCardOnBoard(Func<GameCard, bool> predicate)
        {
            foreach (var c in cards)
            {
                if (predicate(c)) return true;
            }
            return false;
        }

        public int GetNumCardsOnBoard()
        {
            int i = 0;
            foreach (GameCard card in cards)
            {
                if (card != null) i++;
            }
            return i;
        }
        #endregion

        #region game mechanics
        public void RemoveFromBoard(GameCard toRemove)
        {
            if (toRemove?.Location != CardLocation.Field) return;

            RemoveFromBoard(toRemove.BoardX, toRemove.BoardY);
        }

        public void RemoveFromBoard(int x, int y) => cards[x, y] = null;

        /// <summary>
        /// Puts the card on the board
        /// </summary>
        /// <param name="toPlay">Card to be played</param>
        /// <param name="toX">X coordinate to play the card to</param>
        /// <param name="toY">Y coordinate to play the card to</param>
        public virtual bool Play(GameCard toPlay, int toX, int toY, Player controller, IStackable stackSrc = null)
        {
            Debug.Log($"In boardctrl, playing {toPlay.CardName} to {toX}, {toY}");
            toPlay.Remove(stackSrc);

            if (toPlay.CardType == 'A') cards[toX, toY].AddAugment(toPlay, stackSrc);
            else cards[toX, toY] = toPlay;

            toPlay.Location = CardLocation.Field;
            toPlay.Position = (toX, toY);
            toPlay.Controller = controller;

            return true;
        }

        //movement
        public virtual bool Swap(GameCard card, int toX, int toY, bool playerInitiated, IStackable stackSrc = null)
        {
            Debug.Log($"Swapping {card?.CardName} to {toX}, {toY}");

            if (!ValidIndices(toX, toY) || card == null) return false;
            if (card.AugmentedCard != null) throw new NotImplementedException();

            var (tempX, tempY) = card.Position;
            GameCard temp = cards[toX, toY];
            cards[toX, toY] = card;
            cards[tempX, tempY] = temp;

            //then let the cards know they've been moved
            if (playerInitiated)
            {
                card.CountSpacesMovedTo((toX, toY));
                temp?.CountSpacesMovedTo((tempX, tempY));
            }
            card.Position = (toX, toY);
            if (temp != null) temp.Position = (tempX, tempY);
            return true;
        }

        public bool Move(GameCard card, int toX, int toY, bool playerInitiated, IStackable stackSrc = null)
        {
            if (!ValidIndices(toX, toY)) return false;

            if (card.AugmentedCard != null)
            {
                if (card.Remove(stackSrc))
                {
                    cards[toX, toY].AddAugment(card, stackSrc);
                    return true;
                }
                return false;
            }
            else return Swap(card, toX, toY, playerInitiated, stackSrc);
        }

        public void PutCardsBack()
        {
            foreach (GameCard card in cards)
            {
                if (card != null) card.PutBack();
            }
        }

        public bool ExistsCardOnBoard(CardRestriction restriction)
        {
            foreach (GameCard c in cards)
            {
                if (c != null && restriction.Evaluate(c)) return true;
            }

            return false;
        }

        public bool CanPlayTo(int playerIndex, int x, int y)
        {
            foreach (GameCard c in cards)
            {
                if (c != null && c.IsAdjacentTo(x, y) && c.ControllerIndex == playerIndex) return true;
            }

            return false;
        }

        public void DiscardSimples()
        {
            foreach (GameCard c in cards)
            {
                if (c != null && c.SpellSubtype == CardBase.SimpleSubtype) c.Discard();
            }
        }
        #endregion game mechanics

        public void OnMouseDown()
        {
            //select nothing
            game.uiCtrl.SelectCard(null, true);

            if (game.targetMode != Game.TargetMode.SpaceTarget) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var intersection = transform.InverseTransformPoint(hit.point);

                int xIntersection = PosToGridIndex(intersection.x);
                int yIntersection = PosToGridIndex(intersection.z);
                //then, if the game is a clientgame, request a space target
                game.OnClickBoard(xIntersection, yIntersection);
            }
        }
    }
}