using KompasCore.Cards;
using KompasCore.GameCore;

namespace KompasCore.Effects
{
    /// <summary>
    /// An object to hold all the parameters required to initialize any restriction/restriction elemnt
    /// </summary>
    public struct RestrictionContext
    {
        public readonly Game game;
        public readonly GameCard source;

        public RestrictionContext(Game game, GameCard source)
        {
            this.game = game;
            this.source = source;
        }
    }
}