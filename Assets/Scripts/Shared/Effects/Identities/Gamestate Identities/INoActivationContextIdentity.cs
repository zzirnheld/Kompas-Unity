namespace KompasCore.Effects.Identities
{
    /// <summary>
    /// Can be used whether or not the caller does or doesn't care about an ActivationContext.
    /// (Eventually, can be used whether or not the caller does or doesn't care about an ActivationContext,
    /// but that interface doesn't even exist yet.)
    /// </summary>
    public interface INoActivationContextIdentity<ReturnType> : IContextInitializeable
    {
        public ReturnType Item { get; }
    }

    public abstract class NoActivationContextIdentityBase<ReturnType> : ContextInitializeableBase,
        IActivationContextIdentity<ReturnType>, INoActivationContextIdentity<ReturnType>
    {
        protected abstract ReturnType AbstractItem { get; }

        public ReturnType Item
        {
            get
            {
                ComplainIfNotInitialized();
                return AbstractItem;
            }
        }

        public ReturnType From(ActivationContext context, ActivationContext secondaryContext) => Item;
    }
}