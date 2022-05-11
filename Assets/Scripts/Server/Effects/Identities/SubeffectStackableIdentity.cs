using KompasCore.Effects;
using KompasCore.Effects.Identities;

namespace KompasServer.Effects.Identities
{
    public abstract class SubeffectStackableIdentity : ContextInitializeableBase
    {
        protected abstract IStackable AbstractStackable { get; }

        public IStackable Stackable
        {
            get
            {
                ComplainIfNotInitialized();
                return AbstractStackable;
            }
        }
    }

    namespace SubeffectStackableIdentities
    {
        public class ThisEffect : SubeffectStackableIdentity
        {
            protected override IStackable AbstractStackable => InitializationContext.effect;
        }

        public class FromActivationContext : SubeffectStackableIdentity
        {
            public IActivationContextIdentity<IStackable> stackable;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                stackable.Initialize(initializationContext);
            }

            protected override IStackable AbstractStackable
                => stackable.From(InitializationContext.subeffect.CurrentContext);
        }
    }
}