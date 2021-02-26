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
        public const int SpacesInGrid = 7;
        public const float BoardLenOffset = 7f;
        public const float LenOneSpace = 2f;
        public const float SpaceOffset = LenOneSpace / 2f;

        public Game game;

        public static int PosToGridIndex(float pos) 
            => (int)((pos + BoardLenOffset) / (LenOneSpace));

        public static float GridIndexToPos(int gridIndex)
            => (float)((gridIndex * LenOneSpace) + SpaceOffset - BoardLenOffset);

        public static Vector3 GridIndicesToCardPos(int x, int y)
            => new Vector3(GridIndexToPos(x), 0.1f, GridIndexToPos(y));

        public readonly GameCard[,] Board = new GameCard[SpacesInGrid, SpacesInGrid];

        //helper methods
        #region helper methods
        public bool ValidIndices(int x, int y) => x >= 0 && y >= 0 && x < 7 && y < 7;

        /// <summary>
        /// Checks whether there's too many spells already next to an Avatar
        /// </summary>
        /// <param name="card">The card to be checking whether it can go there</param>
        /// <param name="x">The x coordinate to check for</param>
        /// <param name="y">The y coordinate to check for</param>
        /// <returns><see langword="false"/> if the card is a spell, 
        /// <paramref name="x"/> and <paramref name="y"/> are next to an Avatar, 
        /// and there's already another spell next to that Avatar. <br></br> 
        /// <see langword="true"/> otherwise.</returns>
        public bool ValidSpellSpaceFor(GameCard card, int x, int y)
        {
            //true for non-spells
            if (card == null || card.CardType != 'S') return true;

            //if it's a spell going to a relevant location, count other adjacent spells to the avatar
            if (x >= 5 && y >= 5) return CardsAdjacentTo(6, 6).Count(c => c != card && c.CardType == 'S') < 1;
            else if (x <= 1 && y <= 1) return CardsAdjacentTo(0, 0).Count(c => c != card && c.CardType == 'S') < 1;

            //if it's not in a relevant location, everything is fine
            return true;
        }

        //get game data
        public GameCard GetCardAt(int x, int y) => ValidIndices(x, y) ? Board[x, y] : null;

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
            foreach (var card in Board) if (predicate(card)) list.Add(card);
            return list;
        }

        public List<GameCard> CardsAndAugsWhere(Func<GameCard, bool> predicate)
        {
            var list = new List<GameCard>();
            foreach(var card in Board)
            {
                if (predicate(card)) list.Add(card);
                if (card != null) list.AddRange(card.AugmentsList.Where(predicate));
            }
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
            foreach (var c in Board)
            {
                if (predicate(c)) return true;
            }
            return false;
        }
        #endregion

        #region game mechanics
        public bool RemoveFromBoard(GameCard toRemove)
        {
            var (x, y) = toRemove.Position;
            if (toRemove.Location == CardLocation.Field && Board[x, y] == toRemove)
            {
                RemoveFromBoard(x, y);
                return true;
            }
            return false;
        }

        public void RemoveFromBoard(int x, int y) => Board[x, y] = null;

        /// <summary>
        /// Puts the card on the board
        /// </summary>
        /// <param name="toPlay">Card to be played</param>
        /// <param name="toX">X coordinate to play the card to</param>
        /// <param name="toY">Y coordinate to play the card to</param>
        public virtual bool Play(GameCard toPlay, int toX, int toY, Player controller, IStackable stackSrc = null)
        {
            if (toPlay == null) return false;
            if (toPlay.Location == CardLocation.Field) return false;
            if (!ValidSpellSpaceFor(toPlay, toX, toY))
            {
                Debug.LogError($"Tried to play {toPlay} to space {toX}, {toY}. " +
                    $"This isn't ok, that's an invalid spell spot.");
                return false;
            }

            Debug.Log($"In boardctrl, playing {toPlay.CardName} to {toX}, {toY}");

            //augments can't be played to a regular space.
            if (toPlay.CardType == 'A')
            {
                //augments therefore just get put on whatever card is on that space rn.
                var augmented = Board[toX, toY];
                //if there isn't a card, well, you can't do that.
                if (augmented == null)
                {
                    Debug.LogError($"Can't play an augment to empty space at {toX}, {toY}");
                    return false;
                }
                //assuming there is a card there, try and add the augment. if it don't work, it borked.
                if (!Board[toX, toY].AddAugment(toPlay, stackSrc)) return false;
            }
            //otherwise, put a card to the requested space
            else
            {
                toPlay.Remove(stackSrc);
                Board[toX, toY] = toPlay;
                toPlay.Location = CardLocation.Field;
                toPlay.Position = (toX, toY);
            }

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
            GameCard temp = Board[toX, toY];
            //check valid spell positioning
            if (!ValidSpellSpaceFor(card, toX, toY) || !ValidSpellSpaceFor(temp, tempX, tempY))
            {
                Debug.LogError($"Tried to move {card} to space {toX}, {toY}. " +
                    $"{(temp == null ? "" : $"This would swap {temp.CardName} to {tempX}, {tempY}.")}" +
                    $"This isn't ok, that's an invalid spell spot.");
                return false;
            }

            Board[toX, toY] = card;
            Board[tempX, tempY] = temp;

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
                if (Board[toX, toY] != null && card.Remove(stackSrc))
                {
                    Board[toX, toY].AddAugment(card, stackSrc);
                    return true;
                }
                return false;
            }
            else return Swap(card, toX, toY, playerInitiated, stackSrc);
        }

        public void ClearSpells()
        {
            foreach (GameCard c in Board)
            {
                if (c == null) continue;
                else if (c.CardType == 'S')
                {
                    switch (c.SpellSubtype)
                    {
                        case CardBase.SimpleSubtype: 
                            c.Discard();
                            break;
                        case CardBase.DelayedSubtype:
                        case CardBase.VanishingSubtype:
                            if (c.TurnsOnBoard >= c.Arg) c.Discard();
                            break;
                    }
                }
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