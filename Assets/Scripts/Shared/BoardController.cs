using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KompasCore.Effects;
using KompasCore.Cards;
using System.Text;
using KompasCore.Exceptions;

namespace KompasCore.GameCore
{
    public class BoardController : MonoBehaviour, IGameLocation
    {
        public const int SpacesInGrid = 7;
        public const float BoardLenOffset = 7f;
        public const float LenOneSpace = 2f;
        public const float SpaceOffset = LenOneSpace / 2f;
        public const float CardHeight = 0.15f;

        public Game game;

        public CardLocation CardLocation => CardLocation.Field;

        public static int PosToGridIndex(float pos) 
            => (int)((pos + BoardLenOffset) / (LenOneSpace));

        public static float GridIndexToPos(int gridIndex)
            => (float)((gridIndex * LenOneSpace) + SpaceOffset - BoardLenOffset);

        public static Vector3 GridIndicesToCardPos(int x, int y)
            => new Vector3(GridIndexToPos(x), CardHeight, GridIndexToPos(y));

        public readonly GameCard[,] Board = new GameCard[SpacesInGrid, SpacesInGrid];

        //helper methods
        #region helper methods
        public int IndexOf(GameCard card) => card.Position.Index;

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

        public bool Surrounded(Space s) => s.AdjacentSpaces.All(s => GetCardAt(s) != null);

        //get game data
        public GameCard GetCardAt(Space s)
        {
            if (s.Valid)
            {
                var (x, y) = s;
                return Board[x, y];
            }
            else return null;
        }

        public List<GameCard> CardsAdjacentTo(Space space)
        {
            var list = new List<GameCard>();
            if (space == null)
            {
                //Debug.LogError("Asking for cards adjacent to a null space");
                return list;
            }

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
            foreach (var card in Board) if (predicate(card)) list.Add(card);
            return list;
        }

        public List<GameCard> CardsAndAugsWhere(Func<GameCard, bool> predicate)
        {
            var list = new List<GameCard>();
            foreach(var card in Board)
            {
                if (predicate(card)) list.Add(card);
                if (card != null) list.AddRange(card.Augments.Where(predicate));
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
            if (Board[destination.x, destination.y] != null) return 50;

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
            foreach (var c in Board)
            {
                if (predicate(c)) return true;
            }
            return false;
        }
        #endregion

        #region game mechanics
        public virtual void Remove(GameCard toRemove)
        {
            if (toRemove.Location != CardLocation.Field) 
                throw new CardNotHereException(CardLocation, $"Tried to remove {toRemove} not on board");
            if (toRemove.Position == null) throw new InvalidSpaceException(toRemove.Position, "Can't remove a card from a null space");
            var (x, y) = toRemove.Position;
            if(Board[x, y] == toRemove) RemoveFromBoard(toRemove.Position);
            else throw new CardNotHereException(CardLocation, $"Card thinks it's at {toRemove.Position}, but {Board[x, y]} is there");
        }

        private void RemoveFromBoard(Space space)
        {
            var (x, y) = space;
            Board[x, y] = null;
        }

        /// <summary>
        /// Puts the card on the board
        /// </summary>
        /// <param name="toPlay">Card to be played</param>
        /// <param name="toX">X coordinate to play the card to</param>
        /// <param name="toY">Y coordinate to play the card to</param>
        public virtual void Play(GameCard toPlay, Space to, Player controller, IStackable stackSrc = null)
        {
            if (toPlay == null) throw new NullCardException($"Null card to play to {to}");
            if (toPlay.Location == CardLocation.Field) throw new AlreadyHereException(CardLocation);
            if (to == null) throw new InvalidSpaceException(to, $"Space to play a card to cannot be null!");
            if (!ValidSpellSpaceFor(toPlay, to)) throw new InvalidSpaceException(to, 
                $"Tried to play {toPlay} to space {to}. This isn't ok, that's an invalid spell spot.");

            Debug.Log($"In boardctrl, playing {toPlay.CardName} to {to}");

            //augments can't be played to a regular space.
            if (toPlay.CardType == 'A')
            {
                //augments therefore just get put on whatever card is on that space rn.
                var augmented = GetCardAt(to);
                //if there isn't a card, well, you can't do that.
                if (augmented == null) throw new NullCardException($"Can't play an augment to empty space at {to}");
                //assuming there is a card there, try and add the augment. if it don't work, it borked.
                augmented.AddAugment(toPlay, stackSrc);

                toPlay.Controller = controller;
            }
            //otherwise, put a card to the requested space
            else
            {
                toPlay.Remove(stackSrc);
                var (toX, toY) = to;
                Board[toX, toY] = toPlay;
                toPlay.Position = to;
                toPlay.GameLocation = this;

                toPlay.Controller = controller;
            }
        }

        //movement
        public virtual void Swap(GameCard card, Space to, bool playerInitiated, IStackable stackSrc = null)
        {
            Debug.Log($"Swapping {card?.CardName} to {to}");

            if (!to.Valid) throw new InvalidSpaceException(to);
            if (card == null) throw new NullCardException("Card to be swapped must not be null");
            if (card.Attached) throw new NotImplementedException();
            if (card.Location != CardLocation.Field || card != GetCardAt(card.Position)) 
                throw new CardNotHereException(CardLocation.Field, 
                    $"{card} not at {card.Position}, {GetCardAt(card.Position)} is there instead");

            var (tempX, tempY) = card.Position;
            var from = card.Position;
            var (toX, toY) = to;
            GameCard temp = Board[toX, toY];
            //check valid spell positioning
            string swapDesc = $"Tried to move {card} to space {toX}, {toY}. " +
                    $"{(temp == null ? "" : $"This would swap {temp.CardName} to {tempX}, {tempY}.")}";
            if (!ValidSpellSpaceFor(card, to)) throw new InvalidSpaceException(to, $"{swapDesc}, but space is an invalid spell space");
            if (!ValidSpellSpaceFor(temp, from)) throw new InvalidSpaceException(from, $"{swapDesc}, but space is an invalid spell space");

            //then let the cards know they've been moved, but before moving them, so you can count properly
            if (playerInitiated)
            {
                card.CountSpacesMovedTo((toX, toY));
                temp?.CountSpacesMovedTo((tempX, tempY));
            }

            Board[toX, toY] = card;
            Board[tempX, tempY] = temp;

            card.Position = to;
            if (temp != null) temp.Position = from;
        }

        public void Move(GameCard card, Space to, bool playerInitiated, IStackable stackSrc = null)
        {
            if (card.Attached)
            {
                if (!to.Valid) throw new InvalidSpaceException(to, $"Can't move {card} to invalid space");
                var (toX, toY) = to;
                if (GetCardAt(to) == null) throw new NullCardException($"Null card to attach {card} to at {to}");
                card.Remove(stackSrc);
                Board[toX, toY].AddAugment(card, stackSrc);
            }
            else
            {
                Swap(card, to, playerInitiated, stackSrc);
            }
        }

        public void ClearSpells()
        {
            foreach (GameCard c in Board)
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
                                if (c.TurnsOnBoard >= c.Duration) c.Vanish();
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
                    var card = Board[i, j];
                    if (card != null) sb.Append($"At {i}, {j}, {card.CardName} id {card.ID}");
                }
            }
            return sb.ToString();
        }
    }
}