using KompasCore.Cards;

namespace KompasCore.Effects.Identities
{
    public abstract class ActivationContextCardIdentityBase : ContextInitializeableBase, 
        IActivationContextIdentity<GameCardBase>
    {
        public GameCardBase From(ActivationContext context)
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
            public IActivationContextIdentity<Space> position;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                position.Initialize(initializationContext);
            }

            protected override GameCardBase CardFromAbstract(ActivationContext context)
            {
                var finalSpace = position.From(context);
                return context.game.boardCtrl.GetCardAt(finalSpace);
            }
        }
    }
}