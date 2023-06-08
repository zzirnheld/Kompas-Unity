using System.Collections.Generic;
using KompasCore.Cards;
using UnityEngine;

namespace KompasCore.GameCore
{
	public interface IGameLocation
	{
		public Game Game { get; }
		public CardLocation CardLocation { get; }

		public IEnumerable<GameCard> Cards { get; }

		public int IndexOf(GameCard card);

		public void Remove(GameCard card);

		public void Refresh();
	}

	/// <summary>
	/// Base class for GameLocations owned by a player (from whom we can infer what game they're in)
	/// </summary>
	public abstract class OwnedGameLocation : MonoBehaviour, IGameLocation
	{
		public abstract Player Owner { get; }
		public virtual Game Game => Owner.Game;

		public abstract CardLocation CardLocation { get; }

		public abstract IEnumerable<GameCard> Cards { get; }

		public abstract int IndexOf(GameCard card);

		public abstract void Remove(GameCard card);

		public abstract void Refresh();

		public override string ToString() => $"{GetType()} owned by {Owner}";
	}
}