using KompasCore.Cards;
using KompasCore.GameCore;

namespace KompasCore.Effects
{
    /// <summary>
    /// An object to hold all the parameters required to initialize any restriction/restriction elemnt
    /// </summary>
    public struct EffectInitializationContext
    {
        public readonly Game game;
        public readonly GameCard source;

        public readonly Effect effect;

        public readonly Trigger trigger;
        public readonly Subeffect subeffect;

        public Player Controller => effect?.Controller ?? source?.Controller;

        public EffectInitializationContext(Game game, GameCard source, 
            Effect effect = default, Trigger trigger = default, Subeffect subeffect = default)
        {
            this.game = game;
            this.source = source;

            this.effect = effect;

            this.trigger = trigger;
            this.subeffect = subeffect;
        }
    }
}