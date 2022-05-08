using System;
using KompasCore.Cards;

namespace KompasCore.Effects.Identities
{
    public abstract class ActivationContextCardIdentity
    {
        private bool initialized;

        public void Initialize()
        {

            initialized = true;
        }

        public GameCard GameCardFromContext(ActivationContext context)
        {
            if (!initialized) throw new NotImplementedException("Failed to initialize an ActivationContextCardIdentity!");
            return GameCardFromContextLogic(context);
        }

        protected abstract GameCard GameCardFromContextLogic(ActivationContext context);
    }

    public class GameContextCardIdentity : ActivationContextCardIdentity
    {
        public IGamestateCardIdentity gamestateCardIdentity;

        protected override GameCard GameCardFromContextLogic(ActivationContext context)
            => gamestateCardIdentity.GameCardFrom(context.game, context);
    }
}