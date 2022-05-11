using KompasCore.Cards;

namespace KompasCore.Effects.Identities
{
    /// <summary>
    /// Uniquely identifies a singular card, w/r/t an ActivationContext
    /// </summary>
    public interface IActivationContextCardIdentity : IContextInitializeable
    {
        public GameCardBase CardFrom(ActivationContext context);
    }

    public abstract class ActivationContextCardIdentityBase : ContextInitializeableBase, IActivationContextCardIdentity
    {
        public GameCardBase CardFrom(ActivationContext context)
        {
            ComplainIfNotInitialized();
            return CardFromAbstract(context);
        }

        protected abstract GameCardBase CardFromAbstract(ActivationContext context);
    }

    namespace ActivationContextCardIdentities
    {

        public class MainCardBefore : ActivationContextCardIdentityBase
        {
            protected override GameCardBase CardFromAbstract(ActivationContext context)
                => context.mainCardInfoBefore;
        }

        public class CardAtPosition : ActivationContextCardIdentityBase
        {
            public IActivationContextSpaceIdentity position;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                position.Initialize(initializationContext);
            }

            protected override GameCardBase CardFromAbstract(ActivationContext context)
            {
                var finalSpace = position.SpaceFrom(context);
                return context.game.boardCtrl.GetCardAt(finalSpace);
            }
        }
    }
}