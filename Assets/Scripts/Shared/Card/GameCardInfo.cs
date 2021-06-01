using KompasCore.Effects;
using System;
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
        string SpellSubtype { get; }
        GameCard AugmentedCard { get; }
        IEnumerable<GameCard> Augments { get; }
        bool KnownToEnemy { get; }

        PlayRestriction PlayRestriction { get; }

        bool Fast { get; }

        int N { get; }
        int E { get; }
        int S { get; }
        int W { get; }
        int C { get; }
        int A { get; }
        int Cost { get; }
        int BaseE { get; }

        bool Negated { get; }
        int SpacesMoved { get; }
        int SpacesCanMove { get; }
        IEnumerable<GameCard> AdjacentCards { get; }

        Space Position { get; }

        int DistanceTo(Space space);
        int DistanceTo(IGameCardInfo card);
        bool IsAdjacentTo(IGameCardInfo card);
        bool IsAdjacentTo(Space pos);
        bool SameColumn(IGameCardInfo card);
        bool WithinSpaces(int spaces, IGameCardInfo card);
        bool InCorner();
        int ShortestPath(Space space, Func<GameCard, bool> throughPredicate);
        bool CardInAOE(IGameCardInfo c);
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
        public bool KnownToEnemy { get; }

        public PlayRestriction PlayRestriction { get; }

        public bool Fast { get; }

        public int N { get; }
        public int E { get; }
        public int S { get; }
        public int W { get; }
        public int C { get; }
        public int A { get; }
        public int Cost { get; }
        public int BaseE { get; }

        public bool Negated { get; }
        public int SpacesMoved { get; }
        public int SpacesCanMove { get; }
        public IEnumerable<GameCard> AdjacentCards { get; }

        public Space Position { get; }
        public Space SubjectivePosition => Controller.SubjectiveCoords(Position);

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
            KnownToEnemy = card.KnownToEnemy;
            PlayRestriction = card.PlayRestriction;
            Fast = card.Fast;
            N = card.N;
            E = card.E;
            S = card.S;
            W = card.W;
            C = card.C;
            A = card.A;
            BaseE = card.BaseE;
            Cost = card.Cost;
            Negated = card.Negated;
            SpacesMoved = card.SpacesMoved;
            SpacesCanMove = card.SpacesCanMove;
            AdjacentCards = card.AdjacentCards.ToArray();
            Position = card.Position;
        }

        #region distance/adjacency
        public int DistanceTo(Space space)
            => Location == CardLocation.Field ? Position.DistanceTo(space) : int.MaxValue;
        public int DistanceTo(IGameCardInfo card) => DistanceTo(card.Position);

        public bool WithinSpaces(int numSpaces, IGameCardInfo card)
            => card != null && card.Location == CardLocation.Field && Location == CardLocation.Field && DistanceTo(card) <= numSpaces;

        public bool IsAdjacentTo(IGameCardInfo card) => Location == CardLocation.Field && card != null
            && card.Location == CardLocation.Field && Position.AdjacentTo(card.Position);
        public bool IsAdjacentTo(Space space) => Location == CardLocation.Field && Position.AdjacentTo(space);

        public bool SpaceInAOE(Space space) => SpellSubtype == CardBase.RadialSubtype && DistanceTo(space) <= Arg;
        public bool CardInAOE(IGameCardInfo c) => SpaceInAOE(c.Position);

        public bool SameColumn(Space space) => Location == CardLocation.Field && Position.SameColumn(space);
        public bool SameColumn(IGameCardInfo c) => c.Location == CardLocation.Field && SameColumn(c.Position);

        /// <summary>
        /// Returns whether the <paramref name="space"/> passed in is in front of this card
        /// </summary>
        /// <param name="space">The space to check if it's in front of this card</param>
        /// <returns><see langword="true"/> if <paramref name="space"/> is in front of this, <see langword="false"/> otherwise.</returns>
        public bool SpaceInFront(Space space) => Controller.SubjectiveCoords(space).NorthOf(SubjectivePosition);

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
        public bool SpaceBehind(Space space) => SubjectivePosition.NorthOf(Controller.SubjectiveCoords(space));

        /// <summary>
        /// Returns whether the card passed in is behind this card
        /// </summary>
        /// <param name="card">The card to check if it's behind this one</param>
        /// <returns><see langword="true"/> if <paramref name="card"/> is behind this, <see langword="false"/> otherwise.</returns>
        public bool CardBehind(IGameCardInfo card) => SpaceBehind(card.Position);

        public bool SpaceDirectlyInFront(Space space)
            => Location == CardLocation.Field && Controller.SubjectiveCoords(space) == SubjectivePosition.DueNorth;

        public bool CardDirectlyInFront(IGameCardInfo card)
            => card.Location == CardLocation.Field && SpaceDirectlyInFront(card.Position);

        public bool OnMyDiagonal(Space space) => Location == CardLocation.Field && Position.SameDiagonal(space);

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

        public int ShortestPath(int x, int y, Func<GameCard, bool> throughPredicate)
            => Card.Game.boardCtrl.ShortestPath(Card, x, y, throughPredicate);
        #endregion distance/adjacency
    }
}