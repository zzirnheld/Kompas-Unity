using KompasCore.Cards;
using UnityEngine;

namespace KompasCore.GameCore
{
    public interface IGameLocation
    {
        public CardLocation CardLocation { get; }

        public int IndexOf(GameCard card);

        public void Remove(GameCard card);
    }

    /// <summary>
    /// Abstract GameLocation class containing shared logic for GameLocations that don't have to care about server vs client game/player
    /// </summary>
    public abstract class GameLocation : MonoBehaviour, IGameLocation
    {
        public Game game;
        public Player owner;

        public abstract CardLocation CardLocation { get; }

        public abstract int IndexOf(GameCard card);

        public abstract void Remove(GameCard card);

        public override string ToString() => $"{GetType()} owned by {owner}";
    }
}