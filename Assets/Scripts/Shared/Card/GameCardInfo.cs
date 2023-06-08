using KompasCore.Effects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Cards
{
	public abstract class GameCardBase : CardBase
	{
		#region immutable aspects
		public abstract GameCard Card { get; }

		public abstract int IndexInList { get; }
		public abstract Player Owner { get; }
		public abstract bool Summoned { get; }
		public abstract bool IsAvatar { get; }

		public abstract IReadOnlyCollection<GameCard> AdjacentCards { get; }

		public abstract PlayRestriction PlayRestriction { get; }
		public abstract MovementRestriction MovementRestriction { get; }
		/// <summary>
        /// When attacking, this restriction must be true of the defender.
        /// </summary>
		public abstract IRestriction<GameCardBase> AttackingDefenderRestriction { get; }
		#endregion

		#region mutable aspects
		public abstract CardLocation Location { get; protected set; }
		public abstract Player Controller { get; set; }
		public abstract GameCard AugmentedCard { get; protected set; }
		public abstract IReadOnlyCollection<GameCard> Augments { get; protected set; }
		/// <summary>
		/// Represents whether this card is currently known to the enemy of this player.
		/// TODO: make this also be accurate on client, remembering what thigns have been revealed
		/// </summary>
		public abstract bool KnownToEnemy { get; set; }

		public abstract bool Activated { get; protected set; }
		public abstract bool Negated { get; protected set; }
		public abstract int SpacesMoved { get; set; }
		public int SpacesCanMove => N - SpacesMoved;

		public abstract Space Position { get; set; }
		#endregion

		public bool Hurt => CardType == 'C' && Location == CardLocation.Board && E < BaseE;

		#region distance/adjacency
		public Space SubjectivePosition => Controller.SubjectiveCoords(Position);

		public int RadiusDistanceTo(Space space)
			=> Location == CardLocation.Board ? Position.RadiusDistanceTo(space) : int.MaxValue;
		public int DistanceTo(Space space)
			=> Location == CardLocation.Board ? Position.DistanceTo(space) : int.MaxValue;
		public int DistanceTo(GameCardBase card) => DistanceTo(card.Position);

		public bool WithinSpaces(int numSpaces, GameCardBase card)
			=> card != null && card.Location == CardLocation.Board && Location == CardLocation.Board && DistanceTo(card) <= numSpaces;

		public bool IsAdjacentTo(GameCardBase card) => Location == CardLocation.Board && card != null
			&& card.Location == CardLocation.Board && Position.IsAdjacentTo(card.Position);
		public bool IsAdjacentTo(Space space) => Location == CardLocation.Board && Position.IsAdjacentTo(space);

		/// <summary>
		/// Whether <paramref name="space"/> is in this card's AOE if this card is at <paramref name="mySpace"/>
		/// </summary>
		public bool SpaceInAOE(Space space, Space mySpace)
			=> space != null && mySpace != null && SpellSubtypes != null && SpellSubtypes.Any(s => s switch
			{
				RadialSubtype => mySpace.DistanceTo(space) <= Radius,
				_ => false
			});
		public bool SpaceInAOE(Space space) => SpaceInAOE(space, Position);
		/// <summary>
		/// Whether <paramref name="c"/> is in this card's AOE if this card is at <paramref name="mySpace"/>
		/// </summary>
		public bool CardInAOE(GameCardBase c, Space mySpace) => SpaceInAOE(c.Position, mySpace);
		/// <summary>
		/// Whether <paramref name="c"/> is in the aoe of <see cref="this"/> card.
		/// </summary>
		public bool CardInAOE(GameCardBase c) => CardInAOE(c, Position);
		/// <summary>
		/// Whether <paramref name="c"/> and this card have any spaces shared between their AOEs,
		/// if this card is at <paramref name="mySpace"/>
		/// </summary>
		public bool Overlaps(GameCardBase c, Space mySpace) => Space.Spaces.Any(s => SpaceInAOE(s, mySpace) && c.SpaceInAOE(s));
		/// <summary>
		/// Whether <paramref name="c"/> and this card have any spaces shared between their AOEs
		/// </summary>
		public bool Overlaps(GameCardBase c) => Overlaps(c, Position);

		public bool SameColumn(Space space) => Location == CardLocation.Board && Position.SameColumn(space);
		public bool SameColumn(GameCardBase c) => c.Location == CardLocation.Board && SameColumn(c.Position);

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
		public bool CardInFront(GameCardBase card) => SpaceInFront(card.Position);

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
		public bool CardBehind(GameCardBase card) => SpaceBehind(card.Position);

		public bool SpaceDirectlyInFront(Space space)
			=> Location == CardLocation.Board && Controller.SubjectiveCoords(space) == SubjectivePosition.DueNorth;

		public bool CardDirectlyInFront(GameCardBase card)
			=> card.Location == CardLocation.Board && SpaceDirectlyInFront(card.Position);

		public bool SameDiagonal(Space space) => Location == CardLocation.Board && Position.SameDiagonal(space);
		public bool SameDiagonal(GameCardBase card) => card?.Location == CardLocation.Board && SameDiagonal(card.Position);

		public bool InCorner() => Location == CardLocation.Board && Position.IsCorner;

		/// <summary>
		/// Refers to this situation: <br></br>
		/// | <paramref name="space"/> | <br></br>
		/// | this card | <br></br>
		/// | <paramref name="card"/> param | <br></br>
		/// </summary>
		/// <param name="space">The space in the same axis as this card and <paramref name="card"/> param</param>
		/// <param name="card">The card in the same axis as this card and the <paramref name="space"/> param.</param>
		/// <returns></returns>
		public bool SpaceDirectlyAwayFrom((int x, int y) space, GameCardBase card)
		{
			if (card.Location != CardLocation.Board || Location != CardLocation.Board) return false;
			int xDiffCard = card.Position.x - Position.x;
			int yDiffCard = card.Position.y - Position.y;
			int xDiffSpace = space.x - Position.x;
			int yDiffSpace = space.y - Position.y;

			return (xDiffCard == 0 && xDiffSpace == 0)
				|| (yDiffCard == 0 && yDiffSpace == 0)
				|| (xDiffCard == yDiffCard && xDiffSpace == yDiffSpace);
		}

		public int ShortestPath(Space space, Func<GameCard, bool> throughPredicate)
			=> Card.Game.BoardController.ShortestPath(Card.Position, space, throughPredicate);
		#endregion distance/adjacency

		public bool HasSubtype(string subtype) => SubtypeText.ToLower().Contains(subtype.ToLower());


		protected GameCardBase(CardStats stats,
							string subtext, string[] spellTypes,
							bool unique,
							int radius, int duration,
							char cardType, string cardName, string fileName,
							string effText,
							string subtypeText)
			: base(stats, subtext, spellTypes, unique, radius, duration, cardType, cardName, fileName, effText, subtypeText)
		{ }

		protected GameCardBase(SerializableCard card, string fileName)
			: this((card.n, card.e, card.s, card.w, card.c, card.a),
					   card.subtext, card.spellTypes,
					   card.unique,
					   card.radius, card.duration,
					   card.cardType, card.cardName, fileName,
					   card.effText,
					   card.subtypeText)
		{ }
	}

	/// <summary>
	/// Holds the info for a card at a given snapshot in time.
	/// Used for triggers.
	/// </summary>
	public class GameCardInfo : GameCardBase
	{
		#region immutable aspects
		public override GameCard Card { get; }

		public override int IndexInList { get; }
		public override Player Owner { get; }
		public override bool Summoned { get; }
		public override bool IsAvatar { get; }

		public override IReadOnlyCollection<GameCard> AdjacentCards { get; }

		public override PlayRestriction PlayRestriction { get; }
		public override MovementRestriction MovementRestriction { get; }
		public override IRestriction<GameCardBase> AttackingDefenderRestriction { get; }


		public override int BaseN { get; }
		public override int BaseE { get; }
		public override int BaseS { get; }
		public override int BaseW { get; }
		public override int BaseC { get; }
		public override int BaseA { get; }
		#endregion

		#region mutable aspects
		//Note for the unfamiliar: most of these have setters so that inheritors can have setters for the same property names without hiding
		public override CardLocation Location { get; protected set; }
		public override Player Controller { get; set; }
		public override GameCard AugmentedCard { get; protected set; }
		public override IReadOnlyCollection<GameCard> Augments { get; protected set; }
		public override bool KnownToEnemy { get; set; }

		public override bool Activated { get; protected set; }
		public override bool Negated { get; protected set; }
		public override int SpacesMoved { get; set; }
		public override Space Position { get; set; }
		#endregion

		/// <summary>
		/// Snapshots the information of a card.
		/// </summary>
		/// <param name="card">The card whose information to snapshot</param>
		/// <returns>A <see cref="GameCardInfo"/> whose information matches the current state of <paramref name="card"/>, 
		/// or null if <paramref name="card"/> is <see langword="null"/></returns>
		public static GameCardInfo CardInfoOf(GameCard card)
		{
			if (card == null) return null;

			return new GameCardInfo(card);
		}

		public GameCardInfo(GameCard card)
			: base(card.Stats,
						card.Subtext, card.SpellSubtypes,
						card.Unique,
						card.Radius, card.Duration,
						card.CardType, card.CardName, card.FileName,
						card.EffText,
						card.SubtypeText)
		{
			Card = card;
			Location = card.Location;
			IndexInList = card.IndexInList;
			Controller = card.Controller;
			Owner = card.Owner;
			Summoned = card.Summoned;
			IsAvatar = card.IsAvatar;
			AugmentedCard = card.AugmentedCard;
			Augments = card.Augments.ToArray();
			KnownToEnemy = card.KnownToEnemy;
			PlayRestriction = card.PlayRestriction;
			MovementRestriction = card.MovementRestriction;
			AttackingDefenderRestriction = card.AttackingDefenderRestriction;

			BaseN = card.BaseN;
			BaseE = card.BaseE;
			BaseS = card.BaseS;
			BaseW = card.BaseW;
			BaseC = card.BaseC;
			BaseA = card.BaseA;

			Activated = card.Activated;
			Negated = card.Negated;
			SpacesMoved = card.SpacesMoved;
			AdjacentCards = card.AdjacentCards.ToArray();
			Position = card.Position?.Copy;
		}

		public override string ToString()
		{
			return CardName;
		}
	}
}