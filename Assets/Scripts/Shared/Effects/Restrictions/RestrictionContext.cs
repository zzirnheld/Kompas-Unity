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
        public readonly Subeffect subeffect;

        public RestrictionContext(Game game, GameCard source, Subeffect subeffect = default)
        {
            this.game = game;
            this.source = source;
            this.subeffect = subeffect;
        }
    }
}