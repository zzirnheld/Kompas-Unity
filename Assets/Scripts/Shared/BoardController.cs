using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KompasCore.Effects;
using KompasCore.Cards;
using System.Text;

namespace KompasCore.GameCore
{
    public class BoardController : MonoBehaviour
    {
        public const int SpacesInGrid = 7;
        public const float BoardLenOffset = 7f;
        public const float LenOneSpace = 2f;
        public const float SpaceOffset = LenOneSpace / 2f;
        public const float CardHeight = 0.15f;

        public Game game;

        public static int PosToGridIndex(float pos) 
            => (int)((pos + BoardLenOffset) / (LenOneSpace));

        public static float GridIndexToPos(int gridIndex)
            => (float)((gridIndex * LenOneSpace) + SpaceOffset - BoardLenOffset);

        public static Vector3 GridIndicesToCardPos(int x, int y)
            => new Vector3(GridIndexToPos(x), CardHeight, GridIndexToPos(y));

        public readonly GameCard[,] board = new GameCard[SpacesInGrid, SpacesInGrid];
        public readonly List<GameCard>[,] augments = new List<GameCard>[SpacesInGrid, SpacesInGrid];

        private void Awake()
        {
            for (int x = 0; x < SpacesInGrid; x++)
            {
                for (int y = 0; y < SpacesInGrid; y++)
                {
                    augments[x, y] = new List<GameCard>();
                }
            }
        }

        //helper methods
        #region helper methods
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
        public bool ValidSpellSpaceFor(GameCard card, Space space)
        {
            //true for non-spells
            if (card == null || card.CardType != 'S') return true;

            var (x, y) = space;
            //if it's a spell going to a relevant location, count other adjacent spells to the avatar
            if (x >= 5 && y >= 5 && space != (5, 5)) 
                return CardsAdjacentTo(Space.FarCorner)
                    .Count(c => c != card && c.CardType == 'S' && c.Controller == card.Controller) < 1;
            else if (x <= 1 && y <= 1 && space != (1, 1)) 
                return CardsAdjacentTo(Space.NearCorner)
                    .Count(c => c != card && c.CardType == 'S' && c.Controller == card.Controller) < 1;

            //if it's not in a relevant location, everything is fine
            return true;
        }

        //get game data
        public GameCard GetCardAt(Space s)
        {
            if (s.Valid)
            {
                var (x, y) = s;
                return board[x, y];
            }
            else return null;
        }

        public List<GameCard> CardsAdjacentTo(Space space)
        {
            var list = new List<GameCard>();

            foreach (var s in space.AdjacentSpaces)
            { 
                var card = GetCardAt(s);
                if (card != null) list.Add(card);
            }

            return list;
        }

        public List<GameCard> CardsWhere(Func<GameCard, bool> predicate)
        {
            var list = new List<GameCard>();
            foreach (var card in board) if (predicate(card)) list.Add(card);
            return list;
        }

        public List<GameCard> CardsAndAugsWhere(Func<GameCard, bool> predicate)
        {
            var list = new List<GameCard>();
            foreach(var card in board)
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
        /// <param name="throughPredicate">What all cards you go through must fit</param>
        /// <returns></returns>
        public int ShortestPath(Space src, Space space, Func<Space, bool> throughPredicate)
        {
            //record shortest distances to cards
            var dist = new Dictionary<Space, int>();
            //and if you've seen them
            var seen = new HashSet<Space>();
            //the queue of nodes to process next. things should only go on here once, the first time they're seen
            var queue = new Queue<Space>();

            //set up the structures with the source node
            queue.Enqueue(src);
            dist.Add(src, 0);
            seen.Add(src);

            //iterate until the queue is empty, in which case you'll have seen all connected cards that fit the restriction.
            while (queue.Any())
            {
                //consider the next node's adjacent cards
                var next = queue.Dequeue();
                foreach (var card in space.AdjacentSpaces.Where(throughPredicate))
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
            foreach (var s in space.AdjacentSpaces)
            {
                if (dist.ContainsKey(s) && dist[s] < min) min = dist[s];
            }
            return min;
        }

        public int ShortestPath(Space source, Space end, Func<GameCard, bool> throughPredicate)
            => ShortestPath(source, end, s => throughPredicate(GetCardAt(s)));

        public int ShortestPath(GameCard source, Space space, CardRestriction restriction, Effects.ActivationContext context)
            => ShortestPath(source.Position, space, c => restriction.Evaluate(c, context));

        private IEnumerable<Space> AdjacentEmptySpacesTo(Space space)
        {
            return space.AdjacentSpaces.Where(s => GetCardAt(s) == null);
        }

        public int ShortestEmptyPath(GameCard src, Space destination)
        {
            if (board[destination.x, destination.y] != null) return 50;

            int[,] dist = new int[7, 7];
            bool[,] seen = new bool[7, 7];

            var queue = new Queue<Space>();

            queue.Enqueue(src.Position);
            dist[src.Position.x, src.Position.y] = 0;
            seen[src.Position.x, src.Position.y] = true;

            while (queue.Any())
            {
                var next = queue.Dequeue();
                foreach(Space s in AdjacentEmptySpacesTo(next))
                {
                    if(!seen[s.x, s.y])
                    {
                        seen[s.x, s.y] = true;
                        queue.Enqueue(s);
                        dist[s.x, s.y] = dist[next.x, next.y] + 1;
                    }
                    else if (dist[next.x, next.y] + 1 < dist[s.x, s.y]) dist[s.x, s.y] = dist[next.x, next.y] + 1;
                }
            }

            return dist[destination.x, destination.y] <= 0 ? 50 : dist[destination.x, destination.y];
        }

        public bool ExistsCardOnBoard(Func<GameCard, bool> predicate)
        {
            foreach (var c in board)
            {
                if (predicate(c)) return true;
            }
            return false;
        }
        #endregion

        #region game mechanics
        public virtual bool RemoveFromBoard(GameCard toRemove)
        {
            var (x, y) = toRemove.Position;
            if (toRemove.Location == CardLocation.Field && board[x, y] == toRemove)
            {
                RemoveFromBoard(toRemove.Position);
                return true;
            }
            return false;
        }

        private void RemoveFromBoard(Space space)
        {
            var (x, y) = space;
            board[x, y] = null;
        }

        /// <summary>
        /// Puts the card on the board
        /// </summary>
        /// <param name="toPlay">Card to be played</param>
        /// <param name="toX">X coordinate to play the card to</param>
        /// <param name="toY">Y coordinate to play the card to</param>
        public virtual bool Play(GameCard toPlay, Space to, Player controller, IStackable stackSrc = null)
        {
            if (toPlay == null) return false;
            if (toPlay.Location == CardLocation.Field) return false;
            if (!ValidSpellSpaceFor(toPlay, to))
            {
                Debug.LogError($"Tried to play {toPlay} to space {to}. " +
                    $"This isn't ok, that's an invalid spell spot.");
                return false;
                //TODO make this instead an exception that's caught by subeffs
            }

            Debug.Log($"In boardctrl, playing {toPlay.CardName} to {to}");

            //augments can't be played to a regular space.
            if (toPlay.CardType == 'A')
            {
                //augments therefore just get put on whatever card is on that space rn.
                var augmented = GetCardAt(to);
                //if there isn't a card, well, you can't do that.
                if (augmented == null)
                {
                    Debug.LogError($"Can't play an augment to empty space at {to}");
                    return false;
                }
                //assuming there is a card there, try and add the augment. if it don't work, it borked.
                if (!augmented.AddAugment(toPlay, stackSrc)) return false;

                toPlay.Controller = controller;
                return true;
            }
            //otherwise, put a card to the requested space
            else
            {
                if (toPlay.Remove(stackSrc))
                {
                    var (toX, toY) = to;
                    board[toX, toY] = toPlay;
                    toPlay.Location = CardLocation.Field;
                    toPlay.Position = to;

                    toPlay.Controller = controller;
                    return true;
                }
            }

            return false;
        }

        //movement
        public virtual bool Swap(GameCard card, Space to, bool playerInitiated, IStackable stackSrc = null)
        {
            Debug.Log($"Swapping {card?.CardName} to {to}");

            if (!to.Valid || card == null) return false;
            if (card.AugmentedCard != null) throw new NotImplementedException();

            var (tempX, tempY) = card.Position;
            var from = card.Position;
            var (toX, toY) = to;
            GameCard temp = board[toX, toY];
            //check valid spell positioning
            if (!ValidSpellSpaceFor(card, to) || !ValidSpellSpaceFor(temp, from))
            {
                Debug.LogError($"Tried to move {card} to space {toX}, {toY}. " +
                    $"{(temp == null ? "" : $"This would swap {temp.CardName} to {tempX}, {tempY}.")}" +
                    $"This isn't ok, that's an invalid spell spot.");
                return false;
            }

            //then let the cards know they've been moved, but before moving them, so you can count properly
            if (playerInitiated)
            {
                card.CountSpacesMovedTo((toX, toY));
                temp?.CountSpacesMovedTo((tempX, tempY));
            }

            board[toX, toY] = card;
            board[tempX, tempY] = temp;

            card.Position = to;
            if (temp != null) temp.Position = from;
            return true;
        }

        public bool Move(GameCard card, Space to, bool playerInitiated, IStackable stackSrc = null)
        {
            if (!to.Valid) return false;

            if (card.AugmentedCard != null)
            {
                var (toX, toY) = to;
                if (board[toX, toY] != null && card.Remove(stackSrc))
                {
                    board[toX, toY].AddAugment(card, stackSrc);
                    return true;
                }
                return false;
            }
            else return Swap(card, to, playerInitiated, stackSrc);
        }

        public virtual bool PlaceAugment(GameCard aug, Space to, IStackable stackSrc = null)
        {
            if (!aug.Remove(stackSrc)) return false;

            var (x, y) = to;
            augments[x, y].Add(aug);

            return true;
        }

        public void ClearSpells()
        {
            foreach (GameCard c in board)
            {
                if (c == null) continue;
                else if (c.CardType == 'S')
                {
                    foreach (string s in c.SpellSubtypes)
                    {
                        switch (s)
                        {
                            case CardBase.SimpleSubtype:
                                c.Discard();
                                break;
                            case CardBase.DelayedSubtype:
                            case CardBase.VanishingSubtype:
                                if (c.TurnsOnBoard >= c.Duration) c.Discard();
                                break;
                        }
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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    var card = board[i, j];
                    if (card != null) sb.Append($"At {i}, {j}, {card.CardName} id {card.ID}");
                }
            }
            return sb.ToString();
        }
    }
}