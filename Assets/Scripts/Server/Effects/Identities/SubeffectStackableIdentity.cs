using KompasCore.Effects;
using KompasCore.Effects.Identities;

namespace KompasServer.Effects.Identities.SubeffectStackableIdentities
{
    public class ThisEffect : SubeffectIdentityBase<IStackable>
    {
        protected override IStackable AbstractItem => InitializationContext.effect;
    }

    public class FromActivationContext : SubeffectIdentityBase<IStackable>
    {
        public IActivationContextIdentity<IStackable> stackable;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            stackable.Initialize(initializationContext);
        }

        protected override IStackable AbstractItem
            => stackable.From(InitializationContext.subeffect.CurrentContext, default);
    }

    public class StackableIndex : SubeffectIdentityBase<IStackable>
    {
        public int index = -1;

        protected override IStackable AbstractItem
            => EffectHelpers.GetItem(InitializationContext.effect.stackableTargets, index);
    }
}