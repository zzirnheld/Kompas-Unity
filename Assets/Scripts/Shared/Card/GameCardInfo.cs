using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.Cards
{
    public interface IGameCardInfo
    {
        GameCard Card { get; }
        CardLocation Location { get; }
        int IndexInList { get; }
        string CardName { get; }
        char CardType { get; }
        Player Controller { get; }
        Player Owner { get; }
        bool Summoned { get; }
        bool IsAvatar { get; }
        string SubtypeText { get; }
        GameCard AugmentedCard { get; }
        IEnumerable<GameCard> Augments { get; }

        int N { get; }
        int E { get; }
        int S { get; }
        int W { get; }
        int C { get; }
        int A { get; }
        int Cost { get; }
        int BaseE { get; }

        bool Negated { get; }
        IEnumerable<GameCard> AdjacentCards { get; }

        (int x, int y) Position { get; }

        int DistanceTo(int x, int y);
        int DistanceTo(IGameCardInfo card);
        bool IsAdjacentTo(IGameCardInfo card);
        bool IsAdjacentTo((int x, int y) pos);
        bool SameColumn(IGameCardInfo card);
        bool WithinSpaces(int spaces, IGameCardInfo card);
        bool InCorner();
    }

    /// <summary>
    /// Holds the info for a card at a given snapshot in time.
    /// Used for triggers.
    /// </summary>
    public class GameCardInfo : IGameCardInfo
    {
        public GameCard Card { get; }
        public char CardType { get; }
        public string SpellSubtype { get; }
        public int Arg { get; }

        public CardLocation Location { get; }
        public int IndexInList { get; }
        public string CardName { get; }
        public Player Controller { get; }
        public Player Owner { get; }
        public bool Summoned { get; }
        public bool IsAvatar { get; }
        public string SubtypeText { get; }
        public GameCard AugmentedCard { get; }
        public IEnumerable<GameCard> Augments { get; }

        public int N { get; }
        public int E { get; }
        public int S { get; }
        public int W { get; }
        public int C { get; }
        public int A { get; }
        public int Cost { get; }
        public int BaseE { get; }

        public bool Negated { get; }
        public IEnumerable<GameCard> AdjacentCards { get; }

        public (int x, int y) Position { get; }
        public (int x, int y) SubjectivePosition => Controller.SubjectiveCoords(Position);

        public GameCardInfo(GameCard card)
        {
            Card = card;
            Location = card.Location;
            IndexInList = card.IndexInList;
            CardName = card.CardName;
            CardType = card.CardType;
            SpellSubtype = card.SpellSubtype;
            Arg = card.Arg;
            Controller = card.Controller;
            Owner = card.Owner;
            Summoned = card.Summoned;
            IsAvatar = card.IsAvatar;
            SubtypeText = card.SubtypeText;
            AugmentedCard = card.AugmentedCard;
            Augments = card.Augments.ToArray();
            N = card.N;
            E = card.E;
            S = card.S;
            W = card.W;
            C = card.C;
            A = card.A;
            BaseE = card.BaseE;
            Cost = card.Cost;
            Negated = card.Negated;
            AdjacentCards = card.AdjacentCards.ToArray();
            Position = card.Position;
        }

        #region distance/adjacency
        public int DistanceTo(int x, int y)
        {
            if (Location != CardLocation.Field) return int.MaxValue;
            return Mathf.Abs(x - Position.x) > Mathf.Abs(y - Position.y) ? Mathf.Abs(x - Position.x) : Mathf.Abs(y - Position.y);
            /* equivalent to
             * if (Mathf.Abs(card.X - X) > Mathf.Abs(card.Y - Y)) return Mathf.Abs(card.X - X);
             * else return Mathf.Abs(card.Y - Y);
             * is card.X - X > card.Y - Y? If so, return card.X -X, otherwise return card.Y - Y
            */
        }
        public int DistanceTo((int x, int y) space) => DistanceTo(space.x, space.y);
        public int DistanceTo(IGameCardInfo card) => DistanceTo(card.Position);
        public bool WithinSpaces(int numSpaces, IGameCardInfo card)
            => card != null && card.Location == CardLocation.Field && Location == CardLocation.Field && DistanceTo(card) <= numSpaces;
        public bool WithinSpaces(int numSpaces, int x, int y) => DistanceTo(x, y) <= numSpaces;
        public bool IsAdjacentTo(IGameCardInfo card) => Location == CardLocation.Field && card != null
            && card.Location == CardLocation.Field && DistanceTo(card) == 1;
        public bool IsAdjacentTo(int x, int y) => Location == CardLocation.Field && DistanceTo(x, y) == 1;
        public bool IsAdjacentTo((int x, int y) pos) => IsAdjacentTo(pos.x, pos.y);
        public bool CardInAOE(IGameCardInfo c) => SpaceInAOE(c.Position);
        public bool SpaceInAOE((int x, int y) space) => SpaceInAOE(space.x, space.y);
        public bool SpaceInAOE(int x, int y)
            => CardType == 'S' && SpellSubtype == CardBase.RadialSubtype && DistanceTo(x, y) <= Arg;
        public bool SameColumn(int x, int y) => Position.x - Position.y == x - y;
        public bool SameColumn(IGameCardInfo c) => c.Location == CardLocation.Field && SameColumn(c.Position.x, c.Position.y);

        /// <summary>
        /// Returns whether the <paramref name="space"/> passed in is in front of this card
        /// </summary>
        /// <param name="space">The space to check if it's in front of this card</param>
        /// <returns><see langword="true"/> if <paramref name="space"/> is in front of this, <see langword="false"/> otherwise.</returns>
        public bool SpaceInFront((int x, int y) space) => Controller.SubjectiveCoord(space.x) > SubjectivePosition.x;

        /// <summary>
        /// Returns whether the card passed in is in front of this card
        /// </summary>
        /// <param name="card">The card to check if it's in front of this one</param>
        /// <returns><see langword="true"/> if <paramref name="card"/> is in front of this, <see langword="false"/> otherwise.</returns>
        public bool CardInFront(IGameCardInfo card) => SpaceInFront(card.Position);

        /// <summary>
        /// Returns whether the <paramref name="space"/> passed in is behind this card
        /// </summary>
        /// <param name="space">The space to check if it's behind this card</param>
        /// <returns><see langword="true"/> if <paramref name="space"/> is behind this, <see langword="false"/> otherwise.</returns>
        public bool SpaceBehind((int x, int y) space) => Controller.SubjectiveCoord(space.x) < SubjectivePosition.x;

        /// <summary>
        /// Returns whether the card passed in is behind this card
        /// </summary>
        /// <param name="card">The card to check if it's behind this one</param>
        /// <returns><see langword="true"/> if <paramref name="card"/> is behind this, <see langword="false"/> otherwise.</returns>
        public bool CardBehind(IGameCardInfo card) => SpaceBehind(card.Position);

        public bool SpaceDirectlyInFront((int x, int y) space)
            => Controller.SubjectiveCoords(space) == (SubjectivePosition.x + 1, SubjectivePosition.y + 1);

        public bool CardDirectlyInFront(IGameCardInfo card)
            => Location == CardLocation.Field && card.Location == CardLocation.Field && SpaceDirectlyInFront(card.Position);

        public bool OnMyDiagonal((int x, int y) space) => Location == CardLocation.Field && (Position.x == space.x || Position.y == space.y);

        public bool InCorner() => (Position.x == 0 || Position.x == 6) && (Position.y == 0 || Position.y == 6);

        /// <summary>
        /// Refers to this situation: <br></br>
        /// | <paramref name="space"/> | <br></br>
        /// | this card | <br></br>
        /// | <paramref name="card"/> param | <br></br>
        /// </summary>
        /// <param name="space">The space in the same axis as this card and <paramref name="card"/> param</param>
        /// <param name="card">The card in the same axis as this card and the <paramref name="space"/> param.</param>
        /// <returns></returns>
        public bool SpaceDirectlyAwayFrom((int x, int y) space, IGameCardInfo card)
        {
            if (card.Location != CardLocation.Field || Location != CardLocation.Field) return false;
            int xDiffCard = card.Position.x - Position.x;
            int yDiffCard = card.Position.y - Position.y;
            int xDiffSpace = space.x - Position.x;
            int yDiffSpace = space.y - Position.y;

            return (xDiffCard == 0 && xDiffSpace == 0)
                || (yDiffCard == 0 && yDiffSpace == 0)
                || (xDiffCard == yDiffCard && xDiffSpace == yDiffSpace);
        }
        #endregion distance/adjacency
    }
}