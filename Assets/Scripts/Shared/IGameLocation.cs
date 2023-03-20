using System.Collections.Generic;
using System.Linq;
using KompasCore.Cards;
using UnityEngine;

namespace KompasCore.GameCore
{
    public interface IGameLocation
    {
        public CardLocation CardLocation { get; }

        public IEnumerable<GameCard> Cards { get; }

        public int IndexOf(GameCard card);

        public void Remove(GameCard card);
    }

    public abstract class GameLocation : MonoBehaviour, IGameLocation
    {
        public abstract Player Owner { get; }
        public Game Game => Owner.Game;

        public abstract CardLocation CardLocation { get; }

        public abstract IEnumerable<GameCard> Cards { get; }

        public abstract int IndexOf(GameCard card);

        public abstract void Remove(GameCard card);

        public override string ToString() => $"{GetType()} owned by {Owner}";
    }
}