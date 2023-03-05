namespace KompasCore.Effects.Identities
{
    /// <summary>
    /// Can be used whether or not the caller does or doesn't care about an ActivationContext.
    /// (Eventually, can be used whether or not the caller does or doesn't care about an ActivationContext,
    /// but that interface doesn't even exist yet.)
    /// </summary>
    public interface IIdentity<ReturnType> : IContextInitializeable
    {
        public ReturnType From(ActivationContext context, ActivationContext secondaryContext);
    }

    /// <summary>
    /// An identity that needs context, either for itself or to pass on to its children.
    /// </summary>
    public abstract class ContextualIdentityBase<ReturnType> : ContextInitializeableBase,
        IIdentity<ReturnType>
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

    public abstract class LeafIdentityBase<ReturnType> : ContextInitializeableBase,
        IIdentity<ReturnType>
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