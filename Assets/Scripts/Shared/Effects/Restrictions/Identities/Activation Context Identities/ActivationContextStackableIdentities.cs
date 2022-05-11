namespace KompasCore.Effects.Identities
{
    public interface IActivationContextIdentity<ReturnType> : IContextInitializeable
    {
        public ReturnType From(ActivationContext activationContext);
    }

    public abstract class ActivationContextStackableIdentityBase : ContextInitializeableBase,
        IActivationContextIdentity<IStackable>
    {
        protected abstract IStackable AbstractStackableFrom(ActivationContext activationContext);

        public IStackable From(ActivationContext activationContext)
        {
            ComplainIfNotInitialized();
            return AbstractStackableFrom(activationContext);
        }
    }

    namespace ActivationContextStackableIdentities
    {
        public class StackableCause : ActivationContextStackableIdentityBase
        {
            protected override IStackable AbstractStackableFrom(ActivationContext activationContext)
                => activationContext.stackableCause;
        }
    }
}