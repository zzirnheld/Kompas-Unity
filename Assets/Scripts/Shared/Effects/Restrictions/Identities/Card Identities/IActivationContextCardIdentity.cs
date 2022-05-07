using KompasCore.Cards;

namespace KompasCore.Effects.Identities
{
    public interface IActivationContextCardIdentity
    {
        public GameCard GameCardFromContext(ActivationContext context);
    }

    public class GameContextCardIdentity : IActivationContextCardIdentity
    {
        public IGamestateCardIdentity gamestateCardIdentity;

        public GameCard GameCardFromContext(ActivationContext context)
            => gamestateCardIdentity.GameCardFrom(context.game, context);
    }
}